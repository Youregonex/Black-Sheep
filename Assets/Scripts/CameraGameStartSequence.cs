using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

namespace Youregone.Camera
{
    public class CameraGameStartSequence : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private bool _testMode;
        [SerializeField] private float _cameraSequenceDelay;
        [SerializeField] private float _cameraMoveDuration;
        [SerializeField] private float _cameraZOffset;
        [SerializeField] private Transform _cameraStartPoint;
        [SerializeField] private Transform _cameraEndPoint;
        [SerializeField] private Transform _cameraGamePoint;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private float _buttonFadeTime;

        public Transform CameraStartPoint => _cameraStartPoint;
        public Transform CameraEndPoint => _cameraEndPoint;
        public Transform CameraGamePoint => _cameraGamePoint;

        public void StartCameraSequence()
        {
            transform.position = new Vector3(_cameraStartPoint.position.x, _cameraStartPoint.position.y, _cameraZOffset);
            StartCoroutine(CameraSequeceDelay(_cameraSequenceDelay));
        }

        public void StartGame()
        {
            transform.position = new Vector3(_cameraGamePoint.position.x, _cameraGamePoint.position.y, _cameraZOffset);
        }

        private IEnumerator CameraSequeceDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            GameStartSequence();
        }

        private void GameStartSequence()
        {
            Vector3 cameraDestination = new(_cameraEndPoint.position.x, _cameraEndPoint.position.y, _cameraZOffset);

            _playButton.interactable = false;
            _exitButton.interactable = false;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(cameraDestination, _cameraMoveDuration));
            sequence.Append(_playButton.image.DOFade(1f, _buttonFadeTime));
            sequence.Append(_exitButton.image.DOFade(1f, _buttonFadeTime));
            sequence.OnComplete(() =>
            {
                _playButton.interactable = true;
                _exitButton.interactable = true;

            });
            sequence.Play();
        }
    }
}