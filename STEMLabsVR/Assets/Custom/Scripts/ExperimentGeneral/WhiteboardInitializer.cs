using UnityEngine;
namespace Custom.Scripts.ExperimentGeneral
{
    // Script that sets the initial color of the whiteboard to white
    public class WhiteboardInitializer : MonoBehaviour
    {
        void Start()
        {
            var texture = (RenderTexture)GetComponent<Renderer>().material.mainTexture;
        
            RenderTexture.active = texture;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = null;
        }
    }
}
