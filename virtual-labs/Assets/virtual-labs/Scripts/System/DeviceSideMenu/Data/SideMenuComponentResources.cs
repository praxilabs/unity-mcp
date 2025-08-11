using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    [CreateAssetMenu(fileName = "SideMenuComponentResources", menuName = "DeviceSideMenu/SideMenuComponentResources", order = 0)]
    public class SideMenuComponentResources : ScriptableObject
    {
        [SerializeField] private List<ComponentStringPrefabPair> componentStringPrefabPairs = new List<ComponentStringPrefabPair>();

        public GameObject GetComponentPrefab(string desiredName)
        {
            for(int i = 0; i < componentStringPrefabPairs.Count; i++)
            {
                if(desiredName == componentStringPrefabPairs[i].Name)
                {
                    return componentStringPrefabPairs[i].Prefab;
                }
            }

            Debug.LogWarning(desiredName + " is not found in the ComponentStringPrefabPairs list, returning a new gameobject");
            return new GameObject(desiredName + "_Dumb");
        }
    }

    [System.Serializable]
    public class ComponentStringPrefabPair
    {
        [field: SerializeField] public string Name {get; private set;}
        [field: SerializeField] public GameObject Prefab {get; private set;}
    }
}