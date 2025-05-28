using System.Collections.Generic;
using Custom.Scripts.ExperimentGeneral;
using UnityEngine;
namespace Custom.Scripts.Helper
{
    // This class is used to store the experiment description data.
    [CreateAssetMenu(fileName = "ExperimentDescriptionData", menuName = "ScriptableObjects/ExperimentDescriptionData")]
    class ExperimentData : ScriptableObject
    {
        [SerializeField]
        private string experimentTitle;
        [SerializeField]
        private int experimentTitleFontSize;
        [SerializeField]
        private List<string> experimentDescriptionText;
        [SerializeField]
        private int experimentDescriptionTextFontSize;
        [SerializeField]
        private List<Texture2D> experimentDescriptionTextures;
        [SerializeField]
        private List<ExperimentDescriptionType> experimentDescriptionTypesOrder;
        [SerializeField]
        private int experimentStepsFontSize;

        // Method that retrieves all the clipboard data for the experiment in the correct format.
        public ExperimentClipboardData ExperimentClipboardData()
        {
            var experimentClipboardData = new ExperimentClipboardData();
            experimentClipboardData.titleFontSize = experimentTitleFontSize;
            experimentClipboardData.contentFontSize = experimentDescriptionTextFontSize;
        
            experimentClipboardData.contents = new List<ClipboardExperimentDescriptionContent>();
            var experimentDescriptionContent = new ClipboardExperimentDescriptionContent();
            experimentDescriptionContent.title = experimentTitle;
            experimentClipboardData.contents.Add(experimentDescriptionContent);
        
            var indexArray = new int[System.Enum.GetValues(typeof(ExperimentDescriptionType)).Length];
            foreach (var type in experimentDescriptionTypesOrder)
            {
                AddContent(experimentClipboardData.contents, type, indexArray);
            }
        
            foreach (var type in System.Enum.GetValues(typeof(ExperimentDescriptionType)))
            {
                var count = ElementTypeCount((ExperimentDescriptionType)type);
                while (indexArray[(int)type] < count)
                {
                    AddContent(experimentClipboardData.contents, (ExperimentDescriptionType)type, indexArray);
                }
            }
        
            experimentClipboardData.stepsFontSize = experimentDescriptionTextFontSize;
        
            return experimentClipboardData;
        }

        private void AddContent(List<ClipboardExperimentDescriptionContent> contents,
            ExperimentDescriptionType type, int[] currentIndexArray)
        {
            var experimentDescriptionContent = new ClipboardExperimentDescriptionContent();
            switch (type)
            {
                case ExperimentDescriptionType.Text:
                    if (currentIndexArray[(int)type] >= experimentDescriptionText.Count)
                    {
                        return;
                    }
                    experimentDescriptionContent.text = experimentDescriptionText[currentIndexArray[(int)type]];
                    currentIndexArray[(int)type]++;
                    break;
                case ExperimentDescriptionType.Image:
                    if (currentIndexArray[(int)type] >= experimentDescriptionTextures.Count)
                    {
                        return;
                    }
                    experimentDescriptionContent.texture = experimentDescriptionTextures[currentIndexArray[(int)type]];
                    currentIndexArray[(int)type]++;
                    break;
            }
            contents.Add(experimentDescriptionContent);
        }
    
        private int ElementTypeCount(ExperimentDescriptionType type)
        {
            switch (type)
            {
                case ExperimentDescriptionType.Text:
                    return experimentDescriptionText.Count;
                case ExperimentDescriptionType.Image:
                    return experimentDescriptionTextures.Count;
                default:
                    return 0;
            }
        }
    }

    internal enum ExperimentDescriptionType
    {
        Text,
        Image
    }
}