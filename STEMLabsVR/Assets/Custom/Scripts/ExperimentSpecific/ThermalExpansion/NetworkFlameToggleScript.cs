using Custom.Scripts.ExperimentGeneral;
using Unity.Netcode;
using UnityEngine;
namespace Custom.Scripts.ExperimentSpecific.ThermalExpansion
{
    // Script that synchronizes the flame particle system across the network and completes the corresponding experiment
    // step when the flame is toggled on.
    public class NetworkFlameToggleScript : NetworkBehaviour
    {
        [SerializeField] private ParticleSystem flameParticleSystem;
        [SerializeField] private ThermalExpansionHandler ballThermalExpansionHandler;
        [SerializeField] private ClipboardHandler clipboardHandler;
        [SerializeField] int stepCompleted;

        NetworkVariable<bool> _isFlameActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            _isFlameActive.OnValueChanged += OnFlameStateChanged;
            
            if (IsOwner)
            {
                _isFlameActive.Value = false;
            }

            if (_isFlameActive.Value)
            {
                flameParticleSystem.Play();
            }
            else
            {
                flameParticleSystem.Stop();
            }
        }
        
        // Method used to update the flame state every time the network variable changes.
        private void OnFlameStateChanged(bool previousValue, bool newValue)
        {
            if (newValue)
            {
                flameParticleSystem.Play();
                ballThermalExpansionHandler.SetFlameObject(flameParticleSystem.gameObject);
                
                if (!IsOwner)
                {
                    // Only the owner of the setup needs to complete the experiment step
                    return;
                }
                clipboardHandler.CompleteExperimentStep(stepCompleted);
            }
            else
            {
                flameParticleSystem.Stop();
                ballThermalExpansionHandler.SetFlameObject(null);
            }
        }
        
        // Method used to toggle the flame state. It is called when the owner player activates the canister.
        public void ToggleFlame()
        {
            if (IsOwner)
            {
                _isFlameActive.Value = !_isFlameActive.Value;
            }
        }
    }
}
