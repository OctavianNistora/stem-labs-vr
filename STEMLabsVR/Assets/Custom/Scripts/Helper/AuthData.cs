using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Custom.Scripts.Helper
{
    public class AuthData
    {
        private static int? _id;

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
        public static string accessToken { get; set; }
        public static string fullName { get; set; }
        public static string role { get; set; }
        public static event Action<int?> OnIdChanged;
    }
}
