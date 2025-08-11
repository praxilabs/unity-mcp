using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Table.Structure.Validators
{
    [System.Serializable]
    public abstract class TableFieldValidator : UnityEngine.MonoBehaviour
    {
        public abstract bool Validate(string value);
    }
}