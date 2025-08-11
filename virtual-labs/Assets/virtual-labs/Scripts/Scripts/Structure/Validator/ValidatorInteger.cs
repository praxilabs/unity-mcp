using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Table.Structure.Validators
{
    [System.Serializable]
    public class ValidatorInteger : TableFieldValidator
    {
        public override bool Validate(string value)
        {
            int result;
            if (int.TryParse(value, out result))
                return true;
            return false;
        }
    }
}