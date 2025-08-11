using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ProgressMap.UI.ExpandedView
{
    public class ExperimentTitle : MonoBehaviour
    {
        #region fields
        [SerializeField] private RectTransform _description;
        [SerializeField] private RectTransform _descriptionTitle;
        [SerializeField] private TMPro.TextMeshProUGUI _descriptionTitleTmp;
        [SerializeField] private TMPro.TextMeshProUGUI _descriptionTmp;

        [SerializeField] private float _descriptionMinimumHeight;
        [SerializeField] private float _descriptionExpandCollapseTime;
        [SerializeField] private Ease _descriptionExpandCollapseEase;
        [SerializeField, Tooltip("If difference between expanded & collapsed sizes is smaller than this, Ignore collapsing.")]
        private float _minimumDifferenceForCollapse;
        private DG.Tweening.Core.TweenerCore<Vector2, Vector2, DG.Tweening.Plugins.Options.VectorOptions> _tweenerCore;
        private Vector2 _descriptionExpandedSize;
        private Vector2 _descriptionMinimizedSize;

        [field: SerializeField, TextArea(5, 1203)] //string titleText;
        public string TitleText { get; set; }
        [field: SerializeField, TextArea(5, 1203)]
        public string SubtitleText { set; get; } = "Experiment title";

        private float _descriptionExpandTimeStamp = 0;
        [SerializeField] private StylingTags _stylingTags;
        [System.Serializable]
        public struct StylingTags
        {
            public string titleTag;
            public string bodyTag;
        }
        public string TagText(string Text, string tag)
        {
            if (tag == string.Empty || tag.Length == 0)
                return Text;
            return $"<style=\"{tag}\">{Text}</style>";
        }

        private bool _expanded = true;
        private bool _canExpandCollapse = true;
        [SerializeField] private Vector4 _collapsedMargins;
        [SerializeField] private Vector4 _expandedMargins;
        #endregion

        #region methods
        private void Start()
        {
            Initialize();
        }
        public void Initialize()
        {

            UpdateText();
            CalculateSizes();

            if (_expanded)
                Expand(immediately: true);
            else
                Collapse(immediately: true);
        }
        public void UpdateText()
        {

            _descriptionTitleTmp.text = TagText(TitleText + (_expanded ? "" : " ..."), _stylingTags.titleTag);
            _descriptionTmp.text = TagText(SubtitleText, _stylingTags.bodyTag);

#if UNITY_EDITOR
            CalculateSizes();
#endif
        }
        [SerializeField]
        private bool _enterOrJustTitle;

        private void CalculateSizes()
        {
            _descriptionExpandedSize = _description.sizeDelta;
            _descriptionMinimizedSize = _descriptionExpandedSize;
            Vector2 titleSize = _descriptionTitle.sizeDelta;
            titleSize.y = _descriptionTitleTmp.preferredHeight;
            _descriptionTitle.sizeDelta = titleSize;

            _descriptionExpandedSize.y = _descriptionTmp.preferredHeight;
            _descriptionMinimizedSize.y = _descriptionMinimumHeight;

            _canExpandCollapse = Mathf.Abs(_descriptionExpandedSize.y - _descriptionMinimizedSize.y) < _minimumDifferenceForCollapse;
 
        }


        [ContextMenu("Expand")]
        public void Expand() => Expand(false);
        public void Expand(bool immediately)
        {
            _expanded = true;

            UpdateText();

            if (immediately)
            {
                _description.sizeDelta = _descriptionExpandedSize;
                return;
            }

            _tweenerCore.Kill(true);
            _tweenerCore = _description.DOSizeDelta(_descriptionExpandedSize, _descriptionExpandCollapseTime);
            _descriptionExpandTimeStamp = Time.time;
        }


        [ContextMenu("Collapse")]
        public void Collapse() => Collapse(false);
        public void Collapse(bool immediately)
        {
            if (_canExpandCollapse)
                return;
            _expanded = false;
            UpdateText();

            _tweenerCore.Kill(true);
            if (immediately)
            {
                _description.sizeDelta = _descriptionMinimizedSize;
                return;
            }
            _tweenerCore = _description.DOSizeDelta(_descriptionMinimizedSize, Mathf.Min(Time.time - _descriptionExpandTimeStamp, _descriptionExpandCollapseTime))
                .SetEase(_descriptionExpandCollapseEase).OnComplete(UpdateText);

        }

        public void Toggle()
        {
            if (_expanded)
                Collapse();
            else
                Expand();
        }

        #endregion
    }
}