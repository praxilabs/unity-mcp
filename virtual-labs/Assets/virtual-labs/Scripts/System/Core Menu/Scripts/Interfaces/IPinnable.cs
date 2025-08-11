using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MenusBehaviors
{
    public interface IPinnable
    {
        bool IsPinned { get; set;}
        public void Pin();
        public void Unpin();
    }
}
