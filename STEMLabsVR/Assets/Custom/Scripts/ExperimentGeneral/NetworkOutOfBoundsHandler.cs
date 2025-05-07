using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
namespace Custom.Scripts.ExperimentGeneral
{
    // Script used to move objects back to the initial position or to the closest point in the bounding box when they
    // are out of bounds.
    [RequireComponent(typeof(XRGrabInteractable))]
    public class NetworkOutOfBoundsHandler : NetworkBehaviour
    {
        [SerializeField]
        private bool returnToInitialPosition = true;

        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private SetupBoundingBox _boundingBoxScript;
    
        void Start()
        {
            if (returnToInitialPosition)
            {
                _initialPosition = transform.position;
                _initialRotation = transform.rotation;
            }
        
            // Retrieve closest SetupBoundingBox
            _boundingBoxScript = GetComponentInParent<SetupBoundingBox>();
            if (!_boundingBoxScript)
            {
                return;
            }
        
            // Add listener to the selectExited event of the XRGrabInteractable component
            var xrGrabInteractable = GetComponent<XRGrabInteractable>();
            if (xrGrabInteractable)
            {
                xrGrabInteractable.selectExited.AddListener(FixOutOfBounds);
            }
        }

        public override void OnDestroy()
        {
            var xrGrabInteractable = GetComponent<XRGrabInteractable>();
            if (xrGrabInteractable)
            {
                xrGrabInteractable.selectExited.RemoveListener(FixOutOfBounds);
            }
            
            base.OnDestroy();
        }

        private void FixOutOfBounds(SelectExitEventArgs args)
        {
            if (!IsOwner)
            {
                return;
            }
        
            // Check if the object is out of bounds
            var bounds = _boundingBoxScript.GetBounds();
            if (bounds.Contains(transform.position))
            {
                return;
            }
        
            // Move the object back to the initial position or to the closest point in the bounding box
            if (returnToInitialPosition)
            {
                transform.position = _initialPosition;
                transform.rotation = _initialRotation;
                return;
            }
            var closestPoint = bounds.ClosestPoint(transform.position);
            transform.position = closestPoint;
        }
    }
}
