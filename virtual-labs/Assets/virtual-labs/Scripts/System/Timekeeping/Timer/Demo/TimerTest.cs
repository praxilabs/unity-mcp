using System;
using System.Collections;
using System.Collections.Generic;
using Praxilabs.Timekeeping.Timer;
using UnityEngine;

public class TimerTest : MonoBehaviour
{
    [SerializeField] private List<float> testSpeedFactors = new() {1f, 2f, 5f};
    private TimerHandler _timerHandler;

    private void Awake() 
    {
        _timerHandler = FindObjectOfType<TimerHandler>(); 
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _timerHandler.UpdateUI("");
            _timerHandler.Setup(1f, testSpeedFactors);
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            _timerHandler.UpdateUI("Microscope");
            _timerHandler.Setup(1f, testSpeedFactors, new TimeSpan(0, 1, 30));
            _timerHandler.PlayInstantly();
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            _timerHandler.UpdateUI("Telescope");
            _timerHandler.Setup(1f, testSpeedFactors, new TimeSpan(0, 1, 30), true);
            _timerHandler.PlayInstantly();
        }
        if(Input.GetKeyDown(KeyCode.G))
        {
            _timerHandler.UpdateUI("Scanner");
            _timerHandler.Setup(1f, testSpeedFactors, new TimeSpan(0, 1, 30), false);
            _timerHandler.PlayInstantly();
        }
    }
}
