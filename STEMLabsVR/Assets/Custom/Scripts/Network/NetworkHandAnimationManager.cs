using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Custom.Scripts.Helper
{
    // This class is responsible for managing hand animations using an Animator component in a networked environment.
    public class NetworkHandAnimationManager : NetworkBehaviour
    {
        private static readonly int Trigger = Animator.StringToHash("Trigger");
        private static readonly int Grip = Animator.StringToHash("Grip");
        public Animator handAnimator;
        public InputActionProperty trigger;
        public InputActionProperty grip;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (!IsOwner)
            {
                return;
            }
            
            trigger.action.performed += SetTrigger;
            trigger.action.canceled += SetTrigger;
        
            grip.action.performed += SetGrip;
            grip.action.canceled += SetGrip;
        }
        
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            
            if (!IsOwner)
            {
                return;
            }
            
            trigger.action.performed -= SetTrigger;
            trigger.action.canceled -= SetTrigger;
        
            grip.action.performed -= SetGrip;
            grip.action.canceled -= SetGrip;
        }
    
        private void SetTrigger(InputAction.CallbackContext context)
        {
            handAnimator.SetFloat(Trigger, context.ReadValue<float>());
        }
    
        private void SetGrip(InputAction.CallbackContext context)
        {
            handAnimator.SetFloat(Grip, context.ReadValue<float>());
        }
    }
}
