using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class CameraViewReadingsComponent : ExtraReadingsComponent
    {
        public override ReadingsComponentType GetReadingsComponentType()
        {
            return ReadingsComponentType.CameraView;
        }
    }
}