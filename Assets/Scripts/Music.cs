using UnityEngine;
using DG.Tweening;
using Youregone.SL;

namespace Youregone.SoundFX
{
    public class Music : MonoBehaviour, IService
    {
        [CustomHeader("Settings")]
        [SerializeField] private AudioClip _mainMusicClip;
        [SerializeField, Range(0f, 1f)] private float _musicTargetVolume;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _musicFadeInDuration = 5f;
        [SerializeField] private float _musicFadeOutDuration = .75f;

        private AudioSource _audioSource;
        private Tween _currentTween;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = _mainMusicClip;
        }

        private void OnDestroy()
        {
            _currentTween?.Kill();
        }

        public void StartMusic()
        {
            _audioSource.volume = _musicTargetVolume;
            _audioSource.Play();
        }

        public void StartMusicWithFadeIn()
        {
            _audioSource.volume = 0f;
            _audioSource.Play();

            FadeInMusic();
        }

        public void PauseMusic()
        {
            _audioSource.Pause();
        }

        public void ResumeMusic()
        {
            _audioSource.Play();
        }

        public void FadeOutMusic()
        {
            float volume = _musicTargetVolume;
            _currentTween = DOTween.To(
                () => volume,
                x => volume = x,
                0f,
                _musicFadeOutDuration)
                .OnUpdate(() =>
                {
                    if (_audioSource == null)
                        _currentTween.Kill();
                })
                .OnComplete(() => _currentTween = null)
                .OnUpdate(() =>
                {
                    if (_audioSource != null)
                        _audioSource.volume = volume;
                });
        }

        public void FadeInMusic()
        {
            float volume = 0f;
            _currentTween = DOTween
                .To(
                    () => volume,
                    x => volume = x,
                    _musicTargetVolume,
                    _musicFadeInDuration)
                .OnComplete(() => _currentTween = null)
                .OnUpdate(() =>
                {
                    if (_audioSource != null)
                        _audioSource.volume = volume;
                });
        }
    }
}
