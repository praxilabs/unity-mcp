using System;
using System.Collections.Generic;

namespace Praxilabs.DeviceSideMenu
{
    [Serializable]
    public class AllDevicesData
    {
        public List<DeviceData> devices = new List<DeviceData>();
    }
}

[Serializable]
public class DeviceData
{
    public string deviceID;
    public string deviceName;
    public string safetyProceduresTitle;
    public string safetyProceduresBody;
    public string descriptionTitle;
    public string descriptionBody;
    public List<DeviceReadingData> readings = new List<DeviceReadingData>();
    public List<DeviceControlData> controls = new List<DeviceControlData>();
}

[Serializable]
public class DeviceReadingData
{
    public string readingID;
    public string deviceName;
    public string name;
    public string label;
    public string displayText;
    public string displayTextSign;
}

[Serializable]
public class DeviceControlData
{ 
    public string name;
    public string componentLabel;
}