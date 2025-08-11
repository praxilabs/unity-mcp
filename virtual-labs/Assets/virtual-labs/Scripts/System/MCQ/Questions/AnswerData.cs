using UnityEngine;

namespace PraxiLabs.MCQ
{
    [System.Serializable]
    public class AnswerData
    {
        [field: SerializeField] public string Answer {get; set;}
        [field: SerializeField, TextArea(1, 3)] public string Explanation {get; set;}
    }
}