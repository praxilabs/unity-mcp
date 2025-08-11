using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Table.Structure.Validators
{
    [System.Serializable]
    public class ValidatorRange : TableFieldValidator
    {
        [SerializeField] private float _minValue = float.MinValue;
        [SerializeField] private float _maxValue = float.MaxValue;

        public override bool Validate(string value)
        {
            float result;
            if(float.TryParse(value, out result))
            {
                return _minValue <= result && result <= _maxValue;
            }
            return false;
        }
    }
}
