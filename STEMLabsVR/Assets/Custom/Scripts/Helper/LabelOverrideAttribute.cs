using UnityEngine;
namespace Custom.Scripts.Helper
{
    // Property attribute to override the label of a field in the inspector
    public class LabelOverrideAttribute : PropertyAttribute
    {
        private readonly string _displayName;
        public LabelOverrideAttribute(string displayName) {
            _displayName = displayName;
        }
        
        public string GetDisplayName() {
            return _displayName;
        }
    }
}