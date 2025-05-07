using Custom.Scripts.Hallway;
using UnityEditor;
using UnityEngine;
namespace Custom.Scripts.Editor
{
    // Class used to add a button to the inspector of the PrimaryHallwayGenerator script to rebuild the primary hallway.
    [CustomEditor(typeof(PrimaryHallwayGenerator))]
    class PrimaryHallwayGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            // Add a button to the inspector to rebuild the primary hallway
            if (GUILayout.Button("Rebuild Primary Hallway"))
            {
                var generator = (PrimaryHallwayGenerator)target;
                generator.RebuildPrimaryHallway();
            }
        }
    }
}