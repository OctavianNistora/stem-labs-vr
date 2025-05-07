using UnityEngine;
namespace Custom.Scripts.Helper
{
    // Singleton-like class to hold references to the XR Origin components
    [DisallowMultipleComponent]
    public class XROriginReferences : MonoBehaviour
    {
        public static XROriginReferences Instance;

        [SerializeField]
        private Transform rootTransform;
        [SerializeField]
        private Transform headTransform;
        [SerializeField]
        private Transform leftHandTransform;
        [SerializeField]
        private Transform rightHandTransform;
    
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
        }

        public Transform GetRootTransform()
        {
            return rootTransform;
        }
        
        public Transform GetHeadTransform()
        {
            return headTransform;
        }
        
        public Transform GetLeftHandTransform()
        {
            return leftHandTransform;
        }
        
        public Transform GetRightHandTransform()
        {
            return rightHandTransform;
        }
    }
}
