using ProgressMap.Structure;
using UnityEngine;

namespace ProgressMap.UI.ExpandedView
{
    public class SelectedStageUI : MonoBehaviour
    {
        #region fields
        private ProgressMap.Structure.Stage _selectedStage;
        [SerializeField] private RectTransform _descriptionArea;
        [SerializeField] private UnityEngine.UI.LayoutGroup _layoutGroup;
        [SerializeField] private string _stageNamePrefix = "Stage";
        [SerializeField] private TMPro.TextMeshProUGUI _stageDescription;
        [SerializeField] private string _stageDescriptionStyle = string.Empty;
        [SerializeField] private TMPro.TextMeshProUGUI _stageName;
        [SerializeField] private string _stageNameStyle = string.Empty;
        [SerializeField] private ProgressBar _progressBar;
        #endregion

        #region properties
        public Stage SelectedStage
        {
            get => _selectedStage;
            set
            {
                if (_selectedStage != null)
                    _selectedStage.ProgressUpdate -= UpdateUI;
                _selectedStage = value;
                _selectedStage.ProgressUpdate += UpdateUI;
                UpdateUI();
            }
        }
        #endregion

        #region methods
        private void UpdateUI()
        {
            UpdateText();
            _progressBar.TotalProgress = _selectedStage.TotalProgress;
            _progressBar.Progress = _selectedStage.Progress;
        }

        private string StyleText(string text, string style)
        {
            if (style == string.Empty || style.Length == 0)
                return text;
            return $"<style=\"{style}\">{text}</style>";
        }

        public void UpdateText()
        {
            SetStageDescriptionText(_selectedStage.Description);
            SetStageName(_selectedStage.StageName);
        }

        private void SetStageDescriptionText(string description)
        {
            _stageDescription.text = StyleText(description, _stageDescriptionStyle);
            _descriptionArea.sizeDelta = new Vector2(_descriptionArea.sizeDelta.x, _layoutGroup.preferredHeight);
        }
        private void SetStageName(string stagePrefix)
        {
            _stageName.text = StyleText($"{stagePrefix} {_selectedStage.StageNumber}: {_selectedStage.LastStepDone}/{_selectedStage.Steps.Length}",
                            _stageNameStyle);
        }
        #endregion
    }
}