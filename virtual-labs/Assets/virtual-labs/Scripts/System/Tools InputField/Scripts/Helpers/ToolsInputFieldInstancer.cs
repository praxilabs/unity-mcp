using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolsInputFieldInstancer : MonoBehaviour
{
    [SerializeField] private GameObject _toolsInputFieldCanvasPrefab;

    public GameObject InstanceToolsInputCanvas()
    {
       return Instantiate(_toolsInputFieldCanvasPrefab);
    }


}
