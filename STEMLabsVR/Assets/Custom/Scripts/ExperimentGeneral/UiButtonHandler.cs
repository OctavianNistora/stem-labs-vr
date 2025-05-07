using UnityEngine;
using UnityEngine.UI;
namespace Custom.Scripts.ExperimentGeneral
{
    // This class is used to handle the Next button click event in the UI.
    class UiButtonHandler : MonoBehaviour
    {
        [SerializeField] private ClipboardHandler clipboardHandler;

        private Image _backgroundImage;

        private void Start()
        {
            _backgroundImage = GetComponent<Image>();
        }

        public void OnHoverEnter()
        {
            _backgroundImage.enabled = true;
        }

        public void OnHoverExit()
        {
            _backgroundImage.enabled = false;
        }
        
        public void OnClickNext()
        {
            clipboardHandler.NextPage();
        }
        
        public void OnClickPrevious()
        {
            clipboardHandler.PreviousPage();
        }
    }
}
