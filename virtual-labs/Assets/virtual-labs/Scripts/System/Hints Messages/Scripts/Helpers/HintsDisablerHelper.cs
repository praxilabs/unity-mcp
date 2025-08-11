using Praxilabs.UIs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintsDisablerHelper : MonoBehaviour
{
    [SerializeField] private HintsManager _hintsManager;
    [SerializeField] private Canvas _oxiCanvas;

    private void OnEnable()
    {
        _oxiCanvas.enabled = true;
    }

    private void OnDisable()
    {
        _oxiCanvas.enabled = false;
        _hintsManager.HideHintBoxWhilePinned();
    }
    public void ShowHints()
    {
        _hintsManager.ShowHintsBoxWhenPinned();
    }
}
