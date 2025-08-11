using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.UIs
{
  public class ProgressBarManager : MonoBehaviour
  {

    [SerializeField] private GameObject _circlePrefab;
    [SerializeField] private Color _lineConnectorFinishColor = new(0.4196078f, 0.3490196f, 0.772549f, 1);

    private List<GameObject> _circlesList = new();
    private bool _isProgressInitialized;
    private StateTypes previousCircleState = StateTypes.Unfinished;

    public void InitializeProgressBar(GameObject progressBarContainer, int numberOfCircles, Action<int> buttonAction)
    {
      if (_isProgressInitialized) return;

      for (int i = 0; i < numberOfCircles; i++)
      {
        GameObject tmp = Instantiate(_circlePrefab,
          progressBarContainer.transform);
        int tmpNumber = i;
        tmp.GetComponent<Button>().onClick.AddListener(() => { buttonAction(tmpNumber); });
        _circlesList.Add(tmp.transform.GetChild(0).gameObject);
        _circlesList[i].transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text =
          (i + 1).ToString();
      }

      _isProgressInitialized = true;
    }
    /// <summary>
    /// Initialize ProgressBar Circles and States
    /// </summary>
    /// <param name="circleNumber">A zero based index of circle number</param>
    /// <param name="circleState">Circle State</param>

    public void InitializeProgressBarStates(int circleNumber, StateTypes circleState)
    {
      if (circleNumber < 0 || circleNumber >= _circlesList.Count) return;
      UpdateCircleShape(circleNumber, circleState, previousCircleState);
      previousCircleState = circleState;
      
    }
    /// <summary>
    /// Update Circle Shape on-demand
    /// </summary>
    /// <param name="circleNumber">A zero based index of circle number</param>
    /// <param name="currentCircleState">Current circle state</param>
    /// <param name="precedingCircleState">Preceding Circle state</param>
    public void UpdateProgressBarCircleState(int circleNumber, StateTypes currentCircleState, StateTypes precedingCircleState)
    {
      if (circleNumber < 0 || circleNumber >= _circlesList.Count) return;
      UpdateCircleShape(circleNumber, currentCircleState, precedingCircleState);
    }

    private void UpdateCircleShape(int circleNumber, StateTypes currentCircleState, StateTypes prevCircleState)
    {
      GameObject circle = _circlesList[circleNumber];
      RectTransform rectTransform = circle.transform.GetChild(1).GetComponent<RectTransform>();

      bool isUnfinished = currentCircleState == StateTypes.Unfinished;
      bool isFinished = currentCircleState == StateTypes.Finished;
      bool isCurrent = currentCircleState == StateTypes.Current;

      rectTransform.GetChild(1).gameObject.SetActive(isUnfinished || isFinished || isCurrent);
      rectTransform.GetChild(2).gameObject.SetActive(isCurrent);
      rectTransform.GetChild(0).gameObject.SetActive(isFinished);

      circle.transform.GetChild(2).gameObject.SetActive((!isCurrent && !isFinished) && isUnfinished);


      if (circleNumber == _circlesList.Count - 1)
      {
        circle.transform.GetChild(0).gameObject.SetActive(false);
      }

      if (currentCircleState == StateTypes.Finished && prevCircleState == StateTypes.Finished && circleNumber > 0)
      {
        _circlesList[circleNumber - 1].transform.GetChild(0).gameObject.GetComponent<Image>().color =
          _lineConnectorFinishColor;
      }
    }
  }

}