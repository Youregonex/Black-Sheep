using UnityEngine;
using Youregone.YPlayerController;
using Youregone.SL;
using DG.Tweening;

namespace Youregone.YCamera
{
    public class CInemachineShake : MonoBehaviour
    {
        [CustomHeader("Shake Settings")]
        [SerializeField] private float _shakeDuration;
        [SerializeField, Range(0f, 1f)] private float _intensity;
        
        private Tween _currentTween;

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

        public void ShakeCamera()
        {
            if (_currentTween != null)
                _currentTween.Kill();

            _currentTween = transform.DOShakePosition(_shakeDuration, _intensity).OnComplete(() => _currentTween = null);
        }
    }
}