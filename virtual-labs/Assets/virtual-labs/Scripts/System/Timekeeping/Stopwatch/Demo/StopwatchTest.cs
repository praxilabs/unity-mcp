using System.Collections;
using System.Collections.Generic;
using Praxilabs.Timekeeping.Stopwatch;
using UnityEngine;

public class StopwatchTest : MonoBehaviour
{
    private StopwatchHandler _stopwatchHandler;

    private void Awake() 
    {
        _stopwatchHandler = FindObjectOfType<StopwatchHandler>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _stopwatchHandler.Setup(1.5f, new List<float> {1f, 1.5f, 2f}, false);
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            _stopwatchHandler.Setup(3f, new List<float> {1f, 3f, 4f}, true);
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            _stopwatchHandler.Setup(5f, new List<float> {1f, 5f, 30f}, true, false);
        }
        if(Input.GetKeyDown(KeyCode.Q))
        {
            _stopwatchHandler.ResetInteractability(true);
        }
        if(Input.GetKeyDown(KeyCode.W))
        {
            _stopwatchHandler.ResetInteractability(false);
        }
    }
}
