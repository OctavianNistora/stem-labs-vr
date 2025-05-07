using System.Collections.Generic;
using Custom.Scripts.Helper;
using Tymski;
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
        private SecondaryHallwayParts secondaryHallwayParts;

        public void SetSecondaryHallwayConfig(bool isWestSideHallway, bool isDeadEnded, List<SceneReference> sceneList)
        {
            this.isWestSideHallway = isWestSideHallway;
            this.isDeadEnded = isDeadEnded;
            this.sceneList = sceneList;

            RebuildSecondaryHallway();
        }

        public void RebuildSecondaryHallway()
        {
            SetDoors();
            SetWalls();
        }

        // This method sets the correct doors of the secondary hallway based on the configuration.
        private void SetDoors()
        {
            if (isWestSideHallway)
            {
                secondaryHallwayParts.westSouthDoor.SetActive(true);
                secondaryHallwayParts.westSouthDoor.GetComponent<DoorSceneSwitchScript>().SetScene(sceneList[0]);
                secondaryHallwayParts.eastSouthDoor.SetActive(false);

                // Check if the hallway has a north door
                if (sceneList.Count > 1)
                {

                    secondaryHallwayParts.westNorthDoor.SetActive(true);
                    secondaryHallwayParts.westNorthDoor.GetComponent<DoorSceneSwitchScript>()
                        .SetScene(sceneList[1]);
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
                secondaryHallwayParts.eastSouthDoor.GetComponent<DoorSceneSwitchScript>().SetScene(sceneList[0]);

                secondaryHallwayParts.westNorthDoor.SetActive(false);
                // Check if the hallway has a north door
                if (sceneList.Count > 1)
                {
                    secondaryHallwayParts.eastNorthDoor.SetActive(true);
                    secondaryHallwayParts.eastNorthDoor.GetComponent<DoorSceneSwitchScript>()
                        .SetScene(sceneList[1]);
                }
                else
                {
                    secondaryHallwayParts.eastNorthDoor.SetActive(false);
                }
            }
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
