using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Custom.Scripts.Data.ScriptableObjects;
using Custom.Scripts.Data.Static;
using Custom.Scripts.Helper;
using Custom.Scripts.UI;
using TMPro;
using Unity.Netcode;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
namespace Custom.Scripts.ExperimentGeneral
{
    // This class handles the clipboard functionality for the experiments.
    public class ClipboardHandler : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI contentText;
        [SerializeField] private RawImage templateImage;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;

        [SerializeField] private float maximumImageRelativeHeight = 0.2f;
        [LabelOverride("enableAutomaticParagraphIndentation")] [SerializeField]
        private bool isAutomaticParagraphIndentationEnabled;

        [FormerlySerializedAs("experimentDescriptionData")]
        [SerializeField] private ExperimentData experimentData;

        private List<Page> _pages;

        private readonly NetworkVariable<int> _currentPage = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
        // String that gets added to the beginning of each paragraph if automatic paragraph indentation is enabled
        private readonly string _paragraphPrefix = "    ";
        private List<Page> _checkListPages;
        private bool[] _stepsCompletedArray;
        private DateTime _startTime;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // Subscribe to the page change event on all clients and the server
            _currentPage.OnValueChanged += HandlePageChange;
            
            // Retrieve only the clipboard data from the experiment data and build the pages
            var clipboardInput = experimentData.ExperimentClipboardData();
            clipboardInput.steps = SessionData.checklistSteps.Count > 0
                ? SessionData.checklistSteps
                : clipboardInput.steps;
            clipboardInput.steps.Add("Write down any observations made during the experiment on the whiteboard.");
            BuildPages(clipboardInput);
            
            // Disable all UI elements and then re-enable only the display UI elements required for the initial page
            DisableAllUiElements();
            EnablePageUiDisplayElements(_currentPage.Value);
            
            // Enable the buttons for the initial page only if the client is the owner of the clipboard and subscribe to
            // the trigger event of the whiteboard collider
            if (!IsOwner)
            {
                return;
            }
            SetButtonsActiveState(_currentPage.Value);
            _stepsCompletedArray = new bool[clipboardInput.steps.Count];
            WhiteboardColliderEventEmitter.Instance.OnTriggerEnterEvent += HandleTrigger;
            var doorHandlers = FindObjectsOfType<DoorHandler>();
            foreach (var doorHandler in doorHandlers)
            {
                doorHandler.SetCliboardHandler(this);
            }
            
            _startTime = DateTime.Now;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        
            // Unsubscribe from the trigger event of the whiteboard collider only if the client is the owner of the
            // clipboard
            if (!IsOwner)
            {
                return;
            }
            WhiteboardColliderEventEmitter.Instance.OnTriggerEnterEvent -= HandleTrigger;

            // Record the amount of time spent in the laboratory room and the number of steps completed for analytics
            AnalyticsService.Instance.RecordEvent(new LaboratoryRoomExited()
            {
                minutesSpent = (int)(DateTime.Now - _startTime).TotalMinutes,
                stepsCompleted = GetStepsCompleted().Count
            });
        }

        // Method use to get to the next page, unless the current page is the last page
        public void NextPage()
        {
            if (_currentPage.Value < _pages.Count - 1)
            {
                _currentPage.Value++;
            }
        }

        // Method use to get to the previous page, unless the current page is the first page
        public void PreviousPage()
        {
            if (_currentPage.Value > 0)
            {
                _currentPage.Value--;
            }
        }
        
        // Method use to manage the UI elements that need to be enabled or disabled when the page changes
        private void HandlePageChange(int previousPage, int currentPage)
        {
            DisablePageUiDisplayElements(previousPage);
            EnablePageUiDisplayElements(currentPage);

            if (IsOwner)
            {
                SetButtonsActiveState(currentPage);
            }
        }

        // Method used to disable all UI display elements of the clipboard
        private void DisableAllUiElements()
        {
            previousButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            
            titleText.gameObject.SetActive(false);
            contentText.gameObject.SetActive(false);

            foreach (var imageGameObjects in _pages[_currentPage.Value].images)
            {
                imageGameObjects.SetActive(false);
            }
        }

        // Method used to disable the UI display elements of the clipboard that are part of the current page
        private void DisablePageUiDisplayElements(int pageIndex)
        {
            if (_pages[pageIndex].title != null)
            {
                titleText.gameObject.SetActive(false);
                return;
            }

            if (_pages[pageIndex].text != null)
            {
                contentText.gameObject.SetActive(false);
            }

            foreach (var imageGameObjects in _pages[pageIndex].images)
            {
                imageGameObjects.SetActive(false);
            }
        }

        // Method used to enable the UI display elements of the clipboard that are part of the current page
        private void EnablePageUiDisplayElements(int pageIndex)
        {
            // If the title text exists for the page set the text and font for the title UI text element, enable it and
            // return early since no other UI elements need to be enabled for title pages
            if (_pages[pageIndex].title != null)
            {
                titleText.text = _pages[pageIndex].title;
                titleText.fontSize = _pages[pageIndex].fontSize;

                titleText.gameObject.SetActive(true);

                return;
            }

            // If the page contains text, set the text and font for the content UI text element and enable it
            if (_pages[pageIndex].text != null)
            {
                contentText.text = _pages[pageIndex].text;
                contentText.fontSize = _pages[pageIndex].fontSize;

                contentText.gameObject.SetActive(true);
            }

            // Enable all the images associated with the page
            foreach (var image in _pages[pageIndex].images)
            {
                image.SetActive(true);
            }
        }

        // Method used to build the pages of the clipboard using the experiment clipboard data
        private void BuildPages(ExperimentClipboardData experimentClipboardData)
        {
            // Compute the aspect ratio of the page, maximum allowed image height and height of each line of text for
            // the pages with the content of the experiment description
            var pageAspectRatio = contentText.rectTransform.rect.width / contentText.rectTransform.rect.height;
            var maximumImageHeight = contentText.rectTransform.rect.height * maximumImageRelativeHeight;
            var lineActualHeight = contentText.font.faceInfo.lineHeight *
                experimentClipboardData.contentFontSize / contentText.font.faceInfo.pointSize;

            _pages = new List<Page>();
            var page = new Page();
            _pages.Add(page);

            // Temporarily enable the content text to facilitate with computation of lines of text taken
            contentText.gameObject.SetActive(true);
            
            
            foreach (var element in experimentClipboardData.contents)
            {
                if (element.title != null)
                {
                    AddTitle(ref page, element.title, experimentClipboardData.titleFontSize);
                }
                else if (element.text != null)
                {
                    AddText(ref page, element.text, experimentClipboardData.contentFontSize);
                }
                else if (element.texture)
                {
                    AddImage(ref page, pageAspectRatio, element.texture, maximumImageHeight, lineActualHeight);
                }
            }

            _checkListPages = new List<Page>();
            // Create a new page if the last page has any content
            if (page.title != null || page.text != null || page.images.Count > 0)
            {
                page = new Page();
                _pages.Add(page);
                _checkListPages.Add(page);
            }

            // Add the experiment steps checklist as the last page(s) of the clipboard
            AddCheckList(ref page, experimentClipboardData.steps,
                experimentClipboardData.stepsFontSize);

            contentText.gameObject.SetActive(false);
        }

        // Method used to add to the page (or create a new page if the current page is not empty) the title of the
        // experiment and the correct font size
        private void AddTitle(ref Page page, string title, int fontSize)
        {
            if (page.title != null || page.text != null || page.images.Count > 0)
            {
                page = new Page();
                _pages.Add(page);
            }

            page.title = title;
            page.fontSize = fontSize;

            page = new Page();
            _pages.Add(page);
        }

        // Method used to add to the page (or create a new page if the current page is not empty) the text of the
        // experiment and the correct font size, while also adding any additional pages if the text is too long to fit
        // in the current page and automatically adding the correct paragraph indentation if the option is enabled
        private void AddText(ref Page page, string text, int fontSize)
        {
            if (page.text != null)
            {
                page.text += $"\n";
            }

            if (isAutomaticParagraphIndentationEnabled)
            {
                page.text += _paragraphPrefix;
            }

            page.text += text;
            page.fontSize = fontSize;

            // If the option is enabled, add the paragraph prefix to the beginning of each paragraph
            if (isAutomaticParagraphIndentationEnabled)
            {
                var offset = 0;
                foreach (Match match in Regex.Matches(page.text, @"\n[^\n\s]", RegexOptions.None))
                {
                    page.text = page.text.Insert(match.Index + offset + 1, _paragraphPrefix);
                    offset += _paragraphPrefix.Length;
                }
            }

            UpdateContentText(page);
            while (contentText.isTextTruncated)
            {
                var firstTMPCharTruncatedIndex = contentText.firstOverflowCharacterIndex;
                for (int j = 0; j < contentText.textInfo.wordCount; j++)
                {
                    if (contentText.textInfo.wordInfo[j].lastCharacterIndex < firstTMPCharTruncatedIndex)
                    {
                        continue;
                    }

                    if (contentText.textInfo.wordInfo[j].firstCharacterIndex > firstTMPCharTruncatedIndex)
                    {
                        break;
                    }

                    firstTMPCharTruncatedIndex = contentText.textInfo.wordInfo[j].firstCharacterIndex;
                }

                page.text = contentText.text.Substring(0, firstTMPCharTruncatedIndex);

                page = new Page();
                _pages.Add(page);
                page.text = contentText.text.Substring(firstTMPCharTruncatedIndex);
                page.fontSize = fontSize;

                UpdateContentText(page);
            }
        }

        private void AddImage(ref Page page, float pageAspectRatio, Texture2D texture, float maximumImageHeight,
            float lineActualHeight)
        {
            var imageAspectRatio = (float)texture.width / texture.height;
            var imageDisplayHeight = imageAspectRatio < pageAspectRatio
                ? contentText.rectTransform.rect.height
                : contentText.rectTransform.rect.width / imageAspectRatio;
            if (maximumImageRelativeHeight is > 0 and <= 1)
            {
                imageDisplayHeight = Mathf.Min(imageDisplayHeight, maximumImageHeight);
            }

            var heightConsumed = lineActualHeight * contentText.textInfo.lineCount;
            if (contentText.rectTransform.rect.height - heightConsumed < imageDisplayHeight)
            {
                page = new Page();
                _pages.Add(page);

                heightConsumed = 0;
            }

            var textLinesTaken = (int)Mathf.Ceil(imageDisplayHeight / lineActualHeight);
            page.text += new string('\n', textLinesTaken);

            var duplicatedImageGameObject =
                Instantiate(templateImage.gameObject, templateImage.rectTransform.parent);
            duplicatedImageGameObject.SetActive(false);
            page.images.Add(duplicatedImageGameObject);

            var duplicatedRawImageComponent = duplicatedImageGameObject.GetComponent<RawImage>();
            duplicatedRawImageComponent.rectTransform.anchoredPosition = new Vector2(0,
                contentText.rectTransform.rect.height / 2 - heightConsumed - imageDisplayHeight / 2);
            duplicatedRawImageComponent.rectTransform.sizeDelta =
                new Vector2(imageDisplayHeight * imageAspectRatio, imageDisplayHeight);
            duplicatedRawImageComponent.texture = texture;
        }

        private void AddCheckList(ref Page page, List<string> experimentStepsList, int experimentStepsListPageFontSize)
        {
            page.text = "Experiment Steps Checklist:\n";
            page.fontSize = experimentStepsListPageFontSize;

            foreach (var step in experimentStepsList)
            {
                page.text += $"Step {experimentStepsList.IndexOf(step) + 1}: {step}\n";
            }
            
            UpdateContentText(page);

            while (contentText.isTextTruncated)
            {
                var firstPartiallyVisibleStepIndex = contentText.text.LastIndexOf("\n",
                    contentText.firstOverflowCharacterIndex, StringComparison.Ordinal) + 1;

                page.text = contentText.text.Substring(0, firstPartiallyVisibleStepIndex);

                page = new Page();
                _pages.Add(page);
                _checkListPages.Add(page);
                page.text = contentText.text.Substring(firstPartiallyVisibleStepIndex);

                UpdateContentText(page);
            }
        }

        // Set the text for the content text element, update the font size if needed and force the UI text to update
        private void UpdateContentText(Page page)
        {
            contentText.text = page.text;
            contentText.fontSize = page.fontSize;
            contentText.ForceMeshUpdate(true, true);
        }

        // Method used to complete the experiment step by marking it as completed in the checklist and updating the
        // corresponding UI text element, unless the step has already been completed
        public void CompleteExperimentStep(int stepSelected)
        {
            var stepIndex = stepSelected - 1;
            if (_stepsCompletedArray[stepIndex])
            {
                return;
            }
            
            if (stepIndex < 0 || stepIndex >= _stepsCompletedArray.Length)
            {
                Debug.LogError($"Invalid step index {stepIndex}");
                return;
            }

            foreach (var checkListPage in _checkListPages)
            {
                var match = Regex.Match(checkListPage.text, $@"Step {stepIndex + 1}: [^\n]*\n");
                if (match.Success)
                {
                    var textToReplace = "<mark=#00ff0080>" + match.Value.Substring(0, match.Value.Length - 1) +
                                        "</mark>\n";
                    checkListPage.text = checkListPage.text.Replace(match.Value, textToReplace);

                    if (_pages[_currentPage.Value] == checkListPage)
                    {
                        contentText.text = checkListPage.text;
                        contentText.ForceMeshUpdate(true, true);
                    }

                    _stepsCompletedArray[stepIndex] = true;
                    break;
                }
            }
        }

        // Method used to set the active state of the previous and next buttons based on the page index
        private void SetButtonsActiveState(int pageIndex)
        {
            previousButton.gameObject.SetActive(pageIndex > 0);
            nextButton.gameObject.SetActive(pageIndex < _pages.Count - 1);
        }
        
        // Method used to handle the trigger event of the whiteboard collider and complete the last experiment step
        // which requires the user to write/draw on the whiteboard any observations made during the experiment
        private void HandleTrigger(Collider other)
        {
            CompleteExperimentStep(_stepsCompletedArray.Length);
        }
        
        public List<int> GetStepsCompleted()
        {
            var stepsCompleted = new List<int>();
            for (int i = 0; i < _stepsCompletedArray.Length - 1; i++)
            {
                if (_stepsCompletedArray[i])
                {
                    stepsCompleted.Add(i + 1);
                }
            }
            return stepsCompleted;
        }
        
        public bool AreObservationsMade()
        {
            return _stepsCompletedArray.Length > 0 && _stepsCompletedArray[_stepsCompletedArray.Length - 1];
        }
    }

    // Auxiliary class used to store the clipboard data for each page
    internal class Page
    {
        public int fontSize { get; set; }
        public String title { get; set; }
        public String text { get; set; }
        public List<GameObject> images { get; set; } = new();
    }

    internal class LaboratoryRoomExited : Unity.Services.Analytics.Event
    {
        public LaboratoryRoomExited() : base("laboratoryRoomExited")
        {
        }

        public int minutesSpent { set => SetParameter("timeSpent", value); }
        public int stepsCompleted { set => SetParameter("stepsCompleted", value); }
    }
}