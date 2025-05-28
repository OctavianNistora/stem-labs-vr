using Custom.Scripts.Helper;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Custom.Scripts.ExperimentGeneral
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class InviteCodeDisplay : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshPro;

        private void Awake()
        {
            _textMeshPro = GetComponent<TextMeshProUGUI>();
        
            SessionData.OnInviteCodeChanged += UpdateInviteCodeDisplay;
            UpdateInviteCodeDisplay(SessionData.inviteCode);
        }

        private void UpdateInviteCodeDisplay([CanBeNull] string inviteCode)
        {
            if (string.IsNullOrEmpty(inviteCode))
            {
                return;
            }
            _textMeshPro.text = $"Invite Code: {inviteCode}";
        }
    }
}
