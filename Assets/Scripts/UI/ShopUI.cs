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

        [CustomHeader("Skin Preview Settings")]
        [SerializeField] private int _skinsPreviewsOnScreenAmount;
        [SerializeField] private float _skinOptionDistance;
        [SerializeField] private RectTransform _skinPreviewParent;
        [SerializeField] private Transform _currentSkinTransform;
        [SerializeField] private SkinOption _skinOptionPrefab;

        [CustomHeader("Components")]
        [SerializeField] private CanvasGroup _selfCanvasGroup;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _animationDuration;
        [SerializeField] private Vector3 _currentSkinOptionGoalScale;
        [SerializeField] private Vector3 _skinOptionDefaultScale;

        [CustomHeader("Buttons")]
        [SerializeField] private Button _nextSkinButton;
        [SerializeField] private Button _previousSkinButton;
        [SerializeField] private Button _selectButton;
        [SerializeField] private Button _closeShopButton;

        [CustomHeader("Debug")]
        [SerializeField] private List<SkinOption> _skinOptionList;

        private int _currentIndex;
        private Coroutine _currentCoroutine;

        private int _skinPreviewOverallAmount
        {
            get
            {
                return _skinsPreviewsOnScreenAmount + 2; // +2 offscreen (1 most-left and 1 most-right)
            }
        }

        public bool ShopOpened { get; private set; }

        private void Awake()
        {
            _skinOptionList = new();
            _selfCanvasGroup.alpha = 0f;
            SetupButtons();
        }

        public void ShowWindow()
        {
            ShopOpened = true;

            Vector2 newSizeDelta = _skinPreviewParent.sizeDelta;
            newSizeDelta.x = _skinOptionDistance * _skinsPreviewsOnScreenAmount;
            _skinPreviewParent.sizeDelta = newSizeDelta;

            _selfCanvasGroup.alpha = 1f;
            _currentIndex = 0;

            for (int i = -(_skinPreviewOverallAmount / 2); i <= _skinPreviewOverallAmount / 2; i++)
            {
                float positionX = i * _skinOptionDistance;
                SkinOption skinOption = CreateSkinOption(positionX, false);

                if (i == 0)
                    skinOption.transform.localScale = _currentSkinOptionGoalScale;
            }
        }

        public void CloseWindow()
        {
            ShopOpened = false;

            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            foreach(SkinOption skinOption in _skinOptionList)
            {
                Destroy(skinOption.gameObject);
            }
            _skinOptionList.Clear();
            _selfCanvasGroup.alpha = 0f;
        }

        private SkinOption CreateSkinOption(float positionX, bool addAtStart)
        {
            SkinOption skinOption;
            Sprite sprite;

            sprite = _skinsList[GetSkinIdFromPosition(positionX)];

            skinOption = Instantiate(_skinOptionPrefab);
            skinOption.transform.SetParent(_skinPreviewParent);
            skinOption.SelfRectTransform.anchoredPosition = new Vector2(positionX, 0f);
            skinOption.SetSprite(sprite);

            if(addAtStart)
                _skinOptionList.Insert(0, skinOption);
            else
                _skinOptionList.Add(skinOption);

            return skinOption;
        }

        private int GetSkinIdFromPosition(float positionX)
        {
            int skinOptionPlacementId = (int)(positionX / _skinOptionDistance);
            int skinOptionPlacementIdToSkinListId = _currentIndex + skinOptionPlacementId;
            int resultId = skinOptionPlacementIdToSkinListId;

            if(skinOptionPlacementIdToSkinListId < 0)
                resultId = _skinsList.Count - Mathf.Abs(skinOptionPlacementIdToSkinListId);
            
            if (skinOptionPlacementIdToSkinListId >= _skinsList.Count)
                resultId = skinOptionPlacementIdToSkinListId - _skinsList.Count;
            
            return resultId;
        }

        private void SetupButtons()
        {
            _closeShopButton.onClick.AddListener(() =>
            {
                CloseWindow();
            });

            _nextSkinButton.onClick.AddListener(() =>
            {
                _nextSkinButton.interactable = false;
                _previousSkinButton.interactable = false;

                _currentIndex++;

                if (_currentIndex == _skinsList.Count)
                    _currentIndex = 0;

                _currentCoroutine =  StartCoroutine(PlayNextButtonAnimation(() =>
                {
                    _nextSkinButton.interactable = true;
                    _previousSkinButton.interactable = true;
                    _currentCoroutine = null;
                }));
            });

            _previousSkinButton.onClick.AddListener(() =>
            {
                _nextSkinButton.interactable = false;
                _previousSkinButton.interactable = false;

                _currentIndex--;

                if (_currentIndex < 0)
                    _currentIndex = _skinsList.Count - 1;

                _currentCoroutine = StartCoroutine(PlayPreviousButtonAnimation(() =>
                {
                    _nextSkinButton.interactable = true;
                    _previousSkinButton.interactable = true;
                    _currentCoroutine = null;
                }));
            });
        }

        private IEnumerator PlayNextButtonAnimation(Action callback)
        {
            int foreachIndex = 0;
            Tween lastTween = null;

            foreach(SkinOption skinOption in _skinOptionList)
            {
                lastTween = skinOption.SelfRectTransform.DOAnchorPosX(
                    skinOption.SelfRectTransform.anchoredPosition.x - _skinOptionDistance,
                    _animationDuration);

                if(foreachIndex == _skinPreviewOverallAmount / 2)
                {
                    skinOption.SelfRectTransform.DOScale(_skinOptionDefaultScale, _animationDuration);
                }

                if(foreachIndex == _skinPreviewOverallAmount / 2 + 1)
                {
                    skinOption.SelfRectTransform.DOScale(_currentSkinOptionGoalScale, _animationDuration);
                }

                foreachIndex++;
            }

            yield return lastTween.WaitForCompletion();

            lastTween = null;
            Destroy(_skinOptionList[0].gameObject);
            _skinOptionList.RemoveAt(0);
            callback?.Invoke();
            CreateSkinOption(foreachIndex / 2 * _skinOptionDistance, false);
        }

        private IEnumerator PlayPreviousButtonAnimation(Action callback)
        {
            int foreachIndex = 0;
            Tween lastTween = null;

            foreach (SkinOption skinOption in _skinOptionList)
            {
                lastTween = skinOption.SelfRectTransform.DOAnchorPosX(
                    skinOption.SelfRectTransform.anchoredPosition.x + _skinOptionDistance,
                    _animationDuration
                );

                if (foreachIndex == _skinPreviewOverallAmount / 2)
                {
                    skinOption.SelfRectTransform.DOScale(_skinOptionDefaultScale, _animationDuration);
                }

                if (foreachIndex == _skinPreviewOverallAmount / 2 - 1)
                {
                    skinOption.SelfRectTransform.DOScale(_currentSkinOptionGoalScale, _animationDuration);
                }

                foreachIndex++;
            }

            yield return lastTween.WaitForCompletion();
            lastTween = null;

            Destroy(_skinOptionList[^1].gameObject);
            _skinOptionList.RemoveAt(_skinOptionList.Count - 1);

            callback?.Invoke();

            CreateSkinOption(-foreachIndex / 2 * _skinOptionDistance, true);
        }
    }
}