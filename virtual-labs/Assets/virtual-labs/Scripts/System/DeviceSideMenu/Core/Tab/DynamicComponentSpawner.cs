using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DynamicComponentSpawner : MonoBehaviour 
    {
        [SerializeField] private SideMenuComponentResources sideMenuComponentResources;

        public GameObject SpawnComponent(string desiredComponentName)
        {
            return Instantiate(sideMenuComponentResources.GetComponentPrefab(desiredComponentName));
        }
    }
}