using UnityEngine;
using System.Collections;
using Youregone.SL;
using DG.Tweening;

namespace Youregone.UI
{
    public class Transition : MonoBehaviour, IService
    {
        [CustomHeader("Settings")]
        [SerializeField] private RectTransform _topAnchoredTransition;
        [SerializeField] private RectTransform _bottomAnchoredTransition;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private RectTransform _parentCanvas;

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
            Vector2 canvasSize = _parentCanvas.rect.size;

            Vector2 startingSize = new(canvasSize.x, 0f);
            _topAnchoredTransition.sizeDelta = startingSize;

            _topAnchoredTransition.gameObject.SetActive(true);
            _bottomAnchoredTransition.gameObject.SetActive(false);

            Vector2 goalSize = canvasSize;

            if (_currentTween != null)
                _currentTween.Kill();

            _currentTween = _topAnchoredTransition
                .DOSizeDelta(goalSize, _transitionDuration)
                .OnComplete(() => _currentTween = null);

            yield return _currentTween.WaitForCompletion();
        }

        private IEnumerator PlayTransitionEndCoroutine()
        {
            Vector2 canvasSize = _parentCanvas.rect.size;

            Vector2 startingSize = new(canvasSize.x, canvasSize.y);
            _bottomAnchoredTransition.sizeDelta = startingSize;

            _bottomAnchoredTransition.gameObject.SetActive(true);
            _topAnchoredTransition.gameObject.SetActive(false);

            Vector2 goalSize = new(canvasSize.x, 0f);

            if (_currentTween != null)
                _currentTween.Kill();

            _currentTween = _bottomAnchoredTransition
                .DOSizeDelta(goalSize, _transitionDuration)
                .OnComplete(() =>
                {
                    _currentTween = null;
                    _bottomAnchoredTransition.gameObject.SetActive(false);
                });

            yield return _currentTween.WaitForCompletion();
        }
    }
}