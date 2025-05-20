using UnityEngine;
using System;
using System.Collections;

namespace Youregone.SoundFX
{
    public class AudioSourceSoundFX : MonoBehaviour
    {
        public event Action<AudioSourceSoundFX> OnDestruction;

        private AudioSource _audioSource;

        private Coroutine _currentCoroutine;


        public void Initialize(AudioClip clip, float volume, bool proximityVolume = false)
        {
            _audioSource.clip = clip;
            _audioSource.volume = volume;
            float clipLength = _audioSource.clip.length;
            _audioSource.spatialBlend = proximityVolume ? 1f : 0f;
            _audioSource.Play();

            _currentCoroutine = StartCoroutine(DelayedDestructionCoroutine(clipLength));
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnDisable()
        {
            _audioSource.Stop();
        }

        private void OnDestroy()
        {
            if (_currentCoroutine != null)
                StopAllCoroutines();
        }

        private IEnumerator DelayedDestructionCoroutine(float clipLength)
        {
            yield return new WaitForSeconds(clipLength);

            _currentCoroutine = null;
            OnDestruction?.Invoke(this);
        }
    }
}
