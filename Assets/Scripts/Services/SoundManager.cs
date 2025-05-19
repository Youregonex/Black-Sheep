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
        [SerializeField] private AudioClip _uiClick;
        [SerializeField] private List<AudioClip> _collectablePickupClipList;

        private BasicPool<AudioSourceSoundFX> _audioSourcePool;
        private Factory<AudioSourceSoundFX> _audioSourceFactory;

        private void Awake()
        {
            _audioSourceFactory = new();
            int initialPoolSize = 5;
            _audioSourcePool = new(_audioSourceFactory, _audioSourcePrefab, null, initialPoolSize);
        }

        public void PlaySoundFXClip(AudioClip audioClip, Vector3 position, float volume = 1f)
        {
            AudioSourceSoundFX audioSource = _audioSourcePool.Dequeue();
            audioSource.transform.position = position;

            audioSource.Initialize(audioClip, volume);
            audioSource.OnDestruction += AudioSourceSoundFX_OnDestruction;
        }

        private void AudioSourceSoundFX_OnDestruction(AudioSourceSoundFX audioSourceSoundFX)
        {
            audioSourceSoundFX.OnDestruction -= AudioSourceSoundFX_OnDestruction;
            _audioSourcePool.Enqueue(audioSourceSoundFX);
        }

        public void PlaySoundFXClip(List<AudioClip> audioClipList, Vector3 position, float volume = 1f)
        {
            int randomClipIndex = UnityEngine.Random.Range(0, audioClipList.Count);
            AudioClip audioClip = audioClipList[randomClipIndex];

            PlaySoundFXClip(audioClip, position, volume);
        }

        public void PlayWaterSplashClip(Vector3 position)
        {
            PlaySoundFXClip(_waterSpashClip, position, .1f);
        }

        public void PlayUIClick()
        {
            PlaySoundFXClip(_uiClick, Vector2.zero);
        }

        public void PlayCollectablePickupClip(Vector3 position)
        {
            PlaySoundFXClip(_collectablePickupClipList, position, 1f);
        }
    }
}