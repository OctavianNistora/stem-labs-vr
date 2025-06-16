using System;
using Custom.Scripts.ExperimentGeneral;
using Unity.Netcode;
using UnityEngine;
namespace Custom.Scripts.ExperimentSpecific.ThermalExpansion
{
    // Script responsible for detecting in the thermal expansion experiment the collision of the ball with the ring or
    // the "ring collision fix" collider.
    public class ThermalExperimentBallCollisionScript : NetworkBehaviour
    {
        [SerializeField] private SphereCollider ballCollider;
        // Due to inaccuracies in the standard Unity physics engine, the ball collider is not always blocked by the ring
        // mesh collider. To fix this, we add a second collider that is enabled only when the ball is touching the ring
        // or the second collider, unless the ball is smaller than the ring, in which case the second collider is disabled.
        [SerializeField] private BoxCollider ringFixCollider;
        [SerializeField] private float ringRadius;
        [SerializeField] private float ringWidth;
        [SerializeField] private ClipboardHandler clipboardHandler;
        [SerializeField] int stepCompleted;
        
        private bool _rpcCalled = false;

        void Start()
        {
            ringFixCollider.enabled = false;
        }

        private void OnCollisionEnter(Collision collision)
        {
            DetermineCollisionOutcome(collision);
        }

        private void DetermineCollisionOutcome(Collision collision)
        {
            if (!IsOwner || !collision.gameObject.CompareTag("Ring"))
            {
                return;
            }
            
            var ballActualRadius = ballCollider.radius * ballCollider.transform.localScale.x;
            if (ballActualRadius <= ringRadius)
            {
                ringFixCollider.enabled = false;
                return;
            }
            
            // The distance from the center of the ball to the surface of the ring is computed using the Pythagorean
            // theorem.
            var distanceCentersBallCircle = Mathf.Sqrt(ballActualRadius*ballActualRadius - ringRadius*ringRadius);
            var distanceDifference = ballActualRadius - distanceCentersBallCircle;

            // In order to accurately place the ringFixCollider, we need to first add half of the ring width in the
            // direction of the ball collider, and then subtract in the opposite direction the difference between the
            // radius of the ball and distance between the centers of the ball and ring surface.
            if (ballCollider.gameObject.transform.position.y > ringFixCollider.transform.position.y)
            {
                ringFixCollider.center = new Vector3(0, 0, ringWidth/2 - distanceDifference);
            }
            else
            {
                ringFixCollider.center = new Vector3(0, 0, -ringWidth/2 + distanceDifference);
            }
            ringFixCollider.enabled = true;

            if (!_rpcCalled)
            {
                _rpcCalled = true;
                RingCollisionRpc();
            }
        }
        
        private void RingCollisionRpc()
        {
            clipboardHandler.CompleteExperimentStep(stepCompleted);
        }
    }
}
