using Unity.Netcode.Components;
using UnityEngine;
namespace Custom.Scripts.Helper
{
    // This script is used to disable the server authority for the NetworkAnimator component
    [DisallowMultipleComponent]
    public class NetworkClientTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
