using System.Collections.Generic;
using Custom.Scripts.Helper;
using UnityEngine;
namespace Custom.Scripts.ExperimentGeneral
{
    // This script is used to activate the correct door in the experiment room
    public class ExperimentRoomDoorSelector : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> wallsWithoutDoor;
        [SerializeField]
        private List<GameObject> wallsWithDoor;

        void Start()
        {
            // Find all the doors in the scene
            var doorScriptList = FindObjectsByType<DoorHandler>(FindObjectsSortMode.None);

            foreach (var doorScript in doorScriptList)
            {
                // Check if the door leads to the previous scene and is in the correct orientation since there could be
                // multiple doors leading to the same scene
                if (doorScript.GetScene().ScenePath != SceneSpawnManager.Instance.previousScenePathName &&
                    Mathf.Abs(doorScript.transform.rotation.y - SceneSpawnManager.Instance.doorOrientation) < 0.1f)
                {
                    continue;
                }

                foreach (var wallWithDoor in wallsWithDoor)
                {
                    // Check if the wall that contains a door object has the matching door object, and if true iterate
                    // through the whole list of walls without door and walls with door to set their active state
                    // accordingly
                    if (doorScript.transform.IsChildOf(wallWithDoor.transform))
                    {
                        var wallIndex = wallsWithDoor.IndexOf(wallWithDoor);

                        foreach (var wall in wallsWithoutDoor)
                        {
                            if (wall == wallsWithoutDoor[wallIndex])
                            {
                                wall.SetActive(false);
                            }
                            else
                            {
                                wall.SetActive(true);
                            }
                        }

                        foreach (var wall in wallsWithDoor)
                        {
                            if (wall == wallsWithDoor[wallIndex])
                            {
                                wall.SetActive(true);
                            }
                            else
                            {
                                wall.SetActive(false);
                            }
                        }

                        return;
                    }
                }
            }
        }
    }
}
