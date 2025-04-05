using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using System.Collections;
using Youregone.SL;
using Youregone.SaveSystem;
using Youregone.GameSystems;
using Youregone.YPlayerController;
using System.Collections.Generic;
using System.Linq;
using Youregone.Web;
using System.Threading.Tasks;

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
        [SerializeField] private RectTransform _flag;
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private CanvasGroup _leaderBoardCanvasGroup;
        [SerializeField] private ScoreHolderUI _scoreHolderPrefab;
        [SerializeField] private Transform _scoreHolderParent;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _buttonsFadeDuration;
        [SerializeField] private float _textFadeDuration;
        [SerializeField] private float _sheepSpeed;

        private LocalDatabase _localDatabase;
        private RectTransform _selfRectTransform;
        private Sequence _currentSequence;

        private void Awake()
        {
            _localDatabase = ServiceLocator.Get<LocalDatabase>();
            _selfRectTransform = GetComponent<RectTransform>();

            _tryAgainButton.onClick.AddListener(() =>
            {
                if (_localDatabase.InternetConnectionAvailable)
                    ScoreWebUploader.UploadScoreHolder(_nameInputField.text, (int)ServiceLocator.Get<ScoreCounter>().CurrentScore);

                _localDatabase.SaveNewScoreEntry(_nameInputField.text, (int)ServiceLocator.Get<ScoreCounter>().CurrentScore);

                OnTryAgainButtonPressed?.Invoke();
                _tryAgainButton.interactable = false;
                _mainMenuButton.interactable = false;
            });

            _mainMenuButton.onClick.AddListener(() =>
            {
                if (_localDatabase.InternetConnectionAvailable)
                    ScoreWebUploader.UploadScoreHolder(_nameInputField.text, (int)ServiceLocator.Get<ScoreCounter>().CurrentScore);

                _localDatabase.SaveNewScoreEntry(_nameInputField.text, (int)ServiceLocator.Get<ScoreCounter>().CurrentScore);

                OnMainMenuButtonPressed?.Invoke();
                _tryAgainButton.interactable = false;
                _mainMenuButton.interactable = false;
            });
        }

        private void Start()
        {
            ServiceLocator.Get<PlayerController>().PlayerCharacterInput.OnScreenTap += PlayerCharacterInput_OnScreenTap;
            _nameInputField.onValueChanged.AddListener(CheckNameInputField);
        }
        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().PlayerCharacterInput.OnScreenTap -= PlayerCharacterInput_OnScreenTap;
        }

        public void ShowWindow()
        {
            HideAllElements();
            gameObject.SetActive(true);

            _currentScoreText.text = ((int)ServiceLocator.Get<ScoreCounter>().CurrentScore).ToString();
            _highScoreText.text = ServiceLocator.Get<LocalDatabase>().Highscore.ToString();

            StartCoroutine(ShowWindowCoroutine());
        }

        private void PlayerCharacterInput_OnScreenTap()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && _currentSequence != null)
                _currentSequence.Complete();
        }

        private IEnumerator ShowWindowCoroutine()
        {
            float currentScore = ServiceLocator.Get<ScoreCounter>().CurrentScore;
            int highScrore = ServiceLocator.Get<LocalDatabase>().Highscore;
            float t;

            if (highScrore != 0)
                t = currentScore / highScrore;
            else
                t = 100f;

            if (t > 1)
            {
                t = .75f; // (.51f - 1f)
                _flag.anchoredPosition = _pathImage.rectTransform.anchoredPosition;
            }
            else
                t = t <= .1f ? .1f : t; // min distance so sheep doesn't stand at 0 without movement

            yield return StartCoroutine(PlayBackgroundAnimation());

            yield return StartCoroutine(PlayTextAndPathFadeInAnimation());

            yield return StartCoroutine(PlaySheepAnimation(t));

            yield return StartCoroutine(PlayButtonsFadeInAnimation());

            if (SystemInfo.deviceType == DeviceType.Handheld)
            {
                _nameInputField.ActivateInputField();
                TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
            }
        }

        private IEnumerator PlayBackgroundAnimation()
        {
            float backgroundGoalAlpha = .8f;

            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(_selfRectTransform.DOAnchorPos(Vector2.zero, _animationDuration).From(new Vector2(0f, Screen.height / 2)))
                .Join(_backgroundImage.DOFade(backgroundGoalAlpha, _animationDuration).From(0f))
                .Join(_leaderBoardCanvasGroup.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, _animationDuration).From(new Vector2(0f, Screen.height / 2)))
                .Join(_leaderBoardCanvasGroup.DOFade(1f, _animationDuration).From(0f))
                .OnComplete(() =>
                {
                    _currentSequence = null;
                    StartCoroutine(ShowScoreHolders());
                });

            yield return _currentSequence.WaitForCompletion();
        }

        private IEnumerator PlayTextAndPathFadeInAnimation()
        {
            float uiSheepYOffset = 20f;
            Vector2 uiSheepStartPosition = new(-_pathImage.rectTransform.rect.width / 2f, uiSheepYOffset);
            _sheepImage.rectTransform.anchoredPosition = uiSheepStartPosition;

            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(_highScoreCanvasGroup.DOFade(1f, _textFadeDuration).From(0f))
                .Join(_barCanvasGroup.DOFade(1f, _textFadeDuration).From(0f))
                .Join(_currentScoreCanvasGroup.DOFade(1f, _textFadeDuration).From(0f))
                .OnComplete(() => _currentSequence = null);

            yield return _currentSequence.WaitForCompletion();
        }

        private IEnumerator PlaySheepAnimation(float t)
        {
            float uiSheepYOffset = 20f;
            Vector2 sheepGoalPosition = new(Mathf.Lerp(-_pathImage.rectTransform.rect.width / 2f, _pathImage.rectTransform.rect.width / 2f, t), uiSheepYOffset);

            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(_sheepImage.rectTransform.DOAnchorPos(sheepGoalPosition, _sheepSpeed)
                .SetEase(Ease.InOutQuad))
                .OnComplete(() => _currentSequence = null);

            yield return _currentSequence.WaitForCompletion();
        }

        private IEnumerator PlayButtonsFadeInAnimation()
        {
            _tryAgainButton.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(true);
            _nameInputField.gameObject.SetActive(true);

            _tryAgainButton.interactable = false;
            _mainMenuButton.interactable = false;

            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(_tryAgainButton.image.DOFade(1f, _buttonsFadeDuration).From(0f))
                .Join(_mainMenuButton.image.DOFade(1f, _buttonsFadeDuration).From(0f))
                .Join(_nameInputField.image.DOFade(1f, _buttonsFadeDuration).From(0f))
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tryAgainButton.image.color = new Color(1f, 1f, 1f, .5f);
                    _mainMenuButton.image.color = new Color(1f, 1f, 1f, .5f);
                    _currentSequence = null;
                });

            yield return _currentSequence.WaitForCompletion();
        }

        private void CheckNameInputField(string name)
        {
            if(!string.IsNullOrWhiteSpace(name))
            {
                _tryAgainButton.image.color = new Color(1f, 1f, 1f, 1f);
                _mainMenuButton.image.color = new Color(1f, 1f, 1f, 1f);

                _tryAgainButton.interactable = true;
                _mainMenuButton.interactable = true;
            }
            else
            {
                _tryAgainButton.image.color = new Color(1f, 1f, 1f, .5f);
                _mainMenuButton.image.color = new Color(1f, 1f, 1f, .5f);

                _tryAgainButton.interactable = false;
                _mainMenuButton.interactable = false;
            }
        }

        private IEnumerator ShowScoreHolders()
        {
            //List<ScoreEntry> scoreHolders = null;

            //Debug.Log("Starting download");
            //Task<List<ScoreEntry>> downloadTask = ScoreWebUploader.DownloadScoreHoldersAsync();

            //while (!downloadTask.IsCompleted)
            //{
            //    Debug.Log("Downloading...");
            //    yield return null;
            //}

            //if (downloadTask.IsCompletedSuccessfully)
            //{
            //    Debug.Log("Download succeeded");
            //    scoreHolders = downloadTask.Result;
            //}
            //else
            //{
            //    Debug.LogError("Download failed");
            //    yield break;
            //}

            //if (scoreHolders == null || scoreHolders.Count == 0)
            //{
            //    Debug.Log("Data is null");
            //    yield break;
            //}

            List<ScoreEntry> scoreHolders = ServiceLocator.Get<LocalDatabase>().ScoreHoldersList;
            int amountOfHoldersToShow = Mathf.Min(8, scoreHolders.Count);
            float _delay = .15f;

            for(int i = 0; i < amountOfHoldersToShow; i++)
            {
                ScoreHolderUI recordHolderUI = Instantiate(_scoreHolderPrefab);
                recordHolderUI.transform.SetParent(_scoreHolderParent, false);
                recordHolderUI.SetData(scoreHolders[i].name, scoreHolders[i].score);

                yield return new WaitForSeconds(_delay);
            }
        }

        private void HideAllElements()
        {
            _tryAgainButton.gameObject.SetActive(false);
            _mainMenuButton.gameObject.SetActive(false);
            _nameInputField.gameObject.SetActive(false);

            _highScoreCanvasGroup.alpha = 0f;
            _currentScoreCanvasGroup.alpha = 0f;
            _barCanvasGroup.alpha = 0f;
        }
    }
}