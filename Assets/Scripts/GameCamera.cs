using UnityEngine;
using DG.Tweening;
using Youregone.PlayerControls;
using Youregone.SL;

namespace Youregone.Cam
{
    public class GameCamera : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private float _startSize;
        [SerializeField] private float _ramSize;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _sizeChangeDuration;

        private Camera _mainCamera;
        private Tween _currentTween;

        private void Start()
        {
            _mainCamera = transform.GetComponent<Camera>();
            _startSize = _mainCamera.orthographicSize;
            ServiceLocator.Get<PlayerController>().OnRamStart += PlayerController_OnRamStart;
            ServiceLocator.Get<PlayerController>().OnRamStop += PlayerController_OnRamStop; ;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().OnRamStart -= PlayerController_OnRamStart;
            ServiceLocator.Get<PlayerController>().OnRamStop -= PlayerController_OnRamStop; ;
        }

        private void PlayerController_OnRamStop()
        {
            if(_currentTween != null)
                _currentTween.Kill();

            _currentTween = _mainCamera.DOOrthoSize(_startSize, _sizeChangeDuration).OnComplete(() => _currentTween = null);
        }

        private void PlayerController_OnRamStart()
        {
            if (_currentTween != null)
                _currentTween.Kill();

            _currentTween = _mainCamera.DOOrthoSize(_ramSize, _sizeChangeDuration).OnComplete(() => _currentTween = null);
        }
    }
}
