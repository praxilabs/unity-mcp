using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Praxilabs.Audio
{
    public class AudioUIHandler : MonoBehaviour
    {
        [Header("Master Control Settings")]
        [SerializeField] private VolumeControlData _masterVolCtrlData;
        [Header("Other Control Settings")]
        [SerializeField] private List<VolumeControlData> _volumeControlList;
        [Header("Elements")]
        [SerializeField] private Sprite _mutedSprite;
        [SerializeField] private Sprite _unmutedSprite;

        [Serializable]
        public class VolumeControlData
        {
            public AudioGenre audioGenre;
            public Slider slider;
            public Button muteBtn;
            public bool IsMuted {get; set;}
            public float SavedValue {get; set;}
        }

        private void Start() 
        {
            InitializeMasterControlUI();
            InitializeVolumeControlsUI();
        }

        private void InitializeMasterControlUI()
        {
            if(_masterVolCtrlData.slider is not null)
            {
                _masterVolCtrlData.slider.value = AudioManager.Instance.GetMasterVolume();

                _masterVolCtrlData.slider.onValueChanged.AddListener((value) => {
                    SetMasterVolume(value);
                });
            }

            if(_masterVolCtrlData.muteBtn is not null)
            {
                _masterVolCtrlData.muteBtn.onClick.AddListener(() => {
                    ToggleMuteButton(_masterVolCtrlData);
                });
            }
        }

        private void InitializeVolumeControlsUI()
        {
            foreach(VolumeControlData volCtrlData in _volumeControlList)
            {
                if(volCtrlData.slider is null) continue;

                volCtrlData.slider.value = AudioManager.Instance.GetVolume(volCtrlData.audioGenre);

                volCtrlData.slider.onValueChanged.AddListener((value) => {
                    SetVolume(volCtrlData, value);
                });

                if(volCtrlData.muteBtn is null) continue;

                volCtrlData.muteBtn.onClick.AddListener(() => {
                    ToggleMuteButton(volCtrlData);
                });
            }
        }

        private void SetVolume(VolumeControlData volCtrlData, float value)
        {
            AudioManager.Instance.SetVolume(volCtrlData.audioGenre, value);

            volCtrlData.IsMuted = value == 0; // Since it can be muted from both the slider and the mute button

            UpdateButtonSprite(volCtrlData);
        }

        private void SetMasterVolume(float value)
        {
            AudioManager.Instance.SetMasterVolume(value);

            if(value == 0)
            {
                _masterVolCtrlData.IsMuted = true;
            }
            else
            {
                if(_masterVolCtrlData.IsMuted == false) return;
                
                _masterVolCtrlData.IsMuted = false;
            }

            UpdateButtonSprite(_masterVolCtrlData);
        }

        private void ToggleMuteButton(VolumeControlData volCtrlData)
        {
            // Used for any individual button to toggle mute/unmute
            if(volCtrlData.IsMuted)
            {
                volCtrlData.slider.value = volCtrlData.SavedValue; // Will call SetVolume()
            }
            else
            {
                volCtrlData.SavedValue = volCtrlData.slider.value;
                volCtrlData.slider.value = 0; // Will call SetVolume()
            }
        }

        
        private void UpdateButtonSprite(VolumeControlData volCtrlData)
        {
            Image muteBtnImage = volCtrlData.muteBtn.GetComponent<Image>();

            if(volCtrlData.IsMuted)
            {
                muteBtnImage.sprite = _mutedSprite;
            }
            else
            {
                if(muteBtnImage.sprite == _unmutedSprite) return; // Case we changed from a value to another and unmuted sprite is already set

                muteBtnImage.sprite = _unmutedSprite;
            }
        }
    }
}
