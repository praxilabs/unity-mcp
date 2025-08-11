using ProgressMap.Structure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProgressMap.UI.ExpandedView
{
    public class JumpButton : MonoBehaviour
    {
        #region fields
        private Stage _selectedStage;
        private Stage _currentStage;
        [SerializeField] private UnityEngine.UI.Button _jumpButton;
        [SerializeField] private ProgressMapController _progressMapController;
        [SerializeField] private JumpPrompt _jumpPrompt;

        [SerializeField] private char _jumpPromptStageReplaceCharacter;
        #endregion

        #region properties
        public Stage CurrentStage { get => _currentStage; set { _currentStage = value; UpdateUI(); } }
        public Stage SelectedStage { get => _selectedStage; set { _selectedStage = value; UpdateUI(); } }
        #endregion

        #region methods
        private void UpdateUI()
        {
            if (_currentStage == null || _selectedStage == null)
            {
                _jumpButton.interactable = false;
                return;
            }
            _jumpButton.gameObject.SetActive(_selectedStage != _currentStage);
            _jumpButton.interactable = _selectedStage.CanJumpTo && _currentStage.CanJumpFrom;
        }

        public void JumpPrompt()
        {
            _jumpPrompt.Prompt(_jumpPromptStageReplaceCharacter, _selectedStage.StageNumber, ChangeCurrent, () => print("Jump prompt canceled"));
        }
        public void ChangeCurrent()
        {
            _progressMapController.SelectedEqualCurrent();
        }
        #endregion
    }
}