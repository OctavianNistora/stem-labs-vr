using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

namespace Custom.Scripts.UI
{
    // This script is used to display a custom keyboard when the TMP_InputField is selected
    [RequireComponent(typeof(TMP_InputField))]
    public class DisplayKeyboard : MonoBehaviour
    {
        [SerializeField]
        private Transform keyboardTransform;
        
        private TMP_InputField _inputField;
    
        void Start()
        {
            _inputField = GetComponent<TMP_InputField>();

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
