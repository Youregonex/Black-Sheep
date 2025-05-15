using System.Collections.Generic;
using UnityEngine;
using Youregone.SL;

namespace Youregone.GameSystems
{
    public class SoundManager : MonoBehaviour, IService
    {
        [CustomHeader("Settings")]
        [SerializeField] private bool _soundEnabled;
        [SerializeField] private AudioSource _audioSourcePrefab;

        public void PlaySoundFXClip(AudioClip audioClip, Vector3 position, float volume)
        {
            if (!_soundEnabled)
                return;

            AudioSource audioSource = Instantiate(_audioSourcePrefab, position, Quaternion.identity);
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            float clipLength = audioSource.clip.length;
            audioSource.Play();
            Destroy(audioSource.gameObject, clipLength);
        }

        public void PlaySoundFXClip(List<AudioClip> audioClipList, Vector3 position, float volume)
        {
            if (!_soundEnabled)
                return;

            int randomClipIndex = UnityEngine.Random.Range(0, audioClipList.Count);
            AudioClip audioClip = audioClipList[randomClipIndex];

            PlaySoundFXClip(audioClip, position, volume);
        }
    }
}