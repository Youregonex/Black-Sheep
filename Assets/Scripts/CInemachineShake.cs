using UnityEngine;
using Cinemachine;
using System.Collections;
using Youregone.PlayerControls;
using Youregone.SL;
using DG.Tweening;

namespace Youregone.Cam
{
    public class CInemachineShake : MonoBehaviour
    {
        [CustomHeader("Shake Settings")]
        [SerializeField] private float _shakeDuration;
        [SerializeField] private float _intensity;
        
        private CinemachineVirtualCamera _camera;
        private Coroutine _currentCoroutine;
        private Tween _currentTween;

        private void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            ServiceLocator.Get<PlayerController>().OnDamageTaken += PlayerController_OnDamageTaken;       
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().OnDamageTaken -= PlayerController_OnDamageTaken;
        }

        private void PlayerController_OnDamageTaken()
        {
            //if (_currentCoroutine != null)
            //    StopCoroutine(_currentCoroutine);

            //_currentCoroutine = StartCoroutine(ShakeCamera());
            ShakeCam();
        }

        private IEnumerator ShakeCamera()
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                _camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = _intensity;

            yield return new WaitForSeconds(_shakeDuration);

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            _currentCoroutine = null;
        }

        private void ShakeCam()
        {
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                _camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = _intensity;

            if (_currentTween != null)
                _currentTween.Kill();

            _currentTween = DOTween.To(
                () => cinemachineBasicMultiChannelPerlin.m_AmplitudeGain,
                x => cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = x,
                0f,
                _shakeDuration).OnComplete(() => _currentTween = null);
        }
    }
}