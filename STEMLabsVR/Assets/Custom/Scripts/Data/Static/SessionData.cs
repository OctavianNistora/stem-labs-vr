using System;
using System.Collections.Generic;

namespace Custom.Scripts.Data.Static
{
    // This class contains static session data for the application.
    public static class SessionData
    {
        private static string _inviteCode = string.Empty;
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
        public static event Action<string?> OnInviteCodeChanged;
        public static bool isClientHost { get; set; } = true;
        public static List<string> checklistSteps { get; set; } = new();
    }
}