using DG.Tweening;
using UnityEngine;

public class TutorialsAnimationManager : MonoBehaviour
{
    [SerializeField] private TutorialsDisplay _tutorialsDisplay;
    TutorialSO _tutorialSO;
    RectTransform _tutorialCardRectTran;

    public void AssignTutorialsData(TutorialSO SO, RectTransform _tutorialCardRect)
    {
        _tutorialSO = SO;
        _tutorialCardRectTran = _tutorialCardRect;
    }

    public void StartAnimatingFromWelcomingBox()
    {
        AnimateTutorialCard(_tutorialSO._tutorialModules[0]._data[0], _tutorialCardRectTran);
    }

    public void AnimateWelcomingIconAndPanel()
    {
        
        _tutorialsDisplay.TutorialIcon.GetComponent<RectTransform>().localScale = Vector3.zero;
        _tutorialsDisplay.TutorialWelcomingPanel.localScale = Vector3.zero;

        DOVirtual.DelayedCall(1f, () =>
        {
            _tutorialsDisplay.TutorialIcon.GetComponent<RectTransform>().DOScale(Vector3.one, 0.5f).OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.25f, () =>
                {
                    _tutorialsDisplay.TutorialWelcomingPanel.DOScale(Vector3.one, 0.3f);
                });
            });

        });
    }

    public void AnimateTutorialCard(TutorialData tutorialData, RectTransform _tutorialCardRect)
    {
        if(tutorialData._animationData._canAnimate)
        {
            Vector2 startPos = tutorialData._animationData._animationStartPosition;

            if (!tutorialData._animationData._isReversed)
            {
                _tutorialCardRect.anchoredPosition = startPos;
                _tutorialCardRect.DOAnchorPos(tutorialData._cardPositionPlaceholder, 0.3f);
            }
            else
            {
                _tutorialCardRect.anchoredPosition = tutorialData._cardPositionPlaceholder;
                _tutorialCardRect.DOAnchorPos(startPos, 0.3f);
            }

        }
    }
    

}
