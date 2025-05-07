using Unity.Netcode;
using UnityEngine;
namespace Custom.Scripts.Debugging
{
    // This script is used to start a local multiplayer in host/client mode by checking if the game is running in the
    // editor or not and choosing the host/client mode accordingly.
    public class LocalMultiplayerDebugging : MonoBehaviour
    {
        [SerializeField]
        private bool isEditorHost = true;
        
        void Start()
        {
            // Check if there is a NetworkManager instance in the scene
            if (!NetworkManager.Singleton)
            {
                return;
            }
            
            if (Application.isEditor == isEditorHost)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}
