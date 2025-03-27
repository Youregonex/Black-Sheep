using UnityEngine;
using Cinemachine;
using Youregone.YPlayerController;
using Youregone.SL;
using DG.Tweening;

namespace Youregone.YCamera
{
    public class CInemachineShake : MonoBehaviour
    {
        [CustomHeader("Shake Settings")]
        [SerializeField] private float _shakeDuration;
        [SerializeField] private float _intensity;
        
        private CinemachineVirtualCamera _camera;
        private Tween _currentTween;

        private void Awake()
        {
            _camera = GetComponent<CinemachineVirtualCamera>();
        }

        private void Start()
        {
            ServiceLocator.Get<PlayerController>().OnDamageTaken += ShakeCamera;
            ServiceLocator.Get<PlayerController>().OnObstacleDestroyed += ShakeCamera;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().OnDamageTaken -= ShakeCamera;
            ServiceLocator.Get<PlayerController>().OnObstacleDestroyed -= ShakeCamera;
        }

        private void ShakeCamera()
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