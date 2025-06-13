using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
namespace Custom.Scripts.Helper
{
    // Script used to prevent other players than the owner from interacting with the grabable object
    [RequireComponent(typeof(XRBaseInteractable))]
    public class NetworkXRInteractableConstraint : NetworkBehaviour, IXRHoverFilter, IXRSelectFilter
    {
        public bool canProcess => isActiveAndEnabled;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        
            var interactable = GetComponent<XRBaseInteractable>();
            if (interactable)
            {
                interactable.selectFilters.Add(this);
                interactable.hoverFilters.Add(this);
            }
        }
    
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        
            var interactable = GetComponent<XRBaseInteractable>();
            if (interactable)
            {
                interactable.selectFilters.Remove(this);
                interactable.hoverFilters.Remove(this);
            }
        }

        // Check if the select interaction should be processed depending on the ownership of the object
        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            return IsOwner;
        }
        
        // Check if the hover interaction should be processed depending on the ownership of the object
        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            return IsOwner;
        }
    }
}
