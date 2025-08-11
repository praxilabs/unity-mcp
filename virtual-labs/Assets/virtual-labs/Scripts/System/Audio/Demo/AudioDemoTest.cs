using System.Collections;
using System.Collections.Generic;
using Praxilabs.Audio;
using UnityEngine;

public class AudioDemoTest : MonoBehaviour
{
    [SerializeField] private KeyCode _play;
    [SerializeField] private KeyCode _pause;
    [SerializeField] private KeyCode _restartFromBeginning;

    private BaseAudioFile audioFile;

    private void Awake()
    {
        audioFile = GetComponent<BaseAudioFile>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(_play))
        {
            audioFile.Play();
        }
        if(Input.GetKeyDown(_pause))
        {
            audioFile.Pause();
        }
        if(Input.GetKeyDown(_restartFromBeginning))
        {
            audioFile.RestartAudioFromBeginning();
        }
    }
}
