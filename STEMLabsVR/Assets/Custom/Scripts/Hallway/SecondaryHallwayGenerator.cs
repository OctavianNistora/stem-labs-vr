using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Custom.Scripts.Helper;
using Custom.Scripts.UI;
using Tymski;
using UnityEditor;
using UnityEngine;
namespace Custom.Scripts.Hallway
{
    // This class is responsible for generating the secondary hallway in the game.
    public class SecondaryHallwayGenerator : MonoBehaviour
    {
        [SerializeField]
        private bool isWestSideHallway;
        [SerializeField]
        private bool isDeadEnded;
        [SerializeField]
        private List<SceneReference> sceneList;
        [SerializeField]
        private Camera vrCamera;

        [SerializeField]
        private SecondaryHallwayParts secondaryHallwayParts;

        public void SetSecondaryHallwayConfig(bool isWestSideHallway, bool isDeadEnded, List<SceneReference> sceneList, Camera vrCamera)
        {
            this.isWestSideHallway = isWestSideHallway;
            this.isDeadEnded = isDeadEnded;
            this.sceneList = sceneList;
            this.vrCamera = vrCamera;

            RebuildSecondaryHallway();
        }

        public void RebuildSecondaryHallway()
        {
            SetDoors();
            SetWalls();
            SetUiCamera();
        }

        // This method sets the correct doors of the secondary hallway based on the configuration.
        private void SetDoors()
        {
            // Method is empty when not in editor mode, as EditorBuildSettings when building the game.
            #if UNITY_EDITOR
            var buildScenes = EditorBuildSettings.scenes.Where(scene => scene.enabled)
                .Select(scene => scene.path).ToList();
            if (isWestSideHallway)
            {
                secondaryHallwayParts.westSouthDoor.SetActive(true);
                var currentDoorHandler = secondaryHallwayParts.westSouthDoor.GetComponent<DoorHandler>();
                currentDoorHandler.SetScene(sceneList[0], buildScenes.IndexOf(sceneList[0]));
                currentDoorHandler.SetDoorTitle(Regex.Match(sceneList[0],@"^(.+\/)*(.+)\.(.+)$").Groups[2].Value);
                secondaryHallwayParts.eastSouthDoor.SetActive(false);

                // Check if the hallway has a north door
                if (sceneList.Count > 1)
                {

                    secondaryHallwayParts.westNorthDoor.SetActive(true);
                    currentDoorHandler = secondaryHallwayParts.westNorthDoor.GetComponent<DoorHandler>();
                    currentDoorHandler.SetScene(sceneList[1], buildScenes.IndexOf(sceneList[1]));
                    currentDoorHandler.SetDoorTitle(Regex.Match(sceneList[1],@"^(.+\/)*(.+)\.(.+)$").Groups[2].Value);
                }
                else
                {
                    secondaryHallwayParts.westNorthDoor.SetActive(false);
                }
                secondaryHallwayParts.eastNorthDoor.SetActive(false);
            }
            else
            {
                secondaryHallwayParts.westSouthDoor.SetActive(false);
                secondaryHallwayParts.eastSouthDoor.SetActive(true);
                var currentDoorHandler = secondaryHallwayParts.eastSouthDoor.GetComponent<DoorHandler>();
                currentDoorHandler.SetScene(sceneList[0], buildScenes.IndexOf(sceneList[0]));
                currentDoorHandler.SetDoorTitle(Regex.Match(sceneList[0],@"^(.+\/)*(.+)\.(.+)$").Groups[2].Value);

                secondaryHallwayParts.westNorthDoor.SetActive(false);
                // Check if the hallway has a north door
                if (sceneList.Count > 1)
                {
                    secondaryHallwayParts.eastNorthDoor.SetActive(true);
                    currentDoorHandler = secondaryHallwayParts.eastNorthDoor.GetComponent<DoorHandler>();
                    currentDoorHandler.SetScene(sceneList[1], buildScenes.IndexOf(sceneList[1]));
                    currentDoorHandler.SetDoorTitle(Regex.Match(sceneList[1],@"^(.+\/)*(.+)\.(.+)$").Groups[2].Value);
                }
                else
                {
                    secondaryHallwayParts.eastNorthDoor.SetActive(false);
                }
            }
            #endif
        }

        private void SetWalls()
        {
            if (isWestSideHallway)
            {
                secondaryHallwayParts.westWall.SetActive(isDeadEnded);
                secondaryHallwayParts.eastWall.SetActive(false);
            }
            else
            {
                secondaryHallwayParts.westWall.SetActive(false);
                secondaryHallwayParts.eastWall.SetActive(isDeadEnded);
            }
        }
        
        private void SetUiCamera()
        {
            var canvasList = gameObject.GetComponentsInChildren<Canvas>();
            foreach (var canvas in canvasList)
            {
                if (canvas.renderMode == RenderMode.WorldSpace)
                {
                    canvas.worldCamera = vrCamera;
                    canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.None;
                }
            }
        }

        // Auxiliary struct to hold the parts of the secondary hallway
        [System.Serializable]
        struct SecondaryHallwayParts
        {
            public GameObject westNorthDoor;
            public GameObject westSouthDoor;
            public GameObject eastNorthDoor;
            public GameObject eastSouthDoor;
            public GameObject eastWall;
            public GameObject westWall;
        }
    }
}
