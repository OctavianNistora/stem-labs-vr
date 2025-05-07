using UnityEngine;
namespace Custom.Scripts.Hallway
{
    // Script that is attached to the exit door
    public class CloseGameHandler : MonoBehaviour
    {
        public void CloseGame()
        {
            // Close the game if the game is built
            Application.Quit();

            // If we are running in the editor, stop playing the scene
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
