using UnityEngine;

namespace Youregone.GameSystems
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance;

        [SerializeField] private bool _sound;
        [SerializeField] private AudioSource _audioSourceObjectPrefab;
        [SerializeField] private AudioSource _audioSourceFX;

        private void Awake()
        {
            instance = this;
        }

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