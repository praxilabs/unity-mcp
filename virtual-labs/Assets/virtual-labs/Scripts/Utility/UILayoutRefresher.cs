using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILayoutRefresher : MonoBehaviour
{
    [field: SerializeField] public List<RectTransform> TargetList {get; private set;}

    public void Refresh()
    {
        foreach(var target in TargetList)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(target);
        }
    }
}
