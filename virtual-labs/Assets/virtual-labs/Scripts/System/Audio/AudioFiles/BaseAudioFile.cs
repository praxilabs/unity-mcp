using UnityEngine;

namespace Praxilabs.Audio
{
    public abstract class BaseAudioFile : MonoBehaviour 
    {
        [field: SerializeField, Space] public AudioClip AudioClip {get; private set;}
        [field: SerializeField, Space] public bool IsResuable {get; private set;}
        [field: SerializeField, Range(0,1), Header("Audio Settings")] public float Volume {get; private set;} = 1f;
        [field: SerializeField] public bool IsSpatialBlend {get; private set;}
        [field: SerializeField, Range(0,1)] public float SpatialBlend {get; private set;}
        [field: SerializeField] public bool Loop {get; private set;}

        public abstract AudioGenre AudioGenre {get;}

        protected AudioFileData _audioFileData = new AudioFileData();

        protected AudioSourceController _audioSrcController;

        protected virtual void Setup()
        {
            _audioFileData.genre = AudioGenre;
            _audioFileData.clip = AudioClip;
            _audioFileData.volume = Volume;
            _audioFileData.isSpatialBlend = IsSpatialBlend;
            _audioFileData.spatialBlend = SpatialBlend;
            _audioFileData.loop = Loop;
            _audioFileData.audioName = gameObject.name;
            _audioFileData.isResuable = IsResuable;
        }

        public void Play()
        {
            if(_audioSrcController != null)
            {
                _audioSrcController.Resume();
                return;
            }

            Setup();

            _audioSrcController = AudioManager.Instance.PlayAudio(_audioFileData, transform.position);
        }

        public void Pause()
        {
            if(_audioSrcController != null)
                _audioSrcController.Pause();
        }

        public void RestartAudioFromBeginning()
        {
            if(_audioSrcController != null)
                _audioSrcController.RestartAudioFromBeginning();
        }
    }


    public struct AudioFileData
    {
        public AudioClip clip;
        public AudioGenre genre;
        public bool isSpatialBlend;
        public float spatialBlend;
        public float volume;
        public string audioName;
        public bool loop;
        public bool isResuable;
    }
}


