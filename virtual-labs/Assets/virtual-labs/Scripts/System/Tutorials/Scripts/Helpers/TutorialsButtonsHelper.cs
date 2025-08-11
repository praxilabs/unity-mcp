using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialsButtonsHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    [SerializeField] private Color hoverColor;
    [SerializeField] private Button hoverButton;

    private Color originalColor;
    private Image buttonImage;

    void Start()
    {
        buttonImage = GetComponent<Image>();
        originalColor = buttonImage.color;  
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.color = hoverColor; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.color = originalColor;  
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        hoverButton.onClick.Invoke();
    }
}
