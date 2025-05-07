using Custom.Scripts.ExperimentGeneral;
using Unity.Netcode;
using UnityEngine;
namespace Custom.Scripts.ExperimentSpecific.ThermalExpansion
{
    // This script handles the thermal expansion of a ball object in Unity using a simplified physics model that is
    // relatively similar to the real-life version, and completes the appropriate experiment steps when the required
    // conditions are met.
    public class ThermalExpansionHandler : NetworkBehaviour
    {
        [SerializeField] private double ambientTemperature = 298.15;
        [SerializeField] private double initialTemperature = 293.15;
        [SerializeField] private double currentTemperature;
        [SerializeField] private double flameTemperature = 1273.15;
        [SerializeField] private double ambientConstant = 0.005;
        [SerializeField] private double flameConstant = 0.01;
        [SerializeField] private float distanceScaling = 5;
        [SerializeField] private double thermalExpansionCoefficient = 35.4e-4;
    
        [SerializeField] private ClipboardHandler clipboardHandler;
        [SerializeField] private float ringRadius;
        [SerializeField] private int proximityStepNumber;
        [SerializeField] private int expansionStepNumber;
    
        private GameObject _flameObject;
        private Renderer _ballRenderer;
        private float _ballRadius;

        private void Start()
        {
            currentTemperature = initialTemperature;
            _ballRenderer = gameObject.GetComponent<Renderer>();
            _ballRadius = gameObject.GetComponent<SphereCollider>().radius;
        }

        void Update()
        {
            // Only the owner of the object should update the temperature and scale of the ball
            if (!IsOwner)
            {
                return;
            }
            
            // Compute how fast the temperature increases per second
            double heatingRate = 0;
            if (_flameObject)
            {
                // Retrieve the distance between the flame and the outside of the ball
                Vector3 ballCenterPosition = _ballRenderer.bounds.center;
                float distanceToFlame = Vector3.Distance(ballCenterPosition, _flameObject.transform.position) -
                                        _ballRadius * gameObject.transform.localScale.x;
                
                // Check if the ball is close enough to the flame to trigger the experiment step
                if (distanceToFlame < 0.05)
                {
                    clipboardHandler.CompleteExperimentStep(proximityStepNumber);
                }

                // Compute the heating rate using a modified version of Newton's Law of Cooling
                heatingRate = flameConstant * Mathf.Exp(-distanceScaling * distanceToFlame) *
                              (flameTemperature - currentTemperature);
            }
        
            // Compute the cooling rate using a modified version of Newton's Law of Cooling
            double coolingRate = ambientConstant * (currentTemperature - ambientTemperature);

            // Update the current temperature of the ball
            double temperatureDifference = (heatingRate - coolingRate) * Time.deltaTime;
            currentTemperature += temperatureDifference;
        
            // Update the ball's scale using the Thermal expansion formula
            float volumeMultiplier = (float)(1 + thermalExpansionCoefficient * (currentTemperature - initialTemperature));
            float scaleFactor = Mathf.Pow(volumeMultiplier, 1.0f/3);
            gameObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        
            // If the ball has expanded enough, complete the corresponding experiment step
            if (_ballRadius * transform.localScale.x > ringRadius)
            {
                clipboardHandler.CompleteExperimentStep(expansionStepNumber);
            }
        }
    
        // This method sets the flame object that the ball is interacting with
        public void SetFlameObject(GameObject newFlame)
        {
            _flameObject = newFlame;
        }
    }
}