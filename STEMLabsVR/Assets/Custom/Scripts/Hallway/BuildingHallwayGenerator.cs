using System.Collections.Generic;
using Custom.Scripts.Hallway;
using Tymski;
using UnityEngine;
namespace Custom.Scripts
{
    // The script is responsible for generating in the Unity editor the hallways of an university building based on the
    // number of scenes added to the list and is configurable to have a maximum number of primary hallways per floor and
    // a maximum number of secondary hallways per primary hallway side.
    public class BuildingHallwayGenerator : MonoBehaviour
    {
        [SerializeField]
        private int numberOfPrimaryHallwaysPerFloor = 3;
        [SerializeField]
        private int numberOfSecondaryHallwaysPerPrimaryHallwaySide = 2;
        [SerializeField]
        private List<SceneReference> experimentScenes;
        [SerializeField]
        private Camera vrCamera;

        [SerializeField]
        private PrimaryHallwayGenerator primaryHallwayPrefab;

        const int RoomsPerSecondaryHallway = 2;

        // This method is called when the button in the inspector is clicked and is responsible for generating the
        // primary hallways and calling the method to generate the secondary hallways for each primary hallway.
        public void RebuildHallways()
        {
            // Check if there are any hallways already present in the scene and destroy them if they exist.
            var existingHallways = transform.Find("PrimaryHallways");
            if (existingHallways)
            {
                DestroyImmediate(existingHallways.gameObject);
            }

            // Create an empty GameObject to hold the primary hallways and make it easier to destroy them in case of a 
            // rebuild.
            var primaryHallways = new GameObject("PrimaryHallways");
            primaryHallways.transform.SetParent(transform);
            primaryHallways.transform.localPosition = Vector3.zero;
            primaryHallways.transform.localRotation = Quaternion.identity;
            primaryHallways.transform.localScale = Vector3.one;
            primaryHallways.isStatic = true;

            // Calculate the center of the primary hallway prefab bounding box
            var primaryRenderers = primaryHallwayPrefab.gameObject.GetComponentsInChildren<Renderer>();
            var primaryCenter = Vector3.zero;
            foreach (var renderer in primaryRenderers)
            {
                if (renderer.CompareTag("FilterRendererBounds"))
                {
                    continue;
                }

                primaryCenter += renderer.bounds.center;
            }
            primaryCenter /= primaryRenderers.Length;

            // Calculate the bounds of the primary hallway prefab to determine the size of the hallways
            var primaryBounds = new Bounds(primaryCenter, Vector3.zero);
            foreach (var renderer in primaryRenderers)
            {
                // Skip the filter renderer bounds as they are not needed for the calculation and can otherwise cause
                // issues generating the hallways.
                if (renderer.CompareTag("FilterRendererBounds"))
                {
                    continue;
                }

                primaryBounds.Encapsulate(renderer.bounds);
            }

            // Calculate the number of rooms per primary hallway and the number of rooms per floor based on the number of
            // primary hallways per floor and the number of secondary hallways per primary hallway side.
            var roomsPerPrimaryHallway = RoomsPerSecondaryHallway * numberOfSecondaryHallwaysPerPrimaryHallwaySide * 2;
            var roomsPerFloor = roomsPerPrimaryHallway * numberOfPrimaryHallwaysPerFloor;

            // Calculate the number of primary hallways required to fit all the scenes in the list and the number of floors
            // required to fit all the primary hallways.
            var primaryHallwaysCount = Mathf.CeilToInt((float)experimentScenes.Count / roomsPerPrimaryHallway);
            var floorCount = Mathf.CeilToInt((float)experimentScenes.Count / roomsPerFloor);

            // Calculate the number of rooms on the last floor
            var lastFloorRoomCount = experimentScenes.Count % roomsPerFloor;
            lastFloorRoomCount = lastFloorRoomCount == 0 ? roomsPerFloor : lastFloorRoomCount;

            // Generate the primary hallways and set their configuration
            for (int i = 0; i < floorCount; i++)
            {
                var floorRooms = i == floorCount - 1 ? lastFloorRoomCount : roomsPerFloor;
                var currentFloorPrimaryHallways = Mathf.CeilToInt((float)floorRooms / roomsPerPrimaryHallway);
                for (int j = 0; j < currentFloorPrimaryHallways; j++)
                {
                    // Calculate the position of the primary hallway to be generated and instantiate it.
                    var primaryPosition = transform.position +
                                          new Vector3(-primaryBounds.size.x * j, primaryBounds.size.y * i, 0);
                    var primaryHallway = Instantiate(primaryHallwayPrefab, primaryPosition, Quaternion.identity,
                        primaryHallways.transform);

                    // Determine the appropriate configuration for the primary hallway
                    var isFirstBuildingPrimaryHallway = i == 0 && j == 0;
                    var isLastFloorPrimaryHallway = j == currentFloorPrimaryHallways - 1;
                    var isFirstFloorPrimaryHallway = j == 0;
                    var isNotFirstFloor = i > 0;
                    var hasHoledCeiling = (i + 1) * numberOfPrimaryHallwaysPerFloor + j >= primaryHallwaysCount;

                    // Retrieve the scenes for the west side of the primary hallway
                    var firstWestSideSceneIndex = i * roomsPerFloor + j * roomsPerPrimaryHallway;
                    var westSideSceneCount = Mathf.Min(roomsPerPrimaryHallway / 2,
                        experimentScenes.Count - roomsPerFloor * i - j * roomsPerPrimaryHallway);
                    var westSideScenes = experimentScenes.GetRange(firstWestSideSceneIndex, westSideSceneCount);

                    // Retrieve the scenes for the east side of the primary hallway if there are any remaining scenes
                    var firstEastSideSceneIndex =
                        i * roomsPerFloor + j * roomsPerPrimaryHallway + roomsPerPrimaryHallway / 2;
                    List<SceneReference> eastSideScenes;
                    // Check if there are any remaining scenes for the east side of the primary hallway since west side
                    // scenes are always assigned first and a generated primary hallway is guaranteed to have at least
                    // one scene.
                    if (firstEastSideSceneIndex < experimentScenes.Count)
                    {
                        var eastSideSceneCount = Mathf.Min(roomsPerPrimaryHallway / 2,
                            experimentScenes.Count - roomsPerFloor * i - j * roomsPerPrimaryHallway -
                            westSideSceneCount);
                        eastSideScenes = experimentScenes.GetRange(firstEastSideSceneIndex, eastSideSceneCount);
                    }
                    else
                    {
                        eastSideScenes = new List<SceneReference>();
                    }

                    // Set the configuration for the primary hallway and generate the secondary hallways for each side
                    primaryHallway.SetHallwayConfig(isFirstBuildingPrimaryHallway, isLastFloorPrimaryHallway,
                        isFirstFloorPrimaryHallway, isNotFirstFloor, hasHoledCeiling, westSideScenes, eastSideScenes,
                        vrCamera);
                }
            }

            // Destroy any object that is not active in the hierarchy in order to clean up the scene and reduce the 
            // memory usage.
            foreach (Transform childTransform in transform.GetComponentsInChildren<Transform>(true))
            {
                if (childTransform && !childTransform.gameObject.activeInHierarchy)
                {
                    DestroyImmediate(childTransform.gameObject);
                }
            }
        }
    }
}
