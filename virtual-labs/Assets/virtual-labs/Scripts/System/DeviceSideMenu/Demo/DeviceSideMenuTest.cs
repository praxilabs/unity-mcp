using UnityEngine;
using Praxilabs.DeviceSideMenu;
using UnityEngine.UI;
using TMPro;
public class DeviceSideMenuTest : MonoBehaviour 
{
    [SerializeField] private DeviceMenuWrapper sideMenuWrapper;
    [SerializeField] private GameObject powerSupplyV2_WithCameras_Prefab;
    [SerializeField] private GameObject anotherPrefab;
    [SerializeField] private KeyCodesDeviceNamePair keyCodeDeviceNamePair1;
    [SerializeField] private KeyCodesDeviceNamePair keyCodeDeviceNamePair2;
    [SerializeField] private KeyCodesDeviceNamePair keyCodeDeviceNamePair3;
    [SerializeField] private KeyCodesDeviceNamePair keyCodeDeviceNamePair4;
    [SerializeField] private KeyCodesDeviceNamePair keyCodeDeviceNamePair5;
    private void Update() {
        if(Input.GetKeyDown(keyCodeDeviceNamePair1.showKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair1.deviceName, true, TabType.Controls);
        }
        if(Input.GetKeyDown(keyCodeDeviceNamePair1.hideKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair1.deviceName, false);
        }
        if(Input.GetKeyDown(KeyCode.N))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair1.deviceName, true, TabType.SafetyProcedures);
        }
        if(Input.GetKeyDown(KeyCode.M))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair1.deviceName, true, TabType.Description);
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair1.deviceName, true, TabType.Readings);
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair1.deviceName, true);
        }
        ////////////////////////////////////////////////////////////////////////////////////////
        if(Input.GetKeyDown(keyCodeDeviceNamePair2.showKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair2.deviceName, true);
        }
        if(Input.GetKeyDown(keyCodeDeviceNamePair2.hideKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair2.deviceName, false);
        }
        ////////////////////////////////////////////////////////////////////////////////////////
        if(Input.GetKeyDown(keyCodeDeviceNamePair3.showKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair3.deviceName, true);
        }
        if(Input.GetKeyDown(keyCodeDeviceNamePair3.hideKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair3.deviceName, false);
        }
        ////////////////////////////////////////////////////////////////////////////////////////
        if(Input.GetKeyDown(keyCodeDeviceNamePair4.showKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair4.deviceName, true);
        }
        if(Input.GetKeyDown(keyCodeDeviceNamePair4.hideKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair4.deviceName, false);
        }
        ////////////////////////////////////////////////////////////////////////////////////////
        if(Input.GetKeyDown(keyCodeDeviceNamePair5.showKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair5.deviceName, true);
        }
        if(Input.GetKeyDown(keyCodeDeviceNamePair5.hideKey))
        {
            sideMenuWrapper.SetDeviceSideMenuVisibility(keyCodeDeviceNamePair5.deviceName, false);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(powerSupplyV2_WithCameras_Prefab);
            Instantiate(anotherPrefab);
        }
    }

    [System.Serializable]
    public class KeyCodesDeviceNamePair
    {
        public KeyCode showKey;
        public KeyCode hideKey;
        public string deviceName;
    }
}