using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenusClickHelper : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            if (!IsPointerOverUIObject())
            {
                //MenuWithNormalView.onClickedOutside?.Invoke();
            }
        }
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (!IsPointerOverUIObject())
                {
                  // MenuWithNormalView.onClickedOutside?.Invoke();
                }

            }
           
        }
    }
    private bool IsPointerOverUIObject()
    {
        
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        if (Input.touchCount > 0)
        {
            pointerEventData.position = Input.GetTouch(0).position;
        }

     
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);
        
        return results.Count > 0;
    }
}