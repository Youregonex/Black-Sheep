using System.Collections.Generic;
using UnityEngine;
using Youregone.SL;

namespace Youregone.GameSystems
{
    public class SoundManager : MonoBehaviour, IService
    {
        [CustomHeader("Settings")]
        [SerializeField] private AudioSource _audioSourcePrefab;
        [SerializeField] private AudioClip _waterSpashClip;

        public void PlaySoundFXClip(AudioClip audioClip, Vector3 position, float volume = 1f)
        {
            AudioSource audioSource = Instantiate(_audioSourcePrefab, position, Quaternion.identity);
            audioSource.clip = audioClip;
            audioSource.volume = volume;
            float clipLength = audioSource.clip.length;
            audioSource.Play();
            Destroy(audioSource.gameObject, clipLength);
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
    }
}