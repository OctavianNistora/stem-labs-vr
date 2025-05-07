using System.Collections.Generic;
using Tymski;
using UnityEngine;

namespace Custom.Scripts.Hallway
{
    // Script that enables the generation of the primary hallway within the editor.
    public class PrimaryHallwayGenerator : MonoBehaviour
    {
        [SerializeField]
        private bool isFirstIteration;
        [SerializeField]
        private bool isDeadEndedNorth;
        [SerializeField]
        private bool isDeadEndedSouth;
        [SerializeField]
        private bool isFloorElevated;
        [SerializeField]
        private bool isCeilingRoof;
        [SerializeField]
        private List<SceneReference> westHallwayScenes;
        [SerializeField]
        private List<SceneReference> eastHallwayScenes;

        [SerializeField]
        private PrimaryHallwayParts primaryHallwayParts;

        // Sets the parameters for the primary hallway.
        public void SetHallwayConfig(bool isFirstIteration, bool isDeadEndedNorth, bool isDeadEndedSouth,
            bool isFloorElevated, bool isCeilingRoof, List<SceneReference> westHallwayScenes,
            List<SceneReference> eastHallwayScenes)
        {
            this.isFirstIteration = isFirstIteration;
            this.isDeadEndedNorth = isDeadEndedNorth;
            this.isDeadEndedSouth = isDeadEndedSouth;
            this.isFloorElevated = isFloorElevated;
            this.isCeilingRoof = isCeilingRoof;
            this.westHallwayScenes = westHallwayScenes;
            this.eastHallwayScenes = eastHallwayScenes;

            RebuildPrimaryHallway();
        }

        // Rebuilds the primary hallway based on the set parameters.
        public void RebuildPrimaryHallway()
        {
            SetNorthWall();
            SetSouthWall();

            SetFloor();
            SetCeiling();

            SetWestWallAndHallway();
            SetEastWallAndHallway();

            ResetSecondaryHallways();
        }

        private void SetNorthWall()
        {
            if (isDeadEndedNorth)
            {
                primaryHallwayParts.northWall.SetActive(true);
            }
            else
            {
                primaryHallwayParts.northWall.SetActive(false);
            }
        }

        private void SetSouthWall()
        {
            if (isFirstIteration)
            {
                primaryHallwayParts.southWall.SetActive(true);
                primaryHallwayParts.doubleDoorExit.SetActive(true);
            }
            else if (isDeadEndedSouth)
            {
                primaryHallwayParts.southWall.SetActive(true);
                primaryHallwayParts.doubleDoorExit.SetActive(false);
            }
            else
            {
                primaryHallwayParts.southWall.SetActive(false);
                primaryHallwayParts.doubleDoorExit.SetActive(false);
            }
        }

        private void SetFloor()
        {
            if (isFloorElevated)
            {
                primaryHallwayParts.groundFloor.SetActive(false);
                primaryHallwayParts.upperFloor.SetActive(true);
            }
            else
            {
                primaryHallwayParts.groundFloor.SetActive(true);
                primaryHallwayParts.upperFloor.SetActive(false);
            }
        }

        private void SetCeiling()
        {
            if (isCeilingRoof)
            {
                primaryHallwayParts.stairs.SetActive(false);
                primaryHallwayParts.roofCeiling.SetActive(true);
                primaryHallwayParts.floorCeiling.SetActive(false);
            }
            else
            {
                primaryHallwayParts.stairs.SetActive(true);
                primaryHallwayParts.roofCeiling.SetActive(false);
                primaryHallwayParts.floorCeiling.SetActive(true);
            }
        }

        private void SetWestWallAndHallway()
        {
            if (westHallwayScenes.Count > 0)
            {
                primaryHallwayParts.westWallEmpty.SetActive(false);
                primaryHallwayParts.westWallRooms.SetActive(true);
            }
            else
            {
                primaryHallwayParts.westWallEmpty.SetActive(true);
                primaryHallwayParts.westWallRooms.SetActive(false);
            }
        }

        private void SetEastWallAndHallway()
        {
            if (eastHallwayScenes.Count > 0)
            {
                primaryHallwayParts.eastWallEmpty.SetActive(false);
                primaryHallwayParts.eastWallRooms.SetActive(true);
            }
            else
            {
                primaryHallwayParts.eastWallEmpty.SetActive(true);
                primaryHallwayParts.eastWallRooms.SetActive(false);
            }
        }

        // Removes the existing secondary hallways (if they exist) and creates new ones based on the current
        // configuration.
        private void ResetSecondaryHallways()
        {
            // Check if the there are any existing secondary hallways and remove them.
            var existingHallways = transform.Find("SecondaryHallways");
            if (existingHallways)
            {
                DestroyImmediate(existingHallways.gameObject);
            }

            // Compute the bounds of the primary hallway, without the unwanted objects
            var primaryRenderers = gameObject.GetComponentsInChildren<Renderer>();
            var center = Vector3.zero;
            foreach (var renderer in primaryRenderers)
            {
                if (renderer.CompareTag("FilterRendererBounds"))
                {
                    continue;
                }
                
                center += renderer.bounds.center;
            }
            center /= primaryRenderers.Length;
            var primaryBounds = new Bounds(center, Vector3.zero);
            foreach (var renderer in primaryRenderers)
            {
                if (renderer.CompareTag("FilterRendererBounds"))
                {
                    continue;
                }
                
                primaryBounds.Encapsulate(renderer.bounds);
            }

            // Create the parent object for the secondary hallways and position it at the center of the primary hallway.
            var secondaryHallways = new GameObject("SecondaryHallways");
            secondaryHallways.transform.SetParent(transform);
            secondaryHallways.transform.localPosition = Vector3.zero;
            secondaryHallways.transform.localRotation = Quaternion.identity;
            secondaryHallways.transform.localScale = Vector3.one;
            secondaryHallways.isStatic = true;

            // Compute the bounds of the secondary hallways, without the unwanted objects
            var secondaryRenderers = primaryHallwayParts.SecondaryHallwayGenerator.GetComponentsInChildren<Renderer>();
            center = Vector3.zero;
            foreach (var renderer in secondaryRenderers)
            {
                if (renderer.CompareTag("FilterRendererBounds"))
                {
                    continue;
                }
                
                center += renderer.bounds.center;
            }
            center /= secondaryRenderers.Length;
            var secondaryBounds = new Bounds(center, Vector3.zero);
            foreach (var renderer in secondaryRenderers)
            {
                if (renderer.tag == "FilterRendererBounds")
                {
                    continue;
                }
                
                secondaryBounds.Encapsulate(renderer.bounds);
            }

            // Generate the west-side secondary hallways based on the scenes provided.
            for (int i = 0; i < westHallwayScenes.Count; i += 2)
            {
                // Compute the correct position for the secondary hallway in the current iteration and instantiate it.
                var position = transform.position + new Vector3(0, 0,
                    -primaryBounds.size.z / 2 - i / 2 * secondaryBounds.size.z - secondaryBounds.size.z / 2);
                var secondaryHallwayGenerator = Instantiate(primaryHallwayParts.SecondaryHallwayGenerator, position,
                    Quaternion.identity, secondaryHallways.transform);

                // Create a sublist of the scenes to be used for the current secondary hallway.
                var sceneList = new List<SceneReference>()
                {
                    westHallwayScenes[i]
                };
                if (i + 1 < westHallwayScenes.Count)
                {
                    sceneList.Add(westHallwayScenes[i + 1]);
                }

                secondaryHallwayGenerator.SetSecondaryHallwayConfig(true, i + 2 >= westHallwayScenes.Count, sceneList);
            }
            
            // Generate the east-side secondary hallways based on the scenes provided.
            for (int i = 0; i < eastHallwayScenes.Count; i += 2)
            {
                // Compute the correct position for the secondary hallway in the current iteration and instantiate it.
                var position = transform.position + new Vector3(0, 0,
                    primaryBounds.size.z / 2 + i / 2 * secondaryBounds.size.z + secondaryBounds.size.z / 2);
                var secondaryHallwayGenerator = Instantiate(primaryHallwayParts.SecondaryHallwayGenerator, position,
                    Quaternion.identity, secondaryHallways.transform);

                // Create a sublist of the scenes to be used for the current secondary hallway.
                var sceneList = new List<SceneReference>()
                {
                    eastHallwayScenes[i]
                };
                if (i + 1 < eastHallwayScenes.Count)
                {
                    sceneList.Add(eastHallwayScenes[i + 1]);
                }

                secondaryHallwayGenerator.SetSecondaryHallwayConfig(false, i + 2 >= eastHallwayScenes.Count, sceneList);
            }
        }

        // Auxiliary struct to hold the parts of the primary hallway.
        [System.Serializable]
        struct PrimaryHallwayParts
        {
            public GameObject groundFloor;
            public GameObject upperFloor;
            public GameObject northWall;
            public GameObject southWall;
            public GameObject doubleDoorExit;
            public GameObject westWallEmpty;
            public GameObject westWallRooms;
            public GameObject eastWallEmpty;
            public GameObject eastWallRooms;
            public GameObject stairs;
            public GameObject roofCeiling;
            public GameObject floorCeiling;
            public SecondaryHallwayGenerator SecondaryHallwayGenerator;
        }
    }
}
