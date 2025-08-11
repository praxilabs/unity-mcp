using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[RequireComponent(typeof(CanvasGroup))]
public class ToolsInputFieldManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _predefinedValue;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _unitText;


    private string _unit;
    private float _fadeDuration = 1.25f;
    private bool _isCorrect;
    public float PredefinedValue { get => _predefinedValue; set => _predefinedValue = value; }
    public bool IsCorrect { get => _isCorrect; set => _isCorrect = value; }
    public string Unit { get => _unit; set => _unit = value; }

    public void Init(Vector3 uiOffset)
    {
        GetComponent<Canvas>().enabled = true;
        _unitText.text = Unit;
        transform.position += uiOffset;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        if (float.TryParse(_inputField.text, out float userInput))
        {
            if (userInput == PredefinedValue)
            {
                Debug.Log("Correct");
                IsCorrect = true;
                _text.color = Color.green;
                StartCoroutine(FadeOut());
            }
            else
            {
                Debug.Log("Incorrect");
                _text.color = Color.red;
                StartCoroutine(ResetColor());
            }
        }
    }

    private IEnumerator FadeOut()
    {
        float startAlpha = _canvasGroup.alpha;

        for (float t = 0; t < _fadeDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / _fadeDuration;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, normalizedTime);
            yield return null;
        }


        XnodeManager.Instance.CurrentStep.Execute(gameObject);
        _inputField.text = string.Empty;
        _canvasGroup.alpha = 1;
        _text.color = Color.white;

        Destroy(gameObject);
        //GetComponent<Canvas>().enabled = false;
        
    }

    private IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(1f);
        _inputField.text = "";
        _text.color = Color.white;
    }

    public enum ToolsInputFieldUnits{
        ml,
        g,
    }
}
