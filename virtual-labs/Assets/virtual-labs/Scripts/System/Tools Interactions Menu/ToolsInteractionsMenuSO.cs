using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tools Interactions Menu Item SO", menuName = "Tools/Tools Interactions Menu SO")]
public class ToolsInteractionsMenuSO : ScriptableObject
{
    [SerializeField] private List <ToolsInteractionsMenuItem> _menuItems;

    public List<ToolsInteractionsMenuItem> MenuItems { get => _menuItems; set => _menuItems = value; }
}


[Serializable]
public class ToolsInteractionsMenuItem
{
    public GameObject _ButtonPrefab;
}
