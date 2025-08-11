using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using PraxiLabs;

public class DevicePointsManager : MonoBehaviour
{
    

    void Start()
    {
        string path = transform.parent.parent.name + "/" + transform.parent.name + "/" + name;
        //CustomDebugger.Log("$$$$$$$$$$     " + path);
    }

    public void TriggrPoint()
    {
        string path = "";
        if (transform.parent.parent != null)
        {
            if (transform.parent.parent.parent != null)
            {
                if (transform.parent.parent.parent.parent != null)
                {
                    path = transform.parent.parent.parent.parent.name + "/" + transform.parent.parent.parent.name + "/" + transform.parent.parent.name + "/" + transform.parent.name + "/" + name;
                }
                else
                    path = transform.parent.parent.parent.name + "/" + transform.parent.parent.name + "/" + transform.parent.name + "/" + name;
            }
            else
                path = transform.parent.parent.name + "/" + transform.parent.name + "/" + name;
        }


        // CustomDebugger.Log("&&&&&&&&&&&     " + path);
        // if (!string.IsNullOrEmpty(path) && ConnectionManager.Instance)
        // {
        //     ConnectionManager.Instance.CheckConnection(path);
        // }
    }

    void Update()
    {
        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //if (Physics.Raycast(ray, out hit, 100.0f))
        //{

        //    CustomDebugger.Log("You selected the " + hit.transform.name +"  "+ hit.transform.); // ensure you picked right object
        //}
    }


    public void OnMouseDown()
    {
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            TriggrPoint();
        }
    }
}
