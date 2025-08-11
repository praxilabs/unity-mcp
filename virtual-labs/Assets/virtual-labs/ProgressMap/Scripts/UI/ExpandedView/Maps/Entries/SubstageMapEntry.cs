using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ProgressMap.UI.ExpandedView.Maps.Entries
{
    public class SubstageMapEntry : MonoBehaviour
    {
        #region fields
        [SerializeField] private CanvasGroup _checkBox;
        [SerializeField] private TMPro.TextMeshProUGUI _substageText;
        [SerializeField] private TMPro.FontStyles _normalStyle;
        [SerializeField] private TMPro.FontStyles _completeStyle;
        [SerializeField] private GameObject _connector;
        [SerializeField] private bool _isLast = false;
        private bool _done;

        public SubstageMapEntry(bool done)
        {
            _done = done;
        }
        #endregion

        #region properties

        public bool Done { get => _done; set { _done = value; UpdateUI(); } }
        #endregion

        #region methods
        private void Start()
        {
            Initialize();
        }
        [ContextMenu("Initiaize")]
        public void Initialize()
        {
            UpdateUI();
        }

        [ContextMenu("ToggleDone")]
        public void ToggleDone()
        {
            Done = !Done;
        }
        [ContextMenu("Update UI")]
        private void UpdateUI()
        {
            _substageText.fontStyle = Done ? _completeStyle : _normalStyle;
            _checkBox.DOFade(Done ? 1 : 0, .5f);
            _connector.SetActive(!_isLast);
        }
        public void SetData(string substageText, bool isLast)
        {
            UpdateText(substageText);
            this._isLast = isLast;
        }

        public void UpdateText(string substageText)
        {
            _substageText.text = substageText;
        }
        #endregion
    }
}