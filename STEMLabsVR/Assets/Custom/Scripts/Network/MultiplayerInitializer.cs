using System;
using System.Collections;
using System.Threading.Tasks;
using Custom.Scripts.Data.Static;
using Custom.Scripts.Helper;
using Custom.Scripts.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WebSocketSharp;

namespace Custom.Scripts.Debugging
{
    public class MultiplayerInitializer : MonoBehaviour
    {
        async void Start()
        {
            // Check if there is a NetworkManager instance in the scene
            if (!NetworkManager.Singleton)
            {
                return;
            }

            try
            {
                if (SessionData.inviteCode.IsNullOrEmpty())
                {
                    SessionData.isClientHost = true;
                    
                    var inviteCode = await StartHostWithRelay(40, "dtls");
                    if (string.IsNullOrEmpty(inviteCode))
                    {
                        throw new Exception("Failed to start host");
                    }
                    SessionData.inviteCode = inviteCode;

                    if (AuthData.role.ToLower() == "professor")
                    {
                        StartCoroutine(CreateSession());
                    }
                }
                else
                {
                    SessionData.isClientHost = false;
                    
                    var connectedSuccessfully = await StartClientWithRelay(SessionData.inviteCode, "dtls");
                    if (!connectedSuccessfully)
                    {
                        throw new Exception("Failed to connect to remote host");
                    }
                }
            }
            catch (Exception)
            {
                var door = FindObjectOfType<DoorHandler>();
                if (door)
                {
                    door.SceneSwitchButtonClicked();
                }
            }
        }

        private async Task<string> StartHostWithRelay(int maxConnections, string connectionType)
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
            var joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return NetworkManager.Singleton.StartHost() ? joinCode : null;
        }

        private async Task<bool> StartClientWithRelay(string joinCode, string connectionType)
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, connectionType));
            return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
        }

        private IEnumerator CreateSession()
        {
            var www = UnityWebRequest.Post($"{AppConfig.ServerHostName}/api/laboratory-sessions",
                $"{{\"creatorId\":{AuthData.id},\"sceneId\":{SceneManager.GetActiveScene().buildIndex},\"inviteCode\":\"{SessionData.inviteCode}\"}}",
                "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + AuthData.accessToken);
            
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success)
            {
                var door = FindObjectOfType<DoorHandler>();
                if (door)
                {
                    door.SceneSwitchButtonClicked();
                }
            }
        }
    }
}
