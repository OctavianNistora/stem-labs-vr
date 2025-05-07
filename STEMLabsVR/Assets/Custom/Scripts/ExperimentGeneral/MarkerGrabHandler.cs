using UnityEngine;
namespace Custom.Scripts.ExperimentGeneral
{
    // Script to handle the marker cap and non-culled tip active states
    public class MarkerGrabHandler : MonoBehaviour
    {
        [SerializeField] private GameObject markerCap;
        [SerializeField] private GameObject markerNonCulledTip;
    
        public void OnGrab()
        {
            markerCap.SetActive(false);
            markerNonCulledTip.SetActive(true);
        }
    
        public void OnRelease()
        {
            markerCap.SetActive(true);
            markerNonCulledTip.SetActive(false);
        }
    }
}
