using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolsInputFieldClickHelper : MonoBehaviour ,IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject _toolsInputField;
    bool isOverUI;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (isOverUI) return;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == transform.gameObject)
                {
                    _toolsInputField.SetActive(true);
                }
                else
                {
                    _toolsInputField.SetActive(false);
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOverUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOverUI = false;
    }
}
