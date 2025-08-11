using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

namespace Praxilabs.UIs
{
    public class TeaseHintsDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _teaseHintText;
        [SerializeField] private Button _confirmMoreHelp;
        [SerializeField] private Button _cancel;
        [SerializeField] private CanvasWebViewPrefab _MainCanvasWebPrefab;
        public TextMeshProUGUI TeaseHintText => _teaseHintText;

        public Button ConfirmMoreHelp => _confirmMoreHelp;

        public Button Cancel => _cancel;

        public CanvasWebViewPrefab MainCanvasWebPrefab
        {
            get => _MainCanvasWebPrefab;
            set => _MainCanvasWebPrefab = value;
        }
    }
}