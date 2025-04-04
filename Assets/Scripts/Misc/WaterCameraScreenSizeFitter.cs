using UnityEngine;

namespace Youregone.Utils
{
    public class WaterCameraScreenSizeFitter : MonoBehaviour
    {
        private Camera _selfCamera;
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            _selfCamera = GetComponent<Camera>();
            MatchMainCameraWidth();
        }

        public void MatchMainCameraWidth()
        {
            if (_mainCamera == null || _selfCamera == null)
                return;

            float newOrthoSize = _mainCamera.orthographicSize * (_mainCamera.aspect / _selfCamera.aspect);
            _selfCamera.orthographicSize = newOrthoSize;
        }
    }
}