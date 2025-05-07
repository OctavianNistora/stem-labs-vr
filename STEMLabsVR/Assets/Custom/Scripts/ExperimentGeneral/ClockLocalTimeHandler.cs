using System;
using UnityEngine;
namespace Custom.Scripts.ExperimentGeneral
{
    // Script that handles the clock's hands rotation based on the current time
    public class ClockLocalTimeHandler : MonoBehaviour
    {
        public GameObject hourHand;
        public GameObject minuteHand;
        public GameObject secondHand;

        void Update()
        {
            // Extract the number of seconds, minutes, and hours from the current time, taking also into account the
            // smaller units of time (milliseconds for seconds, milliseconds and seconds for minutes) and calculate the
            // angles for each hand accordingly.
            DateTime currentTime = DateTime.Now;
            float second = currentTime.Second + currentTime.Millisecond / 1000f;
            float secondAngle = second * 6f;
            float minute = currentTime.Minute + second / 60f;
            float minuteAngle = minute * 6f;
            float hour = currentTime.Hour + minute / 60f;
            float hourAngle = hour * 30f;
        
            // Rotate the hands of the clock based on the calculated angles. The rotation is done in local space to
            // ensure that the hands rotate around their own axes no matter the clock's orientation in world space.
            secondHand.transform.localRotation = Quaternion.Euler(90 + secondAngle, 0, -90);
            minuteHand.transform.localRotation = Quaternion.Euler(90 + minuteAngle, 0, -90);
            hourHand.transform.localRotation = Quaternion.Euler(90 + hourAngle, 0, -90);
        }
    }
}
