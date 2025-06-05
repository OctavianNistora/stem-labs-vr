using Custom.Scripts.Data.Static;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using Event = Unity.Services.Analytics.Event;

namespace Custom.Scripts.Helper
{
    // This script handles the scene load event and records analytics data when a laboratory room is entered.
    public class SceneLoadEventHandler : MonoBehaviour
    {
        async void Start()
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                AnalyticsService.Instance.RecordEvent(new LaboratoryRoomEntered()
                {
                    experimentName = SceneManager.GetActiveScene().name,
                    isGuestMode = AuthData.isGuest
                });
            }
        }
    }

    // Auxiliary class to record the laboratory room entered event.
    class LaboratoryRoomEntered : Event
    {
        public LaboratoryRoomEntered() : base("laboratoryRoomEntered")
        {
        }

        public string experimentName { set => SetParameter("experimentName", value); }
        public bool isGuestMode { set => SetParameter("isGuestMode", value); }
    }
}