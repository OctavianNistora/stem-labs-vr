using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
namespace Custom.Scripts.ExperimentGeneral
{
    // Script that sets the initial color of the whiteboard to white
    public class WhiteboardInitializer : MonoBehaviour
    {
        private RenderTexture _whiteboardTexture;
        
        void Start()
        {
            StartCoroutine(InitializeWhiteboard());
        }

        private IEnumerator InitializeWhiteboard()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            
            if (GetComponent<Renderer>() == null || GetComponent<Renderer>().material == null)
            {
                Debug.LogError("WhiteboardInitializer: Renderer or material is missing.");
                yield break;
            }

            if (GetComponent<Renderer>().material.mainTexture == null)
            {
                Debug.LogError("WhiteboardInitializer: Main texture is missing.");
                yield break;
            }

            _whiteboardTexture = (RenderTexture)GetComponent<Renderer>().material.mainTexture;
            
            RenderTexture.active = _whiteboardTexture;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = null;
        }
    }
}
