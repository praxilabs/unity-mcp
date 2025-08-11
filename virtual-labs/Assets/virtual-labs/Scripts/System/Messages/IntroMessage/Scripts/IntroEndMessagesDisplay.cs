using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.UIs
{
    [RequireComponent(typeof(IntroEndMessagesManager))]
    public class IntroEndMessagesDisplay : MonoBehaviour
    {
        [Header("Text Sections")]
        [SerializeField] private TextMeshProUGUI _header;
        [SerializeField] private TextMeshProUGUI _body;
        [SerializeField] private TextMeshProUGUI _footer;

        [Header("Buttons Set")]
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _backButton;

        [Header("Next/End Button Sprites")]
        [SerializeField] private Sprite _nextButtonInitialImage;
        [SerializeField] private Sprite _closeImage;

        [Header("Colors of Buttons & Segment")]
        [SerializeField] private Color _disabledColor;
        [SerializeField] private Color _activeColor;

        [Header("End Icons")]
        [SerializeField] private Image _oxiImage;
        [SerializeField] private Image _cupImage;

        [Header("Nav Segment")]
        [SerializeField] private GameObject _lineSegementContainer;
        [SerializeField] private GameObject _lineSegementPrefab;


        public TextMeshProUGUI Header => _header;
        public TextMeshProUGUI Body => _body;
        public TextMeshProUGUI Footer => _footer;
        public Button NextButton => _nextButton;
        public Button BackButton => _backButton;
        public Sprite NextButtonInitialImage => _nextButtonInitialImage;
        public Sprite CloseImage => _closeImage;
        public Image OxiImage => _oxiImage;
        public Image CupImage => _cupImage;
        public GameObject LineSegementContainer => _lineSegementContainer;
        public GameObject LineSegementPrefab => _lineSegementPrefab;
        public Color DisabledColor => _disabledColor;
        public Color ActiveColor => _activeColor;
    }

}