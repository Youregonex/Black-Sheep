using UnityEngine;
using DG.Tweening;
using Youregone.PlayerControls;
using Youregone.SL;
using Cinemachine;

namespace Youregone.Cam
{
    public class GameCamera : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private float _startSize;
        [SerializeField] private float _ramSize;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _sizeChangeDuration;

        private CinemachineVirtualCamera _virtualCamera;
        private Tween _currentTween;

        private void Start()
        {
            _virtualCamera = transform.GetComponent<CinemachineVirtualCamera>();
            _startSize = _virtualCamera.m_Lens.OrthographicSize;
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

            _currentTween = DOTween.To(
                () => _virtualCamera.m_Lens.OrthographicSize,
                x => _virtualCamera.m_Lens.OrthographicSize = x,
                _startSize,
                _sizeChangeDuration);
        }

        private void PlayerController_OnRamStart()
        {
            if (_currentTween != null)
                _currentTween.Kill();

            _currentTween = DOTween.To(
                () => _virtualCamera.m_Lens.OrthographicSize,
                x => _virtualCamera.m_Lens.OrthographicSize = x,
                _ramSize,
                _sizeChangeDuration);
        }
    }
}
