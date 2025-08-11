using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DeviceScreenReadingsComponent : ReadingsComponent
    {
        public override ReadingsComponentType GetReadingsComponentType()
        {
            return ReadingsComponentType.DeviceScreen;
        }
    }
}