using Tymski;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Custom.Scripts.Helper
{
    // Script used to transition between scenes when the player enters a door
    public class DoorSceneSwitchScript : MonoBehaviour
    {
        [SerializeField]
        private SceneReference sceneToLoad;
        [SerializeField]
        private Transform spawnPoint;

        void Start()
        {
            SceneSpawnManager.Instance.isTransitioning = false;
        }

        // Getter method to get the scene to load when the player enters the door
        public SceneReference GetScene()
        {
            return sceneToLoad;
        }
    
        // Setter method to set the scene to load when the player enters the door
        public void SetScene(SceneReference scene)
        {
            sceneToLoad = scene;
        }
        
        public SpawnPointStruct GetSpawnPoint()
        {
            return new SpawnPointStruct(spawnPoint.position, spawnPoint.rotation);
        }

        // Method to be called when the player enters the door, which starts the transition to the new scene
        public void Switch()
        {
            if (NetworkManager.Singleton)
            {
                Destroy(NetworkManager.Singleton.gameObject);
                NetworkManager.Singleton.Shutdown();
            }
            
            if (SceneSpawnManager.Instance.isTransitioning)
            {
                return;
            }
            SceneSpawnManager.Instance.isTransitioning = true;
        
            SceneSpawnManager.Instance.previousScenePathName = SceneManager.GetActiveScene().path;
            SceneSpawnManager.Instance.doorOrientation = transform.rotation.eulerAngles.y;
            SceneManager.LoadSceneAsync(sceneToLoad);
        }
    }
}
