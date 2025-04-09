using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using DG.Tweening;

namespace Youregone.UI
{
    public class ShopUI : MonoBehaviour
    {
        [CustomHeader("Skin Settings")]
        [SerializeField] private List<Sprite> _skinsList;

        [CustomHeader("Other")]
        [SerializeField] private CanvasGroup _selfCanvasGroup;
        [SerializeField] private SkinOption _skinOptionPrefab;
        [SerializeField] private Transform _skinPreviewParent;

        [CustomHeader("Positions")]
        [SerializeField] private Transform _nextSkinTransform;
        [SerializeField] private Transform _currentSkinTransform;
        [SerializeField] private Transform _previousSkinTransform;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _animationDuration;

        [CustomHeader("Buttons")]
        [SerializeField] private Button _nextSkinButton;
        [SerializeField] private Button _previousSkinButton;
        [SerializeField] private Button _selectButton;

        private int _currentIndex;
        private SkinOption _nextSkinOption;
        private SkinOption _currentSkinOption;
        private SkinOption _previousSkinOption;

        private Coroutine _currentCoroutine;

        private void Awake()
        {
            ShowWindow();
            SetupButtons();
        }

        public void ShowWindow()
        {
            _selfCanvasGroup.alpha = 1f;
            _currentIndex = 0;

            _currentSkinOption = CreateSkinOption(ESkinOptionType.Current);
            _nextSkinOption = CreateSkinOption(ESkinOptionType.Next);
            _previousSkinOption = CreateSkinOption(ESkinOptionType.Previous);
        }

        public void CloseWindow()
        {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            Destroy(_nextSkinOption.gameObject);
            Destroy(_currentSkinOption.gameObject);
            Destroy(_previousSkinOption.gameObject);

            _selfCanvasGroup.alpha = 0f;
        }

        private SkinOption CreateSkinOption(ESkinOptionType skinOptionType)
        {
            SkinOption skinOption;
            Transform spawnPositionTransform;
            Sprite sprite;

            switch (skinOptionType)
            {
                case ESkinOptionType.Current:
                    spawnPositionTransform = _currentSkinTransform;
                    sprite = _skinsList[_currentIndex];
                    break;

                case ESkinOptionType.Next:

                    spawnPositionTransform = _nextSkinTransform;

                    if (_currentIndex + 1 >= _skinsList.Count)
                        sprite = _skinsList[0];
                    else
                        sprite = _skinsList[_currentIndex + 1];

                    break;

                case ESkinOptionType.Previous:
                    spawnPositionTransform = _previousSkinTransform;

                    if (_currentIndex - 1 < 0)
                        sprite = _skinsList[_skinsList.Count - 1];
                    else
                        sprite = _skinsList[_currentIndex - 1];

                    break;

                default:
                    Debug.LogError("Wrong SkinOption Type");
                    return null;
            }

            skinOption = Instantiate(_skinOptionPrefab, spawnPositionTransform.position, Quaternion.identity);
            skinOption.transform.SetParent(_skinPreviewParent);
            skinOption.SetSprite(sprite);
            return skinOption;
        }

        private void SetupButtons()
        {
            _nextSkinButton.onClick.AddListener(() =>
            {
                _nextSkinButton.interactable = false;
                _previousSkinButton.interactable = false;

                _currentIndex++;

                if (_currentIndex == _skinsList.Count)
                    _currentIndex = 0;

                Destroy(_previousSkinOption.gameObject);

                _currentCoroutine = StartCoroutine(PlayNextButtonAnimation(() =>
                {
                    _previousSkinOption = _currentSkinOption;
                    _currentSkinOption = _nextSkinOption;
                    _nextSkinOption = CreateSkinOption(ESkinOptionType.Next);

                    _nextSkinButton.interactable = true;
                    _previousSkinButton.interactable = true;
                }));
            });

            _previousSkinButton.onClick.AddListener(() =>
            {
                _nextSkinButton.interactable = false;
                _previousSkinButton.interactable = false;

                _currentIndex--;

                if (_currentIndex < 0)
                    _currentIndex = _skinsList.Count - 1;

                Destroy(_nextSkinOption.gameObject);

                _currentCoroutine = StartCoroutine(PlayPreviousButtonAnimation(() =>
                {
                    _nextSkinOption = _currentSkinOption;
                    _currentSkinOption = _previousSkinOption;
                    _previousSkinOption = CreateSkinOption(ESkinOptionType.Previous);

                    _nextSkinButton.interactable = true;
                    _previousSkinButton.interactable = true;
                }));
            });
        }

        private IEnumerator PlayNextButtonAnimation(Action action)
        {
            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(_currentSkinOption.SelfRectTransform.DOAnchorPosX(-400f, _animationDuration))
                .Join(_nextSkinOption.SelfRectTransform.DOAnchorPosX(0f, _animationDuration));

            yield return sequence.WaitForCompletion();
            action?.Invoke();
            _currentCoroutine = null;
        }

        private IEnumerator PlayPreviousButtonAnimation(Action action)
        {
            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(_currentSkinOption.SelfRectTransform.DOAnchorPosX(400f, _animationDuration))
                .Join(_previousSkinOption.SelfRectTransform.DOAnchorPosX(0f, _animationDuration));

            yield return sequence.WaitForCompletion();
            action?.Invoke();
            _currentCoroutine = null;
        }

        private enum ESkinOptionType
        {
            None,
            Current,
            Previous,
            Next
        }
    }
}