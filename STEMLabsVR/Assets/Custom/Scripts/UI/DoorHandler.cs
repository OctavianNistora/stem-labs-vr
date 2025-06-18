using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Custom.Scripts.Data.Dtos;
using Custom.Scripts.Data.Static;
using Custom.Scripts.Data.Structures;
using Custom.Scripts.Data.Wrappers;
using Custom.Scripts.ExperimentGeneral;
using Custom.Scripts.Helper;
using TMPro;
using Tymski;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Custom.Scripts.UI
{
    // This class handles the door interactions in the game, allowing players to enter different scenes and manage login
    // functionality.
    public class DoorHandler : MonoBehaviour
    {
        [SerializeField] private SceneReference sceneToLoad;
        [SerializeField] private int sceneId;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private bool isHallwayDoor;
        [SerializeField] private GameObject uiParent;
        [SerializeField] private TextMeshProUGUI doorTitleText;
        [SerializeField] private GameObject interactableUi;
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button guestLoginButton;
        [SerializeField] private GameObject laboratoryTypeSelectionPanel;
        [SerializeField] private Button joinButton;
        [SerializeField] private ClipboardHandler clipboardHandler;
        [SerializeField] private RenderTexture whiteboardRenderTexture;
        [SerializeField] private TextMeshProUGUI doorLoginErrorText;
    
        private static DoorHandler _selectedDoor;
        private static bool _isWaitingResponse;
        private string _username;
        private string _password;
        private string _inviteCode;
        private List<IMultipartFormSection> _savedMultipartForm;
        private Coroutine currentLoginErrorCoroutine;

        // This method is used to initially disable the interactable UI when the door is instantiated, change the initial
        // panel to be displayed based on the user's login state, and subscribe to the OnIdChanged event to change the
        // panel when the user's ID changes.
        private void Start()
        {
            SceneSpawnManager.Instance.isTransitioning = false;
            interactableUi.SetActive(false);
        
            AuthData.OnIdChanged += ChangePanel;
            ChangePanel(AuthData.id);
        
            HandleLoginButtonState();
            joinButton.interactable = false;
            guestLoginButton.interactable = AuthData.isGuest;

            if (isHallwayDoor)
            {
                uiParent.transform.Rotate(Vector3.forward, 180f);
            }
        }

        // Method used to change the active panel based on the user's login state.
        private void ChangePanel(int? userId)
        {
            var doesRequireLogin = userId == null;

            loginPanel.SetActive(doesRequireLogin);
            laboratoryTypeSelectionPanel.SetActive(!doesRequireLogin);
        }

        // Method called when the player interacts with the door
        public void DoorInteracted()
        {
            // If the scene transition is in progress or the user is waiting for a response from the server to avoid
            // multiple requests, it returns early to avoid any conflicts.
            if (SceneSpawnManager.Instance.isTransitioning)
            {
                return;
            }
            
            // Checks if the door is a hallway door or the user failed to send the multipart form data due to an expired
            // access token.
            if (isHallwayDoor || _savedMultipartForm != null)
            {
                if (_selectedDoor && _selectedDoor != this)
                {
                    _selectedDoor.DisableInteractableUi();
                }
                _selectedDoor = this;

                interactableUi.SetActive(true);
                return;
            }
        
            // Returns early if the user is .
            if (_isWaitingResponse)
            {
                return;
            }
            
            // If the user is a guest or the host of the laboratory session, it switches the scene immediately.
            if (SessionData.isClientHost || AuthData.isGuest)
            {
                SceneSwitchButtonClicked();
                return;
            }
            
            // Create a multipart form data to send the report to the server.
            var form = new List<IMultipartFormSection>();
            form.Add(new MultipartFormDataSection("submitterId", AuthData.id.ToString()));
            form.Add(new MultipartFormDataSection("InvitedCode", SessionData.inviteCode));
            foreach (var step in clipboardHandler.GetStepsCompleted())
            {
                form.Add(new MultipartFormDataSection("StepsCompleted", step.ToString()));
            }
            if (clipboardHandler.AreObservationsMade())
            {
                var whiteboardTexture = new Texture2D(whiteboardRenderTexture.width, whiteboardRenderTexture.height,
                    TextureFormat.RGBA32, false);
                var oldRT = RenderTexture.active;
            
                RenderTexture.active = whiteboardRenderTexture;
                whiteboardTexture.ReadPixels(
                    new Rect(0, 0, whiteboardRenderTexture.width, whiteboardRenderTexture.height), 0, 0);
                whiteboardTexture.Apply();
                RenderTexture.active = oldRT;
            
                var bytes = whiteboardTexture.EncodeToJPG();
                form.Add(new MultipartFormFileSection("ObservationsImage", bytes, "whiteboard.jpg",
                    "image/jpeg"));
            }
            
            StartCoroutine(UploadReport(form));
        }

        private void DisableInteractableUi()
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
    
        // This method checks if the username and password fields are empty, and sets the login button's interactable
        // state accordingly.
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
        
        // This method is called when the user clicks the guest login button. It sets the AuthData properties to indicate
        // that the user is a guest.
        public void GuestLoginClicked()
        {
            if (_isWaitingResponse)
            {
                return;
            }
        
            AuthData.isGuest = true;
            AuthData.id = 0;
            AuthData.accessToken = "";
            AuthData.fullName = "Guest";
            AuthData.role = "Guest";
        }
    
        // This method is called when the user clicks the login button. It checks if the user is already waiting for a 
        // response from the server, and if not, it starts the login coroutine. If the user has a saved multipart form,
        // it attempts to re-login and re-upload the form data.
        public void LogInClicked()
        {
            if (_isWaitingResponse)
            {
                return;
            }

            if (_savedMultipartForm == null)
            {
                StartCoroutine(Login());
            }
            else
            {
                StartCoroutine(AttemptReloginAndReupload());
            }
        }
    
        // This coroutine handles the login process by sending a POST request to the server with the user's credentials.
        private IEnumerator Login()
        {
            _isWaitingResponse = true;

            using (UnityWebRequest www = UnityWebRequest.Post($"{AppConfig.ServerHostName}/api/auth/session",
                       $"{{\n  \"username\": \"{_username}\",\n  \"password\": \"{_password}\",\n  \"respondWithRefreshToken\": false\n}}",
                       "application/json"))
            {
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    if (www.responseCode == 401)
                    {
                        doorLoginErrorText.text = www.downloadHandler.text;
                    }
                    else if (www.responseCode == 500)
                    {
                        doorLoginErrorText.text = "Internal server error. Please try again later.";
                    }
                    else
                    {
                        doorLoginErrorText.text = www.error;
                    }
                    if (currentLoginErrorCoroutine != null)
                    {
                        StopCoroutine(currentLoginErrorCoroutine);
                    }
                    currentLoginErrorCoroutine = StartCoroutine(ResetLoginErrorText());
                }
                else
                {
                    var response = JsonUtility.FromJson<LoginResponseDto>(www.downloadHandler.text);
                    AuthData.isGuest = false;
                    AuthData.id = response.uid;
                    AuthData.accessToken = response.accessToken;
                    AuthData.fullName = response.fullName;
                    AuthData.role = response.role;
                }
            }
        
            _isWaitingResponse = false;
        }
        
        private IEnumerator ResetLoginErrorText()
        {
            yield return new WaitForSeconds(3f);
            doorLoginErrorText.text = "";
        }
    
        // This method is called when the user clicks the host button. It sets the SessionData properties to indicate that
        // the user is the host of the laboratory session and starts the coroutine to get data from the server and switch
        // scenes.
        public void HostClicked()
        {
            if (_isWaitingResponse)
            {
                return;
            }
            SessionData.inviteCode = "";
            SessionData.isClientHost = true;
            StartCoroutine(GetDataFromServerAndSwitchScene());
        }

        // This method is called when the user clicks the join button. It returns early if the user is already waiting
        // for a response or if the invite code is empty. Otherwise, it sets the SessionData properties to indicate that
        // the user is joining as a client and starts the coroutine to get data from the server and switch scenes.
        public void JoinClicked()
        {
            if (_isWaitingResponse || string.IsNullOrEmpty(_inviteCode))
            {
                return;
            }
            SessionData.inviteCode = _inviteCode;
            SessionData.isClientHost = false;
            StartCoroutine(GetDataFromServerAndSwitchScene());
        }

        // This coroutine sends a GET request to the server to retrieve the checklist steps for the laboratory session
        // and switches the scene once the data is received. If the request fails, it logs the error and sets an empty
        // checklist steps list.
        private IEnumerator GetDataFromServerAndSwitchScene()
        {
            _isWaitingResponse = true;
        
            using (UnityWebRequest www = UnityWebRequest.Get($"{AppConfig.ServerHostName}/api/laboratories/{sceneId}/steps"))
            {
                www.SetRequestHeader("Authorization", "Bearer " + AuthData.accessToken);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError(www.error);
                    SessionData.checklistSteps = new List<string>();
                }
                else
                {
                    var response = JsonUtility.FromJson<JsonListWrapper<string>>("{\"items\":" + www.downloadHandler.text + "}");
                    SessionData.checklistSteps = response.items;
                }
            }
        
            SceneSwitchButtonClicked();
            _isWaitingResponse = false;
        }
    
        // Method used to switch the scene when the player enters the door. It destroys the NetworkManager if it exists,
        // disables the interactable UI of the selected door, and loads the new scene asynchronously. It also sets the
        // previous scene path name and door orientation in the SceneSpawnManager.
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
            
            SceneSpawnManager.Instance.isTransitioning = true;
        
            SceneSpawnManager.Instance.previousScenePathName = SceneManager.GetActiveScene().path;
            SceneSpawnManager.Instance.doorOrientation = transform.rotation.eulerAngles.y;
            SceneManager.LoadSceneAsync(sceneToLoad);
        }
    
        public SceneReference GetScene()
        {
            return sceneToLoad;
        }
    
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
    
        // This coroutine uploads the report to the server using a multipart form data. If the request fails with a 401
        // status code, it saves the multipart form data and sets the interactable UI to active. If the request fails for
        // any other reason, it saves the form data as a file to prevent data loss. On successful upload, it switches
        // the scene by calling the SceneSwitchButtonClicked method.
        IEnumerator UploadReport(List<IMultipartFormSection> form)
        {
            _isWaitingResponse = true;
            using (UnityWebRequest www = UnityWebRequest.Post($"{AppConfig.ServerHostName}/api/laboratory-reports", form))
            {
                www.SetRequestHeader("Authorization", "Bearer " + AuthData.accessToken);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    if (www.responseCode == 401)
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

            _isWaitingResponse = false;
            SceneSwitchButtonClicked();
        }

        public void SetClipboard(ClipboardHandler clipboardHandler)
        {
            this.clipboardHandler = clipboardHandler;
        }
        
        // This method saves the multipart form data as a JSON file in the persistent data path.
        private void SaveFormAsFile(List<IMultipartFormSection> form)
        {
            var filePath = System.IO.Path.Combine(Application.persistentDataPath,
                $"formdata_{AuthData.id}_{SceneManager.GetActiveScene().buildIndex}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.json");
            using (var file = System.IO.File.CreateText(filePath))
            {
                file.WriteLine("[");
                for (int i = 0; i < form.Count; i++)
                {
                    file.WriteLine("\t{");
                    
                    file.WriteLine($"\t\tsectionName: \"{form[i].sectionName}\",");
                    if (form[i].sectionName == "ObservationsImage")
                    {
                        file.WriteLine($"\t\tsectionData: \"{Convert.ToBase64String(form[i].sectionData)}\"");
                    }
                    else
                    {
                        file.WriteLine($"\t\tsectionData: \"{Encoding.UTF8.GetString(form[i].sectionData)}\"");
                    }
                    
                    if (i < form.Count - 1)
                    {
                        file.WriteLine("\t},");
                    }
                    else
                    {
                        file.WriteLine("\t}");
                    }
                }
                file.WriteLine("]");
            }
        }
        
        // This coroutine attempts to re-login the user and re-upload the saved multipart form data if the access token has
        // expired. It sends a POST request to the server with the user's credentials, updates the AuthData properties,
        // and then tries to re-upload the saved multipart form data. If the re-upload fails, it saves the form data as a
        // file to prevent data loss and/or prevent keeping the user stuck in the laboratory room.
        private IEnumerator AttemptReloginAndReupload()
        {
            _isWaitingResponse = true;

            var www = UnityWebRequest.Post($"{AppConfig.ServerHostName}/api/auth/session",
                $"{{\n  \"username\": \"{_username}\",\n  \"password\": \"{_password}\",\n  \"respondWithRefreshToken\": false\n}}",
                "application/json");
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                if (www.responseCode == 401)
                {
                    doorLoginErrorText.text = www.downloadHandler.text;
                }
                else if (www.responseCode == 500)
                {
                    doorLoginErrorText.text = "Internal server error. Please try again later.";
                }
                else
                {
                    doorLoginErrorText.text = www.error;
                }
                if (currentLoginErrorCoroutine != null)
                {
                    StopCoroutine(currentLoginErrorCoroutine);
                }
                currentLoginErrorCoroutine = StartCoroutine(ResetLoginErrorText());
                yield break;
            }
            var response = JsonUtility.FromJson<LoginResponseDto>(www.downloadHandler.text);
            AuthData.id = response.uid;
            AuthData.accessToken = response.accessToken;
            AuthData.fullName = response.fullName;
            AuthData.role = response.role;
            
            interactableUi.SetActive(false);
            
            www = UnityWebRequest.Post($"{AppConfig.ServerHostName}/api/laboratory-reports", _savedMultipartForm);
            www.SetRequestHeader("Authorization", "Bearer " + AuthData.accessToken);
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
                SaveFormAsFile(_savedMultipartForm);
            }
            
            _isWaitingResponse = false;
            SceneSwitchButtonClicked();
        }
        
        public void SetCliboardHandler(ClipboardHandler clipboardHandler)
        {
            this.clipboardHandler = clipboardHandler;
        }
        
        public void SetDoorTitle(string title)
        {
            doorTitleText.text = title;
        }
    }
}
