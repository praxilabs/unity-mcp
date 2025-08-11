using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.Audio
{
    public class AudioSourceController : MonoBehaviour
    {
        private AudioSource _audioSrc;
        private AudioFileData _audioFileData;
        private bool _isPausedByAudioFile;
        private bool _isPausedByManager;

        private void Awake() 
        {
            _audioSrc = gameObject.AddComponent<AudioSource>();
        }

        public void PlayAudio(AudioFileData audioFileData, float genreVolume, float masterVolume)
        {
            _audioFileData = audioFileData;

            _audioSrc.clip = audioFileData.clip;
            _audioSrc.volume = audioFileData.volume * genreVolume * masterVolume;
            _audioSrc.loop = audioFileData.loop;

            if(audioFileData.isSpatialBlend)
                _audioSrc.spatialBlend = audioFileData.spatialBlend;
                        
            Play();

            HandleAudioPlayback();
        }

        public void UpdateVolume(float genreVolume, float masterVolume)
        {
            _audioSrc.volume = _audioFileData.volume * genreVolume * masterVolume;
        }

        public void Pause(bool isPausedByManager = false)
        {
            if(isPausedByManager)
            {
                _isPausedByManager = true;
            }
            else
            {
                _isPausedByAudioFile = true;
            }
            
            PauseImmediately();
        }

        public void Resume(bool isResumedByManager = false)
        {
            if(isResumedByManager)
            {
                _isPausedByManager = false;
            }
            else
            {
                _isPausedByAudioFile = false;
            }
            
            CheckAndResumeAudio();
        }

        public void RestartAudioFromBeginning()
        {
            _isPausedByAudioFile = false;
            Play();
            if(_isPausedByManager)
                PauseImmediately();
        }

        private void CheckAndResumeAudio()
        {
            // Resume the audio only if neither the manager nor the audio file are pausing it
            if (!_isPausedByManager && !_isPausedByAudioFile)
            {
                ResumeImmediately();
            }
        }

        private void Play()
        {
            _audioSrc.time = 0;
            _audioSrc.Play();
        }

        private void PauseImmediately()
        {
            _audioSrc.Pause();
        }

        private void ResumeImmediately()
        {
            _audioSrc.UnPause();
        }

        private void HandleAudioPlayback()
        {
            AudioManager.Instance.RegisterActiveSource(_audioFileData.genre, this);

            // Don't destroy unless the audio source won't loop and it is not resuable
            if(!_audioSrc.loop && !_audioFileData.isResuable)
            {
                StartCoroutine(HandleAudioDestroy());
            }
        }

        private IEnumerator HandleAudioDestroy()
        {
            yield return new WaitUntil(() => !_audioSrc.isPlaying && _audioSrc.time >= _audioSrc.clip.length);

            Destroy(gameObject);
        }

        private void OnDestroy() 
        {
            AudioManager.Instance.UnregisterActiveSource(_audioFileData.genre, this);
        }
    }
}