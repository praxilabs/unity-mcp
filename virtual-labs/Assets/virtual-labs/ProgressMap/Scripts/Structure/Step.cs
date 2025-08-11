using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProgressMap.Structure
{
    [System.Serializable]
    public class Step
    {
        #region fields
        //[SerializeField] protected string _idName;
        #endregion

        #region properties
        [field: SerializeField, UnityEngine.Serialization.FormerlySerializedAs("_idName")]
        public string IdName { get; set; }
        /// <summary>
        /// Sets weight for step, normaly step weight should be 1
        /// </summary>
        [field: SerializeField] public float Weight { get; protected set; } = 1;
        #endregion
    }
}
