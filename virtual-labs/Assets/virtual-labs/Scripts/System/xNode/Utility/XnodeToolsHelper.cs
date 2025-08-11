using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XnodeToolsHelper : Singleton<XnodeToolsHelper>
{
    [HideInInspector]
    public Dictionary<string, object> savedValues;
    [HideInInspector]
    public Dictionary<string, GameObject> savedRefrences;
    [HideInInspector]
    public List<GameObject> flashObjects;
    [HideInInspector]
    public List<GameObject> attachObjects;
  
    public void Init()
    {
        flashObjects = new List<GameObject>();
        attachObjects = new List<GameObject>();
        savedRefrences = new Dictionary<string, GameObject>();
        savedValues = new Dictionary<string, object>();

        //XnodeManager.Instance.E_IgnoreToolsPrepration.AddListener(ClearLists);
    }

    //called from node graph
    public void RenameSavedTool(string key, string newName)
    {
        savedRefrences[key].name = newName;
        savedRefrences.Remove(key);
    }
    private void ClearLists()
    {
        flashObjects.Clear();
        attachObjects.Clear();
    }
 
}
