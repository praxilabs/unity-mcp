using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PraxiLabs.MCQ
{
    public class AnswerUI : MonoBehaviour 
    {
        [field: SerializeField, Header("General Elements")] public Toggle Toggle {get; private set;}
        [SerializeField] private TextMeshProUGUI _answerText; 
        [SerializeField] private Image _borderImage;
        [SerializeField] private Image _overlayImage;
        [SerializeField] private Image _toggleImage;
        [SerializeField] private Image _checkmarkImage;

        [Header("Explanation Elements")]
        [SerializeField] private TextMeshProUGUI _explanationText;
        [SerializeField] private GameObject _explanationContainer;
        [SerializeField] private GameObject _toggleImageDuplicateBox;

        [Header("Color/Icon Settings")]
        [SerializeField] private Color _correctColor;
        [SerializeField] private Color _incorrectColor;
        [SerializeField] private Sprite _correctIcon;
        [SerializeField] private Sprite _incorrectIcon;

        private AnswerData _answerData;
        private Sprite _defaultIcon;
        private Color _defaultBorderColor;
        private Color _correctColorOpaque;
        private Color _incorrectColorOpaque;
        private Color _fullyTransparentWhite;

        private Action _layoutRefreshAction;

        private void Awake()
        {
            _defaultBorderColor = _borderImage.color;
            _defaultIcon = _toggleImage.sprite;
            _correctColorOpaque = new Color(_correctColor.r, _correctColor.g, _incorrectColor.b, 1f);
            _incorrectColorOpaque = new Color(_incorrectColor.r, _incorrectColor.g, _incorrectColor.b, 1f);
            _fullyTransparentWhite = Color.white;
            _fullyTransparentWhite.a = 0f;
        }

        public AnswerData GetAnswerData()
        {
            return _answerData;
        }

        public void SetAnswerData(AnswerData answerData)
        {
            _answerData = answerData;
            _answerText.text = _answerData.Answer;
            _explanationText.text = _answerData.Explanation;
        }

        public void DisplayFeedback(bool isCorrect, bool showExplanation, Action layoutRefreshAction)
        {
            _layoutRefreshAction = layoutRefreshAction;
            if(isCorrect)
                CorrectFeedback(showExplanation);
            else
                IncorrectFeedback(showExplanation);
        }

        public void SetToggleInteractability(bool isInteractable)
        {
            Toggle.interactable = isInteractable;
        }
        public void SetToggleImageVisibility(bool isVisible)
        {
            _toggleImage.gameObject.SetActive(isVisible);
            _toggleImageDuplicateBox.gameObject.SetActive(isVisible);
        }

        public void Reset()
        {
            // Toggle.SetIsOnWithoutNotify(false);
            _answerData = null;

            ResetVisuals();
            _explanationText.text = "";

            SetToggleInteractability(true);
            SetToggleImageVisibility(true);
        }

        public void ResetVisuals()
        {
            _checkmarkImage.enabled = true;
            _borderImage.color = _defaultBorderColor;
            _toggleImage.sprite = _defaultIcon;
            _toggleImage.color = Color.white;
            _answerText.color = Color.white;
            _overlayImage.color = _fullyTransparentWhite;
            // Hide Explanation;
            _explanationContainer.SetActive(false);
        }

        public void SetCorrectAnswer(bool showExplanation)
        {
            CorrectFeedback(showExplanation);
        }

        private void CorrectFeedback(bool showExplanation)
        {
            _checkmarkImage.enabled = false;
            _borderImage.color = _correctColorOpaque;
            _toggleImage.sprite = _correctIcon;
            _toggleImage.color = _correctColorOpaque;
            _answerText.color = _correctColorOpaque;
            _overlayImage.color = _correctColor;
            // Show or Hide Explanation
            if(showExplanation)
            {
                _explanationContainer.SetActive(true);
            }

            _layoutRefreshAction?.Invoke();
        }

        private void IncorrectFeedback(bool showExplanation)
        {
            _checkmarkImage.enabled = false;
            _borderImage.color = _incorrectColorOpaque;
            _toggleImage.sprite = _incorrectIcon;
            _toggleImage.color = _incorrectColorOpaque;
            _answerText.color = _incorrectColorOpaque;
            _overlayImage.color = _incorrectColor;
            // Show or Hide Explanation
            if(showExplanation)
            {
                _explanationContainer.SetActive(true);
            }

            _layoutRefreshAction?.Invoke();
        }
    }
}