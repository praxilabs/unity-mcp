using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolsInteractionsMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menuContainer;

    public void InitializeMenuList(string ButtonName)
    {
            Transform childTransform = _menuContainer.transform.Find(ButtonName);

            if (childTransform != null)
            {
                childTransform.gameObject.GetComponent<Button>().interactable = true;
            }
    }

    internal void ResetMenu()
    {
        for (int i = 0; i < _menuContainer.transform.childCount; i++)
        {
            _menuContainer.transform.GetChild(i).gameObject.GetComponent<Button>().interactable = false;
        }
    }
}
