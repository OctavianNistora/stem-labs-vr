using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace Custom.Scripts.Helper
{
    // This script is responsible for updating the player character network transforms using the owner's player character
    // local transforms.
    public class NetworkPlayerSync : NetworkBehaviour
    {
        [SerializeField]
        private Transform networkRootTransform;
        [SerializeField]
        private Transform networkHeadTransform;
        [SerializeField]
        private Transform networkLeftHandTransform;
        [SerializeField]
        private Transform networkRightHandTransform;
        [SerializeField]
        private List<Renderer> networkPlayerRenderers;
    
        private void Start()
        {
            // Disable renderers of the network character for the owner since they are duplicated
            if (!IsOwner)
            {
                return;
            }
            
            foreach (Renderer renderer in networkPlayerRenderers)
            {
                renderer.enabled = false;
            }
        }

        private void Update()
        {
            // Only update the transforms of the network character if the local player is the owner
            if (!IsOwner)
            {
                return;
            }
        
            var localRootTransform = XROriginReferences.Instance.GetRootTransform();
            networkRootTransform.position = localRootTransform.position;
            networkRootTransform.rotation = localRootTransform.rotation;
        
            var localHeadTransform = XROriginReferences.Instance.GetHeadTransform();
            networkHeadTransform.position = localHeadTransform.position;
            networkHeadTransform.rotation = localHeadTransform.rotation;
        
            var localLeftHandTransform = XROriginReferences.Instance.GetLeftHandTransform();
            networkLeftHandTransform.position = localLeftHandTransform.position;
            networkLeftHandTransform.rotation = localLeftHandTransform.rotation;
        
            var localRightHandTransform = XROriginReferences.Instance.GetRightHandTransform();
            networkRightHandTransform.position = localRightHandTransform.position;
            networkRightHandTransform.rotation = localRightHandTransform.rotation;
        }
    }
}
