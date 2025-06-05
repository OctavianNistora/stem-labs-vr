using System;

namespace Custom.Scripts.Data.Static
{
    // This class contains static authentication data for the application.
    public static class AuthData
    {
        private static int? _id;

        public static bool isGuest { get; set; } = true;
        public static int? id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnIdChanged?.Invoke(_id);
                }
            }
        }
        public static string accessToken { get; set; } = string.Empty;
        public static string fullName { get; set; } = string.Empty;
        public static string role { get; set; } = string.Empty;
        public static event Action<int?> OnIdChanged;
    }
}
