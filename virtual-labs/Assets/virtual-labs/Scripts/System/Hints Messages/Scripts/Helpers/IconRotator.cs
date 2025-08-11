using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class IconRotator : MonoBehaviour
{
    [SerializeField] private RectTransform iconRectTransform;
    [SerializeField] private float startRot,endRot;

    private void Start()
    {
        iconRectTransform.rotation = Quaternion.identity;
    }

    public void RotateIcon()
    {
        float currentZRotation = iconRectTransform.rotation.eulerAngles.z;

        if (currentZRotation == 0)
        {
            iconRectTransform.rotation = Quaternion.Euler(0, 0, endRot);
        }
        else
        {
            iconRectTransform.rotation = Quaternion.Euler(0, 0, startRot);
        }
    }

    public void RotateIcon(bool shouldRotateToEndPosition)
    {
        if(shouldRotateToEndPosition)
        {
            iconRectTransform.rotation = Quaternion.Euler(0, 0, endRot);
        }
        else
        {
            iconRectTransform.rotation = Quaternion.Euler(0, 0, startRot);
        }
    }
}
