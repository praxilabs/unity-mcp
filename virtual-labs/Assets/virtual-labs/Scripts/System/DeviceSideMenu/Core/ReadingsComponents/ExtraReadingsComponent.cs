using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class ExtraReadingsComponent : ReadingsComponent
    {
        [field: SerializeField] public List<string> ExtraComponentNames {get; set;}
        public override ReadingsComponentType GetReadingsComponentType()
        {
            return ReadingsComponentType.CameraView;
        }
    }
}