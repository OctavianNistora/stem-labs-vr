using System.Collections.Generic;
using UnityEngine;
namespace Custom.Scripts.ExperimentGeneral
{
    // This class is used to store the clipboard data for the experiment description.
    public class ExperimentClipboardData
    {
        public int titleFontSize { get; set; }
        public int contentFontSize { get; set; }
        public List<ClipboardExperimentDescriptionContent> contents { get; set; }
        public int stepsFontSize { get; set; }
        public List<string> steps { get; set; }
    }

    // This class is used to store the contents of the clipboard for the experiment description, while keeping only one
    // property active at a time.
    public class ClipboardExperimentDescriptionContent
    {
        private string _title;
        private string _text;
        private Texture2D _texture;
    
        public string title
        {
            get => _title;
            set
            {
                ClearAllProperties();
                _title = value;
            }
        }

        public string text
        {
            get => _text;
            set
            {
                ClearAllProperties();
                _text = value;
            }
        }

        public Texture2D texture
        {
            get => _texture;
            set
            {
                ClearAllProperties();
                _texture = value;
            }
        }
    
        private void ClearAllProperties()
        {
            _title = null;
            _text = null;
            _texture = null;
        }
    }
}