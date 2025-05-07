using System;
using UnityEngine;
namespace Custom.Scripts.ExperimentGeneral
{
    // This script is used to emit events when a collider enters the trigger area of the whiteboard.
    public class WhiteboardColliderEventEmitter : MonoBehaviour
    {
        public static WhiteboardColliderEventEmitter Instance { get; private set; }
        public event Action<Collider> OnTriggerEnterEvent;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
        }
    
        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterEvent?.Invoke(other);
        }
    }
}
