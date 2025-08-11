using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private List<CanvasGroup> _targetCanvasGroups;
    private bool _isHidden;
    public void SetTargetCanvasGroups(List<CanvasGroup> canvasGroups) => _targetCanvasGroups = canvasGroups;

    public void ToggleTargetUI(bool enable)
    {
        if (enable == false && _isHidden || enable == true && !_isHidden) return;

        foreach (CanvasGroup canvasGroup in _targetCanvasGroups)
        {
            canvasGroup.DOFade(enable ? 1 : 0.1f, 0.5f);
            canvasGroup.interactable = enable;
            canvasGroup.blocksRaycasts = enable;
        }

        _isHidden = !enable;
    }
}
