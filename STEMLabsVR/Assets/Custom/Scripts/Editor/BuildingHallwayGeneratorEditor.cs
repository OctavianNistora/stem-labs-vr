using UnityEditor;
using UnityEngine;
namespace Custom.Scripts.Editor
{
    // Class used to add a button to the inspector of the BuildingHallwayGenerator script to rebuild the hallways.
    [CustomEditor(typeof(BuildingHallwayGenerator))]
    class BuildingHallwayGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // Add a button to the inspector to rebuild the hallways
            if (GUILayout.Button("Rebuild Building Hallways"))
            {
                var generator = (BuildingHallwayGenerator)target;
                generator.RebuildHallways();
            }
        }
    }
}