using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using System.Collections;
using Youregone.SL;
using Youregone.SaveSystem;

namespace Youregone.UI
{
    public class DeathScreenUI : MonoBehaviour
    {
        public event Action OnTryAgainButtonPressed;
        public event Action OnMainMenuButtonPressed;

        [CustomHeader("Components")]
        [SerializeField] private Button _tryAgainButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private TextMeshProUGUI _currentScoreText;
        [SerializeField] private CanvasGroup _highScoreCanvasGroup;
        [SerializeField] private CanvasGroup _currentScoreCanvasGroup;
        [SerializeField] private CanvasGroup _barCanvasGroup;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _sheepImage;
        [SerializeField] private Image _pathImage;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _buttonsFadeDuration;
        [SerializeField] private float _textFadeDuration;
        [SerializeField] private float _sheepSpeed;

        private RectTransform _selfRectTransform;

        private void Awake()
        {
            _selfRectTransform = GetComponent<RectTransform>();

            _tryAgainButton.onClick.AddListener(() =>
            {
                OnTryAgainButtonPressed?.Invoke();
                _tryAgainButton.interactable = false;
                _mainMenuButton.interactable = false;
            });

            _mainMenuButton.onClick.AddListener(() =>
            {
                OnMainMenuButtonPressed?.Invoke();
                _tryAgainButton.interactable = false;
                _mainMenuButton.interactable = false;
            });
        }

        public void ShowWindow()
        {
            gameObject.SetActive(true);
            HideAllElements();

            _currentScoreText.text = ((int)ServiceLocator.Get<GameScreenUI>().ScoreCounter.CurrentScore).ToString();
            _highScoreText.text = ServiceLocator.Get<PlayerPrefsSaverLoader>().GetHighScore().ToString();

            StartCoroutine(ShowWindowCoroutine());
        }

        private IEnumerator ShowWindowCoroutine()
        {
            Tween currentTween = _selfRectTransform.DOAnchorPos(Vector2.zero, _animationDuration).From(new Vector2(0f, Screen.height / 2));
            _backgroundImage.DOFade(.8f, _animationDuration).From(0f);

            yield return currentTween.WaitForCompletion();

            float sheepYOffset = 20f;
            Vector2 sheepStartPosition = new(-_pathImage.rectTransform.rect.width / 2f, sheepYOffset);
            _sheepImage.rectTransform.anchoredPosition = sheepStartPosition;

            _highScoreCanvasGroup.DOFade(1f, _textFadeDuration).From(0f);
            _barCanvasGroup.DOFade(1f, _textFadeDuration).From(0f);
            currentTween = _currentScoreCanvasGroup.DOFade(1f, _textFadeDuration).From(0f);

            yield return currentTween.WaitForCompletion();

            int currentScore = (int)ServiceLocator.Get<GameScreenUI>().ScoreCounter.CurrentScore;
            int highScrore = ServiceLocator.Get<PlayerPrefsSaverLoader>().GetHighScore();
            float t = (float)currentScore / highScrore;

            if (t < .1f)
                t = .1f;

            Vector2 sheepGoalPosition = new(Mathf.Lerp(-_pathImage.rectTransform.rect.width / 2f, _pathImage.rectTransform.rect.width / 2f, t), sheepYOffset);
            currentTween = _sheepImage.rectTransform.DOAnchorPos(sheepGoalPosition, _sheepSpeed).SetEase(Ease.InOutQuad);

            yield return currentTween.WaitForCompletion();

            _tryAgainButton.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(true);

            _tryAgainButton.image.DOFade(1f, _buttonsFadeDuration).From(0f);
            _mainMenuButton.image.DOFade(1f, _buttonsFadeDuration).From(0f);
        }

        private void HideAllElements()
        {
            _tryAgainButton.gameObject.SetActive(false);
            _mainMenuButton.gameObject.SetActive(false);

            _highScoreCanvasGroup.alpha = 0f;
            _currentScoreCanvasGroup.alpha = 0f;
            _barCanvasGroup.alpha = 0f;
        }
    }
}