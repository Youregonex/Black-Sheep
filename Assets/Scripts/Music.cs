using UnityEngine;
using DG.Tweening;
using Youregone.SL;

namespace Youregone.SoundFX
{
    public class Music : MonoBehaviour, IService
    {
        [CustomHeader("Settings")]
        [SerializeField] private AudioClip _mainMusicClip;

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
            _audioSource.volume = 1f;
            _audioSource.Play();
        }

        public void StartMusicWithFadeIn()
        {
            _audioSource.volume = 0f;
            _audioSource.Play();

            FadeInMusic();
        }

        public void FadeOutMusic()
        {
            _currentTween = DOTween.To(
                () => _audioSource.volume,
                x => _audioSource.volume = x,
                0f,
                _musicFadeOutDuration)
                .OnUpdate(() =>
                {
                    if (_audioSource == null)
                        _currentTween.Kill();
                })
                .OnComplete(() => _currentTween = null);
        }

        public void FadeInMusic(float targetVolume = 1f)
        {
            _currentTween = DOTween
                .To(
                    () => _audioSource.volume,
                    x => _audioSource.volume = x,
                    targetVolume,
                    _musicFadeInDuration)
                .OnComplete(() => _currentTween = null);
        }
    }
}
