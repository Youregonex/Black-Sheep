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
using Youregone.Utils;

namespace Youregone.UI
{
    public class DeathScreenUI : MonoBehaviour
    {
        public event Action OnTryAgainButtonPressed;
        public event Action OnMainMenuButtonPressed;

        [CustomHeader("Canvas Groups")]
        [SerializeField] private CanvasGroup _highScoreCanvasGroup;
        [SerializeField] private CanvasGroup _currentScoreCanvasGroup;
        [SerializeField] private CanvasGroup _barCanvasGroup;
        [SerializeField] private CanvasGroup _cloversCollectedCanvasGroup;

        [CustomHeader("Buttons")]
        [SerializeField] private Button _tryAgainButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _exitGameButton;

        [CustomHeader("Images")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Image _sheepImage;

        [CustomHeader("TMPs")]
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private TextMeshProUGUI _currentScoreText;
        [SerializeField] private TextMeshProUGUI _baseCloversCollectedText;
        [SerializeField] private TextMeshProUGUI _rareCloversCollectedText;

        [CustomHeader("Other")]
        [SerializeField] private RectTransform _pathRectTransform;
        [SerializeField] private TMP_InputField _nameInputField;
        [SerializeField] private RectTransform _flag;
        [SerializeField] private CanvasGroup _leaderBoardCanvasGroup;
        [SerializeField] private ScoreHolderUI _scoreHolderPrefab;
        [SerializeField] private Transform _scoreHolderParent;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _buttonsFadeDuration;
        [SerializeField] private float _textFadeDuration;
        [SerializeField] private float _sheepSpeed;
        [SerializeField] private float _cloverTextAnimationTime;

        [CustomHeader("Debug")]
        [SerializeField] private LocalDatabase _localDatabase;

        private PlayerPrefsSaverLoader _playerPrefs;
        private RectTransform _selfRectTransform;
        private Sequence _currentSequence;

        private void Awake()
        {
            _selfRectTransform = GetComponent<RectTransform>();
            _nameInputField.onValueChanged.AddListener(CheckNameInputField);

            SetupButtons();
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().PlayerCharacterInput.OnScreenTap -= PlayerCharacterInput_OnScreenTap;
        }

        public void ShowWindow()
        {
            _localDatabase = ServiceLocator.Get<LocalDatabase>();
            _playerPrefs = ServiceLocator.Get<PlayerPrefsSaverLoader>();
            ServiceLocator.Get<PlayerController>().PlayerCharacterInput.OnScreenTap += PlayerCharacterInput_OnScreenTap;

            HideAllElements();
            gameObject.SetActive(true);

            _currentScoreText.text = ((int)ServiceLocator.Get<ScoreCounter>().CurrentScore).ToString();
            _highScoreText.text = _localDatabase.Highscore.ToString();

            StartCoroutine(ShowWindowCoroutine());
        }

        private void PlayerCharacterInput_OnScreenTap()
        {
            if (_currentSequence != null)
                _currentSequence.Complete();
        }

        private IEnumerator ShowWindowCoroutine()
        {
            float currentScore = ServiceLocator.Get<ScoreCounter>().CurrentScore;
            int highScore = _localDatabase.Highscore;
            float t;

            string lastName = _playerPrefs.GetLastRecordHolderName();

            if (lastName != null)
                _nameInputField.text = lastName;

            if (highScore != 0)
                t = currentScore / highScore;
            else
                t = 100f;

            if (t > 1)
            {
                t = .75f; // (.51f -- 1f)
                _flag.anchoredPosition = _pathRectTransform.anchoredPosition;
            }
            else
                t = t <= .1f ? .1f : t; // min distance so sheep doesn't stand at 0 without movement

            yield return StartCoroutine(PlayBackgroundAnimation());

            yield return StartCoroutine(PlayTextAndPathFadeInAnimation());

            yield return StartCoroutine(PlaySheepAnimation(t));

            yield return StartCoroutine(PlayButtonsAndCloversCollectedFadeInAnimation());

            CheckNameInputField(_nameInputField.text);
        }

        private IEnumerator PlayBackgroundAnimation()
        {
            RectTransform leaderboardRectTransform = _leaderBoardCanvasGroup.GetComponent<RectTransform>();
            Vector2 leaderboardGoalPosition = new(20f, 0f);
            float backgroundGoalAlpha = 1f;
            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(_selfRectTransform.DOAnchorPos(Vector2.zero, _animationDuration).From(new Vector2(0f, Screen.height / 2f)))
                .Join(_backgroundImage.DOFade(backgroundGoalAlpha, _animationDuration).From(0f))
                .Join(leaderboardRectTransform.DOAnchorPos(leaderboardGoalPosition, _animationDuration).From(new Vector2(leaderboardRectTransform.rect.x, Screen.height / 2f)))
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
            Vector2 uiSheepStartPosition = new(-_pathRectTransform.rect.width / 2f, 0f);
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
            Vector2 sheepGoalPosition = new(Mathf.Lerp(-_pathRectTransform.rect.width / 2f, _pathRectTransform.rect.width / 2f, t), 0f);

            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(_sheepImage.rectTransform.DOAnchorPos(sheepGoalPosition, _sheepSpeed)
                .SetEase(Ease.InOutQuad))
                .OnComplete(() =>
                {
                    _currentSequence = null;
                });

            yield return _currentSequence.WaitForCompletion();
        }

        private IEnumerator PlayButtonsAndCloversCollectedFadeInAnimation()
        {
            _tryAgainButton.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(true);
            _exitGameButton.gameObject.SetActive(true);
            _nameInputField.gameObject.SetActive(true);

            _baseCloversCollectedText.text = "0";
            _rareCloversCollectedText.text = "0";

            _tryAgainButton.interactable = false;
            _mainMenuButton.interactable = false;
            _exitGameButton.interactable = true;

            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(_tryAgainButton.image.DOFade(1f, _buttonsFadeDuration).From(0f))
                .Join(_mainMenuButton.image.DOFade(1f, _buttonsFadeDuration).From(0f))
                .Join(_exitGameButton.image.DOFade(1f, _buttonsFadeDuration).From(0f))
                .Join(_nameInputField.image.DOFade(1f, _buttonsFadeDuration).From(0f))
                .Join(_cloversCollectedCanvasGroup.DOFade(1f, _buttonsFadeDuration).From(0f))
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _tryAgainButton.image.color = new Color(1f, 1f, 1f, .5f);
                    _mainMenuButton.image.color = new Color(1f, 1f, 1f, .5f);
                    _exitGameButton.image.color = new Color(1f, 1f, 1f, 1f);

                    int baseCloverGoal = ServiceLocator.Get<PlayerCloversCollected>().BaseCloversCollected;
                    int rareCloverGoal = ServiceLocator.Get<PlayerCloversCollected>().RareCloversCollected;

                    _currentSequence = DOTween.Sequence();
                    _currentSequence
                    // base uiClover animation
                    .Append(DOVirtual.Float(0f, baseCloverGoal, _cloverTextAnimationTime, value =>
                    {
                        _baseCloversCollectedText.text = Mathf.RoundToInt(value).ToString();
                    }))
                    .SetEase(Ease.InOutQuad)
                    .Join(_baseCloversCollectedText.transform.DOScale(1.5f, .5f).SetLoops(2, LoopType.Yoyo))
                    .Join(_baseCloversCollectedText.DOColor(Color.yellow, 0.2f).SetLoops(2, LoopType.Yoyo))
                    // rare uiClover animation
                    .Join(DOVirtual.Float(0f, rareCloverGoal, _cloverTextAnimationTime, value =>
                    {
                        _rareCloversCollectedText.text = Mathf.RoundToInt(value).ToString();
                    }))
                    .SetEase(Ease.InOutQuad)
                    .Join(_rareCloversCollectedText.transform.DOScale(1.5f, .5f).SetLoops(2, LoopType.Yoyo))
                    .Join(_rareCloversCollectedText.DOColor(Color.yellow, 0.2f).SetLoops(2, LoopType.Yoyo))
                    .OnComplete(() =>
                    {
                        _currentSequence = null;
                        _baseCloversCollectedText.DOColor(Color.white, 0.2f);
                        _rareCloversCollectedText.DOColor(Color.white, 0.2f);
                    });
                });

            yield return _currentSequence.WaitForCompletion();
        }

        private void CheckNameInputField(string name)
        {
            int nameLengthMax = 15;
            if(!string.IsNullOrWhiteSpace(name) && name.Length <= nameLengthMax)
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
            List<ScoreEntry> scoreHolders;

            if (_localDatabase.InternetConnectionAvailable)
                scoreHolders = _localDatabase.PersonalAndWebResults;
            else
                scoreHolders = _localDatabase.PersonalResults;

            int maxScoreHoldersToDisplay = 8;
            int amountOfHoldersToShow = Mathf.Min(maxScoreHoldersToDisplay, scoreHolders.Count);
            float _delay = .15f;

            for(int i = 0; i < amountOfHoldersToShow; i++)
            {
                ScoreHolderUI recordHolderUI = Instantiate(_scoreHolderPrefab);
                recordHolderUI.transform.SetParent(_scoreHolderParent, false);

                bool isPersonalRecord = _localDatabase.PersonalResults.Contains(scoreHolders[i]);
                recordHolderUI.SetData(StringEncryptor.GetShortName(scoreHolders[i].name), scoreHolders[i].score, isPersonalRecord);

                yield return new WaitForSeconds(_delay);
            }
        }

        private void HideAllElements()
        {
            _tryAgainButton.gameObject.SetActive(false);
            _mainMenuButton.gameObject.SetActive(false);
            _exitGameButton.gameObject.SetActive(false);
            _nameInputField.gameObject.SetActive(false);

            _cloversCollectedCanvasGroup.alpha = 0f;
            _highScoreCanvasGroup.alpha = 0f;
            _currentScoreCanvasGroup.alpha = 0f;
            _barCanvasGroup.alpha = 0f;
        }

        private void ButtonClickSaveResults()
        {
            int currentScore = (int)ServiceLocator.Get<ScoreCounter>().CurrentScore;

            _localDatabase.SaveNewScoreEntry(_nameInputField.text, currentScore);
            _playerPrefs.SaveLastRecordHolderName(_nameInputField.text);

            if (_localDatabase.InternetConnectionAvailable)
                ScoreWebUploader.UploadScoreHolder(_nameInputField.text, currentScore, true);
            
            _tryAgainButton.interactable = false;
            _mainMenuButton.interactable = false;
            _exitGameButton.interactable = false;
        }

        private void SetupButtons()
        {
            _tryAgainButton.onClick.AddListener(() =>
            {
                ButtonClickSaveResults();
                OnTryAgainButtonPressed?.Invoke(); 
            });

            _mainMenuButton.onClick.AddListener(() =>
            {
                ButtonClickSaveResults();
                OnMainMenuButtonPressed?.Invoke();
            });

            _exitGameButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}