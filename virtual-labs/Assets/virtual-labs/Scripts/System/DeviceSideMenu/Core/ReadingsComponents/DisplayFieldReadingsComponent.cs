using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.DeviceSideMenu
{
    public class DisplayFieldReadingsComponent : ReadingsComponent
    {
        [field: SerializeField] public string Label {get; set;}
        [field: SerializeField] public string DisplayTextSign {get; set;}

        public override ReadingsComponentType GetReadingsComponentType()
        {
            return ReadingsComponentType.DisplayField;
        }
    }
}