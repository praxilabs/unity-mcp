using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PraxiLabs.MCQ
{
    [RequireComponent(typeof(ExperimentQuestionsParser))]
    public class MCQWrapper : MonoBehaviour 
    {
        public Dictionary<int, BaseInteractiveQuestion> QuestionResponseMap {get; set;}
        [SerializeField] private MCQController _mcqController;
        public UnityEngine.UI.Button GetCloseButton() => _mcqController.CloseButton;
        private int _currentIndex;

        public void ShowQuestion(int index)
        {
            _currentIndex = index;
            HandleCurrentQuestion(_currentIndex);
        }

        private void HandleCurrentQuestion(int index)
        {
            if(!QuestionResponseMap.ContainsKey(index))
            {
                Debug.LogError("Required question number is not found!");
                return;
            }

            BaseInteractiveQuestion interactiveQuestion = QuestionResponseMap[index];
            _mcqController.SetQuestion(interactiveQuestion);
        }

        public void UpdateCurrentQuestion()
        {
            if (QuestionResponseMap.ContainsKey(_currentIndex) == false) return;
            BaseInteractiveQuestion interactiveQuestion = QuestionResponseMap[_currentIndex];
            _mcqController.UpdateQuestion(interactiveQuestion);
            _mcqController.UpdateText();
        }
    }
}