using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Custom.Scripts.Helper
{
    // This class is responsible for managing hand animations using an Animator component.
    public class HandAnimationManager : MonoBehaviour
    {
        private static readonly int Trigger = Animator.StringToHash("Trigger");
        private static readonly int Grip = Animator.StringToHash("Grip");
        public Animator handAnimator;
        public InputActionProperty trigger;
        public InputActionProperty grip;

        private void Start()
        {
            trigger.action.performed += SetTrigger;
            trigger.action.canceled += SetTrigger;
        
            grip.action.performed += SetGrip;
            grip.action.canceled += SetGrip;
        }
        
        private void OnDestroy()
        {
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
