using Custom.Scripts.UI;
using UnityEngine;

namespace Custom.Scripts.Helper
{
    // Script to handle the spawn point of the XR Origin in the scene
    public class XROriginSpawnHandler : MonoBehaviour
    {
        [SerializeField] private Transform defaultSpawnPoint;
        
        void Start()
        {
            // Check if current scene is the first scene to be loaded after the game starts, in which case use the
            // default spawn point
            if (string.IsNullOrEmpty(SceneSpawnManager.Instance.previousScenePathName))
            {
                if (defaultSpawnPoint)
                {
                    transform.position = defaultSpawnPoint.position;
                    transform.rotation = defaultSpawnPoint.rotation;
                }
                return;
            }

            // Find all door scripts in the scene and check if the previous scene path matches
            var doorScriptList = FindObjectsByType<DoorHandler>(FindObjectsSortMode.None);
        
            // Iterate through each door script to find the matching scene path and if found, set the spawn point
            foreach (var doorScript in doorScriptList)
            {
                if (doorScript.GetScene().ScenePath == SceneSpawnManager.Instance.previousScenePathName)
                {
                    var spawnPoint = doorScript.GetSpawnPoint();
                    transform.position = spawnPoint.position;
                    transform.rotation = spawnPoint.rotation;
                    break;
                }
            }
        }
    }
}
