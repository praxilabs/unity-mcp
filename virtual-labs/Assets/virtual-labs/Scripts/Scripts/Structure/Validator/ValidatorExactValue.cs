using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Table.Structure.Validators
{
    [System.Serializable]
    public class ValidatorExactValue : TableFieldValidator
    {
        [SerializeField] private string _wantedValue;

        public override bool Validate(string value)
        {
            return value == _wantedValue;
        }
    }
}