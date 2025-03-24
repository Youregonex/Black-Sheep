using UnityEngine;
using Youregone.SL;

namespace Youregone.GameSystems
{
    public class SoundManager : MonoBehaviour, IService
    {
        [SerializeField] private bool _sound;
        [SerializeField] private AudioSource _audioSourceObjectPrefab;
        [SerializeField] private AudioSource _audioSourceFX;

        //public void PlaySoundAtPosition(AudioClip audioClip, float volume, Vector3 position)
        //{
        //    AudioSource audioSource = Instantiate(_audioSourceObjectPrefab, position, Quaternion.identity);
        //    audioSource.clip = audioClip;
        //    audioSource.volume = volume;
        //    audioSource.Play();

        //    float clipLength = audioSource.clip.length;

        //    Destroy(audioSource.gameObject, clipLength);
        //}

        public void PlaySoundAtPosition(AudioClip audioClip, float volume, Vector3 position)
        {
            if (!_sound)
                return;

            _audioSourceFX.transform.position = position;
            _audioSourceFX.clip = audioClip;
            _audioSourceFX.volume = volume;
            _audioSourceFX.Play();
        }
    }
}