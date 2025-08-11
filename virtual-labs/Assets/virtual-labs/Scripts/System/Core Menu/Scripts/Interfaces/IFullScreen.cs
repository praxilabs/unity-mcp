using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MenusBehaviors
{
    public interface IFullScreen
    {
        bool IsFullScreen { get; set;}
        public void ActivateFullScreen();
    }

}   
