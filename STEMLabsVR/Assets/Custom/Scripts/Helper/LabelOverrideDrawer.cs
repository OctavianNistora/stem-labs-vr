#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Custom.Scripts.Helper
{
    // Custom property drawer for the LabelOverrideAttribute
    [CustomPropertyDrawer(typeof(LabelOverrideAttribute))]
    public class LabelOverrideDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (LabelOverrideAttribute)attribute;
            label.text = attr.GetDisplayName();
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}
#endif