using System.Collections.Generic;
using UnityEngine;
using Youregone.Factories;
using Youregone.ObjectPooling;
using Youregone.SL;
using Youregone.SoundFX;

namespace Youregone.GameSystems
{
    public class SoundManager : MonoBehaviour, IService
    {
        [CustomHeader("Settings")]
        [SerializeField] private AudioSourceSoundFX _audioSourcePrefab;
        [SerializeField] private AudioClip _waterSpashClip;
        [SerializeField] private AudioClip _playerDrownClip;
        [SerializeField] private AudioClip _playerDamagedClip;
        [SerializeField] private AudioClip _playerJumpClip;
        [SerializeField] private AudioClip _rockBreakClip;
        [SerializeField] private AudioClip _uiClickClip;
        [SerializeField] private List<AudioClip> _collectablePickupClipList;
        [SerializeField] private List<AudioClip> _sheepClipList;

        private BasicPool<AudioSourceSoundFX> _audioSourcePool;
        private Factory<AudioSourceSoundFX> _audioSourceFactory;

        private void Awake()
        {
            _audioSourceFactory = new();
            int initialPoolSize = 5;
            _audioSourcePool = new(_audioSourceFactory, _audioSourcePrefab, null, initialPoolSize);
        }

        public void PlaySoundFXClip(AudioClip audioClip, Vector3 position, bool proximityVolume, float volume = 1f)
        {
            AudioSourceSoundFX audioSource = _audioSourcePool.Dequeue();
            audioSource.transform.position = position;

            audioSource.Initialize(audioClip, volume, proximityVolume);
            audioSource.OnDestruction += AudioSourceSoundFX_OnDestruction;
        }

        private void AudioSourceSoundFX_OnDestruction(AudioSourceSoundFX audioSourceSoundFX)
        {
            audioSourceSoundFX.OnDestruction -= AudioSourceSoundFX_OnDestruction;
            _audioSourcePool.Enqueue(audioSourceSoundFX);
        }

        public void PlaySoundFXClip(List<AudioClip> audioClipList, Vector3 position, bool proximityVolume, float volume = 1f)
        {
            int randomClipIndex = UnityEngine.Random.Range(0, audioClipList.Count);
            AudioClip audioClip = audioClipList[randomClipIndex];

            PlaySoundFXClip(audioClip, position, proximityVolume, volume);
        }

        public void PlayWaterSplashClip(Vector3 position)
        {
            PlaySoundFXClip(_waterSpashClip, position, true, 1f);
        }

        public void PlayUIClick()
        {
            PlaySoundFXClip(_uiClickClip, Vector2.zero, false);
        }

        public void PlayCollectablePickupClip(Vector3 position)
        {
            PlaySoundFXClip(_collectablePickupClipList, position, false, 1f);
        }

        public void PlayPlayerDrownClip(Vector3 position)
        {
            PlaySoundFXClip(_playerDrownClip, position, false, .2f);
        }

        public void PlaySheepSound(Vector3 position)
        {
            PlaySoundFXClip(_sheepClipList, position, true, .9f);
        }

        public void PlayPlayerDamagedClip(Vector3 position)
        {
            PlaySoundFXClip(_playerDamagedClip, position, false, 1f);
        }

        public void PlayRockBreakClip(Vector3 position)
        {
            PlaySoundFXClip(_rockBreakClip, position, false);
        }

        public void PlayPlayerJumpClip(Vector3 position)
        {
            PlaySoundFXClip(_playerJumpClip, position, false, .3f);
        }
    }
}