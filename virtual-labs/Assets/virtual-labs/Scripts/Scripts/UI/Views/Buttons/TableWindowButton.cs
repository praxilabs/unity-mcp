using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Table.UI.Views.Buttons
{
    public class TableWindowButton : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI _stageNumberText;
        [SerializeField] private GameObject _stageNumberCircle;
        [SerializeField] private UnityEngine.UI.Button _myButton;

        public bool StagePassed { get; set; }

        public TablesManager TablesManager { get; set; }
        public UI.Views.TablesWindow TablesView { get; set; }
        public UI.Views.TableTransitions TableTransitions { get; set; }
        public int StageIndex { get; set; }
        public bool ShowNumber { get; set; }
        public bool Interactable { get => _myButton.interactable; set => _myButton.interactable = value; }

        private int _currentStageIndex;
        private bool _isPinned;
        private bool _isActive = true;
        public void Initialize()
        {
            //_stageNumberText.text = StageIndex.ToString();
            //_stageNumberCircle.SetActive(ShowNumber);
            gameObject.name = $"Stage {StageIndex} table window button";
            TableTransitions.OnTableHidden.AddListener(OnTableHidden);
            TableTransitions.OnTableShown.AddListener(OnTableShown);
            TablesView.onTableWindowPinChanged.AddListener(SetPinned);
        }

        private void OnTableShown()
        {
            Interactable = false;
        }

        public void SetCurrentStageIndex(int currentStageIndex)
        {
            _currentStageIndex = currentStageIndex;
            //UpdateActive();
        }

        public void SetPinned(bool isPinned)
        {
            _isPinned = isPinned;
            //UpdateActive();
        }
        public bool Active => _isActive && (_isPinned || StageIndex == _currentStageIndex);
        //private void UpdateActive()
        //{
        //    Debug.Log($"button {gameObject.name}, is pinned {_isPinned}, stageIndex={StageIndex}, current stage {_currentStageIndex}");
        //    gameObject.SetActive(_isActive && (_isPinned || StageIndex == _currentStageIndex));
        //}

        private void OnTableHidden()
        {
            Interactable = true;
        }
        public void ShowTable()
        {
            TableTransitions.ExpandSmallView();
        }

        public void HideTable()
        {
            TableTransitions.Collapse();
        }
    }
}