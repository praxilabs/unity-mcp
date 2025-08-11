using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePointsBeforeController : MonoBehaviour
{
    public void ChangePointPosition(string pointName, string X, string Y, string Z)
    {
        Transform point = transform.Find($"LinePoints/{pointName}");

        float x = float.Parse(X);
        float y = float.Parse(Y);
        float z = float.Parse(Z);

        Vector3 newPosition = new Vector3(x, y, z);

        point.localPosition = newPosition;

        OnChangePointPosition?.Invoke(gameObject, "ChangePointPosition");
    }
    public event System.Action<GameObject, string> OnChangePointPosition;

    public void ChangePointRotation(string pointName, string X, string Y, string Z)
    {
        Transform point = transform.Find($"LinePoints/{pointName}");

        float x = float.Parse(X);
        float y = float.Parse(Y);
        float z = float.Parse(Z);

        Quaternion newRotation = Quaternion.Euler(x, y, z);

        point.localRotation = newRotation;

        OnChangePointRotation?.Invoke(gameObject, "ChangePointRotation");
    }
    public event System.Action<GameObject, string> OnChangePointRotation;

    public void AddNewLinePoint(string pointName)
    {
        Transform parentName = transform.Find($"LinePoints");
        GameObject newPoint = new GameObject(pointName);
        newPoint.transform.parent = parentName;

        OnAddNewLinePoint?.Invoke(gameObject, "AddNewLinePoint");
    }
    public event System.Action<GameObject, string> OnAddNewLinePoint;

    public void ChangePointName(string pointName, string newName)
    {
        Transform point = transform.Find($"LinePoints/{pointName}");
        point.gameObject.name = newName;

        OnChangePointName?.Invoke(gameObject, "ChangePointName");
    }
    public event System.Action<GameObject, string> OnChangePointName;
}
