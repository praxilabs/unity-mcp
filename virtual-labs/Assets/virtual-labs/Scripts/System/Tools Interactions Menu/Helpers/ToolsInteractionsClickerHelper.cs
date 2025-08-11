using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolsInteractionsClickerHelper : MonoBehaviour, IPointerEnterHandler , IPointerExitHandler
{
    private GameObject _highlighter;
    [SerializeField] private ToolsInteractionsMenu _toolsInteractionMenuManager;
    bool isOverUI;
    //[SerializeField] private GameObject _ToolsMenuPrefab;
    

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && XnodeManager.Instance.CurrentStep is ToolsInteractionsMenuStep)
        {
            if (_toolsInteractionMenuManager == null) return;
            if (isOverUI) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == transform.gameObject)
                {
                    _toolsInteractionMenuManager.gameObject.SetActive(true);
                }
                else
                {
                    _toolsInteractionMenuManager.gameObject.SetActive(false);
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

    internal void InitializeMenu(ToolsInteractionsMenu parent, string ButtonName)
    {

        _toolsInteractionMenuManager = parent;
        _toolsInteractionMenuManager.InitializeMenuList(ButtonName);
    }
}
