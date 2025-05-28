using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

namespace Custom.Scripts.Helper
{
    [RequireComponent(typeof(TMP_InputField))]
    public class DisplayKeyboard : MonoBehaviour
    {
        [SerializeField]
        private Transform keyboardTransform;
        
        private TMP_InputField _inputField;
    
        void Start()
        {
            _inputField = GetComponent<TMP_InputField>();

            // Set the input field to display the keyboard
            _inputField.onSelect.AddListener(ShowKeyboard);
        }
    
        private void ShowKeyboard(string _)
        {
            NonNativeKeyboard.Instance.InputField = _inputField;
            NonNativeKeyboard.Instance.PresentKeyboard(_inputField.text);

            NonNativeKeyboard.Instance.RepositionKeyboard(keyboardTransform.position);
        }
    }
}
