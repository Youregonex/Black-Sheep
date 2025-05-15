using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Youregone.SaveSystem;
using Youregone.SL;

namespace Youregone.UI
{
    public class SettingsMenu : MonoBehaviour
    {
        private const string AUDIO_MIXER_EXPOSED_PARAMETER_MASTER_VOLUME = "masterVolume";
        private const string AUDIO_MIXER_EXPOSED_PARAMETER_SOUNDFX_VOLUME = "soundFXVolume";
        private const string AUDIO_MIXER_EXPOSED_PARAMETER_MUSIC_VOLUME = "musicVolume";

        [CustomHeader("Settings")]
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private CanvasGroup _windowCanvasGroup;
        [SerializeField] private Toggle _showOutroToggle;

        [CustomHeader("Sliders")]
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _soundFXSlider;

        private PlayerPrefsSaverLoader _playerPrefsSaverLoader;
        private bool _isOpened = false;

        public bool IsOpened => _isOpened;

        private void Awake()
        {
            _masterVolumeSlider.onValueChanged.AddListener(level =>
            {
                SetMasterVolume(level);
                _playerPrefsSaverLoader.SaveMasterVolume(level);
            });

            _musicVolumeSlider.onValueChanged.AddListener(level =>
            {
                SetMusicVolume(level);
                _playerPrefsSaverLoader.SaveMusicVolume(level);
            });

            _soundFXSlider.onValueChanged.AddListener(level =>
            {
                SetSoundFXVolume(level);
                _playerPrefsSaverLoader.SaveSoundFXVolume(level);
            });

            _showOutroToggle.onValueChanged.AddListener(enabled =>
            {
                _playerPrefsSaverLoader.ToggleOutroEnable(enabled);
            });
        }

        private void Start()
        {
            _playerPrefsSaverLoader = ServiceLocator.Get<PlayerPrefsSaverLoader>();

            _masterVolumeSlider.value = _playerPrefsSaverLoader.GetSavedMasterVolume();
            _musicVolumeSlider.value = _playerPrefsSaverLoader.GetSavedMusicVolume();
            _soundFXSlider.value = _playerPrefsSaverLoader.GetSavedSoundFXVolume();
            _showOutroToggle.isOn = _playerPrefsSaverLoader.OutroEnabled;

            SetMasterVolume(_playerPrefsSaverLoader.GetSavedMasterVolume());
            SetMusicVolume(_playerPrefsSaverLoader.GetSavedMusicVolume());
            SetSoundFXVolume(_playerPrefsSaverLoader.GetSavedSoundFXVolume());
        }

        private void SetMasterVolume(float level)
        {
            _audioMixer.SetFloat(AUDIO_MIXER_EXPOSED_PARAMETER_MASTER_VOLUME, Mathf.Log10(level) * 20f);
        }

        private void SetMusicVolume(float level)
        {
            _audioMixer.SetFloat(AUDIO_MIXER_EXPOSED_PARAMETER_MUSIC_VOLUME, Mathf.Log10(level) * 20f);
        }

        private void SetSoundFXVolume(float level)
        {
            _audioMixer.SetFloat(AUDIO_MIXER_EXPOSED_PARAMETER_SOUNDFX_VOLUME, Mathf.Log10(level) * 20f);
        }

        public void ShowWindow()
        {
            _windowCanvasGroup.gameObject.SetActive(true);
            _isOpened = true;
        }

        public void HideWindow()
        {
            _windowCanvasGroup.gameObject.SetActive(false);
            _isOpened = false;
        }
    }
}
