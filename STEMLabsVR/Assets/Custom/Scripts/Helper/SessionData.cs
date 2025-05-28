using System;
using System.Collections.Generic;

namespace Custom.Scripts.Helper
{
    public class SessionData
    {
        private static string _inviteCode;
        public static string inviteCode
        {
            get => _inviteCode;
            set
            {
                if (_inviteCode != value)
                {
                    _inviteCode = value;
                    OnInviteCodeChanged?.Invoke(_inviteCode);
                }
            }
        }
        public static List<string> checklistSteps { get; set; }
        public static event Action<string?> OnInviteCodeChanged;
    }
}