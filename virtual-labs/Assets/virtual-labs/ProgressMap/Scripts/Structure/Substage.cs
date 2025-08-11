using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Substage
{
    [field: SerializeField]public int StepNumber { get; set; }
    [field: SerializeField]public string SubstageName { get; set; }
}
