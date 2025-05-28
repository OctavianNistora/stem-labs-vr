using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Custom.Scripts.ExperimentGeneral;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Tymski;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Custom.Scripts.Helper
{
    public class DoorHandler : MonoBehaviour
    {
        [SerializeField] private SceneReference sceneToLoad;
        [SerializeField] private int sceneId;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool isHallwayDoor;
        [SerializeField] private GameObject interactableUi;
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private Button loginButton;
        [SerializeField] private GameObject laboratoryTypeSelectionPanel;
        [SerializeField] private Button joinButton;
        [SerializeField] private ClipboardHandler clipboardHandler;
        [SerializeField] private RenderTexture whiteboardRenderTexture;
    
        private static DoorHandler _selectedDoor;
        private static bool _isWaitingResponse;
        private string _username;
        private string _password;
        private string _inviteCode;
        private List<IMultipartFormSection> _savedMultipartForm;

        private void Start()
        {
            SceneSpawnManager.Instance.isTransitioning = false;
            interactableUi.SetActive(false);
        
            AuthData.OnIdChanged += ChangePanel;
            ChangePanel(AuthData.id);
        
            HandleLoginButtonState();
            joinButton.interactable = false;
        }

        private void ChangePanel(int? userId)
        {
            var doesRequireLogin = userId == null;

            loginPanel.SetActive(doesRequireLogin);
            laboratoryTypeSelectionPanel.SetActive(!doesRequireLogin);
        }

        public void DoorInteracted()
        {
            if (isHallwayDoor)
            {
                if (_selectedDoor && _selectedDoor != this)
                {
                    _selectedDoor.DisableInteractableUi();
                }
                _selectedDoor = this;

                interactableUi.SetActive(true);
                return;
            }
        
            if (_isWaitingResponse)
            {
                return;
            }

            if (_savedMultipartForm != null)
            {
                AtemptReloginAndReupload();
                return;
            }
            
            var form = new List<IMultipartFormSection>();
            form.Add(new MultipartFormDataSection("submitterId", AuthData.id.ToString()));
            form.Add(new MultipartFormDataSection("InvitedCode", SessionData.inviteCode));
            foreach (var step in clipboardHandler.GetStepsCompleted())
            {
                form.Add(new MultipartFormDataSection("StepsCompleted", step.ToString()));
            }
            if (clipboardHandler.AreObservationsMade())
            {
                var whiteboardTexture = new Texture2D(whiteboardRenderTexture.width, whiteboardRenderTexture.height, TextureFormat.RGBA32, false);
                var oldRT = RenderTexture.active;
            
                RenderTexture.active = whiteboardRenderTexture;
                whiteboardTexture.ReadPixels(new Rect(0, 0, whiteboardRenderTexture.width, whiteboardRenderTexture.height), 0, 0);
                whiteboardTexture.Apply();
                RenderTexture.active = oldRT;
            
                var bytes = whiteboardTexture.EncodeToJPG();
                form.Add(new MultipartFormFileSection("ObservationsImage", bytes, "whiteboard.jpg", "image/jpeg"));
            }
            
            StartCoroutine(UploadReport(form));
        }

        public void DisableInteractableUi()
        {
            interactableUi.SetActive(false);
        }
    
        public void UsernameChanged(string username)
        {
            _username = username;
            HandleLoginButtonState();
        }
    
        public void PasswordChanged(string password)
        {
            _password = password;
            HandleLoginButtonState();
        }
    
        private void HandleLoginButtonState()
        {
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                loginButton.interactable = false;
            }
            else
            {
                loginButton.interactable = true;
            }
        }
    
        public void LogInClicked()
        {
            if (_isWaitingResponse)
            {
                return;
            }
            StartCoroutine(Login());
        }
    
        private IEnumerator Login()
        {
            _isWaitingResponse = true;
        
            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:5096/api/auth/session", $"{{\n  \"username\": \"{_username}\",\n  \"password\": \"{_password}\",\n  \"respondWithRefreshToken\": false\n}}", "application/json"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    var response = JsonUtility.FromJson<LoginResponseDto>(www.downloadHandler.text);
                    AuthData.id = response.uid;
                    AuthData.accessToken = response.accessToken;
                    AuthData.fullName = response.fullName;
                    AuthData.role = response.role;
                
                    loginPanel.SetActive(false);
                    laboratoryTypeSelectionPanel.SetActive(true);
                }
            }
        
            _isWaitingResponse = false;
        }
    
        public void HostClicked()
        {
            if (_isWaitingResponse)
            {
                return;
            }
            SessionData.inviteCode = "";
            StartCoroutine(GetDataFromServerAndSwitchScene());
        }

        public void JoinClicked()
        {
            if (_isWaitingResponse || string.IsNullOrEmpty(_inviteCode))
            {
                return;
            }
            SessionData.inviteCode = _inviteCode;
            StartCoroutine(GetDataFromServerAndSwitchScene());
        }

        private IEnumerator GetDataFromServerAndSwitchScene()
        {
            _isWaitingResponse = true;
        
            using (UnityWebRequest www = UnityWebRequest.Get($"http://localhost:5096/api/laboratories/{sceneId}/steps"))
            {
                www.SetRequestHeader("Authorization", "Bearer " + AuthData.accessToken);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                }
                else
                {
                    var response = JsonUtility.FromJson<JsonListWrapper<string>>("{\"items\":" + www.downloadHandler.text + "}");
                    SessionData.checklistSteps = response.items;
                }
            }
        
            _isWaitingResponse = false;
            SceneSwitchButtonClicked();
        }
    
        public void SceneSwitchButtonClicked()
        {
            if (NetworkManager.Singleton)
            {
                Destroy(NetworkManager.Singleton.gameObject);
                NetworkManager.Singleton.Shutdown();
            }
        
            if (_selectedDoor)
            {
                _selectedDoor.DisableInteractableUi();
            }
            _selectedDoor = null;
        
            if (SceneSpawnManager.Instance.isTransitioning)
            {
                return;
            }
            SceneSpawnManager.Instance.isTransitioning = true;
        
            SceneSpawnManager.Instance.previousScenePathName = SceneManager.GetActiveScene().path;
            SceneSpawnManager.Instance.doorOrientation = transform.rotation.eulerAngles.y;
            SceneManager.LoadSceneAsync(sceneToLoad);
        }
    
        // Getter method to get the scene to load when the player enters the door
        public SceneReference GetScene()
        {
            return sceneToLoad;
        }
    
        // Setter method to set the scene to load when the player enters the door
        public void SetScene(SceneReference scene, int sceneId)
        {
            sceneToLoad = scene;
            this.sceneId = sceneId;
        }
        
        public SpawnPointStruct GetSpawnPoint()
        {
            return new SpawnPointStruct(spawnPoint.position, spawnPoint.rotation);
        }
    
        public void InviteCodeChanged(string inviteCode)
        {
            _inviteCode = inviteCode;
            joinButton.interactable = !string.IsNullOrEmpty(_inviteCode);
        }
    
        IEnumerator UploadReport(List<IMultipartFormSection> form)
        {
            _isWaitingResponse = true;
            using (UnityWebRequest www = UnityWebRequest.Post("http://localhost:5096/api/laboratory-reports", form))
            {
                www.SetRequestHeader("Authorization", "Bearer " + AuthData.accessToken);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    if (www.responseCode == 403)
                    {
                        _savedMultipartForm = form;
                        
                        AuthData.id = null;
                        interactableUi.SetActive(true);
                        
                        _isWaitingResponse = false;

                        yield break;
                    }
                    
                    SaveFormAsFile(form);
                }
            }

            SceneSwitchButtonClicked();
        }

        public void SetClipboard(ClipboardHandler clipboardHandler)
        {
            this.clipboardHandler = clipboardHandler;
        }
        
        private void SaveFormAsFile(List<IMultipartFormSection> form)
        {
            var filePath = System.IO.Path.Combine(Application.persistentDataPath, $"formdata_{System.DateTime.Now:yyyy_MM_dd_HH_mm_ss}.json");
            using (var file = System.IO.File.CreateText(filePath))
            {
                file.WriteLine("{");
                foreach (var section in form)
                {
                    file.WriteLine($"\t{section.sectionName}: \"{Encoding.Default.GetString(section.sectionData)}\",");
                }
                file.WriteLine("}");
            }
        }
        
        IEnumerator AtemptReloginAndReupload()
        {
            _isWaitingResponse = true;

            var www = UnityWebRequest.Post("http://localhost:5096/api/auth/session",
                $"{{\n  \"username\": \"{_username}\",\n  \"password\": \"{_password}\",\n  \"respondWithRefreshToken\": false\n}}",
                "application/json");
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                yield break;
            }
            var response = JsonUtility.FromJson<LoginResponseDto>(www.downloadHandler.text);
            AuthData.id = response.uid;
            AuthData.accessToken = response.accessToken;
            AuthData.fullName = response.fullName;
            AuthData.role = response.role;
            
            interactableUi.SetActive(false);
            
            www = UnityWebRequest.Post("http://localhost:5096/api/laboratory-reports", _savedMultipartForm);
            www.SetRequestHeader("Authorization", "Bearer " + AuthData.accessToken);
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                SaveFormAsFile(_savedMultipartForm);
            }
            
            SceneSwitchButtonClicked();
        }
        
        public void SetCliboardHandler(ClipboardHandler clipboardHandler)
        {
            this.clipboardHandler = clipboardHandler;
        }
    }
}
