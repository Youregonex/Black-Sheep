using UnityEngine;
using DG.Tweening;
using System.Collections;
using System;

namespace Youregone.Camera
{
    public class CameraGameStartSequence : MonoBehaviour
    {
        public event Action OnCameraInPosition;

        [Header("Settings")]
        [SerializeField] private float _cameraSequenceDelay;
        [SerializeField] private float _cameraMoveDuration;
        [SerializeField] private float _cameraZOffset;
        [SerializeField] private Transform _cameraStartPoint;
        [SerializeField] private Transform _cameraEndPoint;
        [SerializeField] private Transform _cameraGamePoint;

        public Transform CameraStartPoint => _cameraStartPoint;
        public Transform CameraEndPoint => _cameraEndPoint;
        public Transform CameraGamePoint => _cameraGamePoint;


        public void MoveCamraToGamePoint()
        {
            transform.position = new Vector3(_cameraGamePoint.position.x, _cameraGamePoint.position.y, _cameraZOffset);
        }

        public void PlayCameraIntroSequence()
        {
            transform.position = new Vector3(_cameraStartPoint.position.x, _cameraStartPoint.position.y, _cameraZOffset);
            StartCoroutine(CameraIntroSequenceCoroutine(_cameraSequenceDelay));
        }

        private IEnumerator CameraIntroSequenceCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector3 cameraDestination = new(_cameraEndPoint.position.x, _cameraEndPoint.position.y, _cameraZOffset);
            transform.DOMove(cameraDestination, _cameraMoveDuration).OnComplete(() => OnCameraInPosition?.Invoke());
        }
    }
}