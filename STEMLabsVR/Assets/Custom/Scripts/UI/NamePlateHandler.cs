using Custom.Scripts.Data.Static;
using Custom.Scripts.Helper;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Custom.Scripts.UI
{
    // This script handles the name plate display for players in the game.
    public class NamePlateHandler : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI namePlateText;
        [SerializeField] private Transform headTransform;
        [SerializeField] private int namePlateHeight;
    
        private readonly NetworkVariable<FixedString128Bytes> _networkName = new("", NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

        // This method is called when the network object is spawned, and it initializes the name plate text and sets up the name change listener.
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
        
            _networkName.OnValueChanged += ChaneNamePlateText;
            ChaneNamePlateText("", _networkName.Value);

            if (!IsOwner)
            {
                return;
            }

            _networkName.Value = AuthData.fullName;
        }

        // This method is called every frame after all Update methods have been called, and it is used to update the position and orientation of the name plate.
        private void LateUpdate()
        {
            if (headTransform)
            {
                transform.position = new Vector3(headTransform.position.x, headTransform.position.y + namePlateHeight, headTransform.position.z);
            }
            if (Camera.main)
            {
                transform.LookAt(transform.position + Camera.main.transform.forward);
            }
        }

        private void ChaneNamePlateText(FixedString128Bytes oldName, FixedString128Bytes newName)
        {
            if (namePlateText)
            {
                namePlateText.text = newName.ToString();
            }
        }
    }
}
