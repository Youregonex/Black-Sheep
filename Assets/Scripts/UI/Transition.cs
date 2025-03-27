using UnityEngine;
using System.Collections;
using Youregone.SL;
using Youregone.YCamera;
using DG.Tweening;

namespace Youregone.UI
{
    public class Transition : MonoBehaviour, IService
    {
        [CustomHeader("Settings")]
        [SerializeField] private RectTransform _topAnchoredTransition;
        [SerializeField] private RectTransform _bottomAnchoredTransition;
        [SerializeField] private CameraGameStartSequence _camera;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _transitionDuration;

        private Tween _currentTween;
        private Coroutine _currentCoroutine;

        public IEnumerator PlayTransitionStart()
        {
            if (_currentCoroutine != null)
            {
                Debug.LogWarning($"Trying to start multiple coroutines!");
                yield break;
            }

            yield return _currentCoroutine = StartCoroutine(PlayTransitionStartCoroutine());

            _currentCoroutine = null;
        }

        public IEnumerator PlayTransitionEnd()
        {
            if(_currentCoroutine != null)
            {
                Debug.LogWarning($"Trying to start multiple coroutines!");
                yield break;
            }

            yield return _currentCoroutine = StartCoroutine(PlayTransitionEndCoroutine());

            _currentCoroutine = null;
        }

        private IEnumerator PlayTransitionStartCoroutine()
        {
            Vector2 startingYSize = _topAnchoredTransition.sizeDelta;
            startingYSize.y = 0f;
            _topAnchoredTransition.sizeDelta = startingYSize;

            _topAnchoredTransition.gameObject.SetActive(true);
            _bottomAnchoredTransition.gameObject.SetActive(false);

            Vector2 goalYSize = new(_topAnchoredTransition.sizeDelta.x, Screen.height);

            if(_currentTween != null)
                _currentTween.Kill();

            _currentTween = _topAnchoredTransition.DOSizeDelta(goalYSize, _transitionDuration);

            yield return _currentTween.WaitForCompletion();

            _currentTween = null;
        }

        private IEnumerator PlayTransitionEndCoroutine()
        {
            Vector2 startingYSize = _topAnchoredTransition.sizeDelta;
            startingYSize = new Vector2(Screen.width, Screen.height);
            _bottomAnchoredTransition.sizeDelta = startingYSize;

            _bottomAnchoredTransition.gameObject.SetActive(true);
            _topAnchoredTransition.gameObject.SetActive(false);

            Vector2 goalYSize = new(_topAnchoredTransition.sizeDelta.x, 0f);

            if (_currentTween != null)
                _currentTween.Kill();

            _currentTween = _bottomAnchoredTransition.DOSizeDelta(goalYSize, _transitionDuration);

            yield return _currentTween.WaitForCompletion();

            _currentTween = null;
            _bottomAnchoredTransition.gameObject.SetActive(false);
        }
    }
}