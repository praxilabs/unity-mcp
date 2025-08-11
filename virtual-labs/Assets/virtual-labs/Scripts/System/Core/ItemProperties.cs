using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Xml;

public class ItemProperties : MonoBehaviour
{
    // ItemProperties
    // Use this for initialization
    /// <summary>
    /// Should be removed
    /// </summary>
    public Sprite prefabImg;
    public bool visible = false;
    public string type = "";
    public string showingName = "";
    public string realName = "";
    public Vector3 itemPosition;
    public Vector3 itemScale;
    public Vector3 itemRotation;
    // prefab for the item
    public GameObject itemPrefab;
    public int itemIndex;
    public string itemType;
    void Start()
    {
        itemPosition = transform.position;
        itemRotation = transform.localEulerAngles;

    }


    public void AddOnMouseHoverText(string value)
    {
        // gameObject.AddComponent<ShowToolTipOnMouseHover>().TooltipText = value;

        if (OnAddOnMouseHoverText != null)
        {
            OnAddOnMouseHoverText(this.gameObject, "AddOnMouseHoverText");
        }
    }
    public event System.Action<GameObject, string> OnAddOnMouseHoverText;

    public void ChangePos(string xpos, string ypos, string zpos)
    {
        transform.position = new Vector3(float.Parse(xpos), float.Parse(ypos), float.Parse(zpos));
    }

    public void AddToolTipOnMouse(string value)
    {
        // ShowToolTipOnMouseHover showToolTipOnMouseHover = gameObject.AddComponent<ShowToolTipOnMouseHover>();
        // showToolTipOnMouseHover.TooltipText = value;

        // if (OnAddToolTipOnMouse != null)
        // {
        //     OnAddToolTipOnMouse(gameObject, "OnAddToolTipOnMouse");
        // }
    }
    public event System.Action<GameObject, string> OnAddToolTipOnMouse;

    public void ChangeRotation(string x, string y, string z)
    {

        itemRotation = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
        if (OnChangeRotation != null)
        {
            OnChangeRotation(gameObject, "ChangeRotation");
        }
    }
    public event System.Action<GameObject, string> OnChangeRotation;


}
