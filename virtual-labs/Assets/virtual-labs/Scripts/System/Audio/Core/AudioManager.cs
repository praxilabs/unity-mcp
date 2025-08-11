using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Praxilabs.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        private Dictionary<AudioGenre, float> _audioGenreVolumes = new Dictionary<AudioGenre, float>();
        // Stores audio sources for each audio genre in order to be able to control the whole genre volume at any given time
        private Dictionary<AudioGenre, List<AudioSourceController>> _activeAudioSourceControllers = new Dictionary<AudioGenre, List<AudioSourceController>>();

        private float _masterVolume = 1f;

        protected override void Initialize()
        {
            InitializeVolumeControls();
        }

        private void InitializeVolumeControls()
        {
            foreach(AudioGenre genre in Enum.GetValues(typeof(AudioGenre)))
            {
                if(genre == AudioGenre.Master) continue;
                
                if(!_audioGenreVolumes.ContainsKey(genre))
                {
                    _audioGenreVolumes[genre] = 1f; // default value for now
                    _activeAudioSourceControllers[genre] = new List<AudioSourceController>(); // initialize the list of audio sources for such a type
                }
            }
        }

        public void SetVolume(AudioGenre audioGenre, float volume)
        {
            if(_audioGenreVolumes.ContainsKey(audioGenre))
            {
                _audioGenreVolumes[audioGenre] = volume;
                UpdateActiveSourcesVolume(audioGenre);
            }
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = volume;
            UpdateAllActiveSourcesVolume();
        }

        public float GetVolume(AudioGenre audioGenre)
        {
            return _audioGenreVolumes[audioGenre];
        }

        public float GetMasterVolume()
        {
            return _masterVolume;
        }

        public void RegisterActiveSource(AudioGenre audioGenre, AudioSourceController audioSrcController)
        {
            if(!_activeAudioSourceControllers[audioGenre].Contains(audioSrcController))
            {
                _activeAudioSourceControllers[audioGenre].Add(audioSrcController);
            }
        }

        public void UnregisterActiveSource(AudioGenre audioGenre, AudioSourceController audioSrcController)
        {
            if(_activeAudioSourceControllers[audioGenre].Contains(audioSrcController))
            {
                _activeAudioSourceControllers[audioGenre].Remove(audioSrcController);
            }
        }

        public AudioSourceController PlayAudio(AudioFileData audioFileData, Vector3 position)
        {
            GameObject audioObject = new GameObject($"AudioSource_{audioFileData.audioName}");
            audioObject.transform.position = position;
            return PlayAudio(audioObject, audioFileData);
        }

        private AudioSourceController PlayAudio(GameObject audioObject, AudioFileData audioFileData)
        {
            AudioSourceController audioController = audioObject.AddComponent<AudioSourceController>();

            float genreVolume = _audioGenreVolumes[audioFileData.genre];
            audioController.PlayAudio(audioFileData, genreVolume, _masterVolume);

            return audioController;
        }

        private void UpdateActiveSourcesVolume(AudioGenre audioGenre)
        {
            foreach(AudioSourceController audioSrcController in _activeAudioSourceControllers[audioGenre])
            {
                // audioSrcController.UpdateVolume(audioGenreVolumes[audioGenre], masterVolume);
                UpdateAudioSourceController(audioSrcController, _audioGenreVolumes[audioGenre], _masterVolume);
            }
        }

        private void UpdateAllActiveSourcesVolume()
        {
            foreach(KeyValuePair<AudioGenre, List<AudioSourceController>> audioGenreEntry in _activeAudioSourceControllers)
            {
                UpdateActiveSourcesVolume(audioGenreEntry.Key);
            }
        }

        private void UpdateAudioSourceController(AudioSourceController audioSrcController, float genreVolume, float masterVolume)
        {
            if(genreVolume == 0 || masterVolume == 0)
            {
                audioSrcController.Pause(true);
            }
            else
            {
                audioSrcController.Resume(true);
            }

            audioSrcController.UpdateVolume(genreVolume, masterVolume);
        }
    }
}
