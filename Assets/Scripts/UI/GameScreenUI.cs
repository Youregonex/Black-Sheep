using UnityEngine;
using Youregone.SL;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Youregone.GameSystems;
using Youregone.SaveSystem;
using DG.Tweening;
using System.Collections;

namespace Youregone.UI
{
    public class GameScreenUI : MonoBehaviour, IService
    {
        public event Action OnMainMenuLoadRequest;
        public event Action OnGameOutroToggleRequest;
        public event Action OnPauseButtonPressed;

        [CustomHeader("Config")]
        [SerializeField] private ScoreCounterUI _scoreCounter;
        [SerializeField] private HealthbarUI _healthbarUI;
        [SerializeField] private CloversCollectedUI _cloversCollectedUI;
        [SerializeField] private CanvasGroup _scoreCanvasGroup;
        [SerializeField] private CanvasGroup _healthCanvasGroup;
        [SerializeField] private CanvasGroup _cloversCanvasGroup;

        [CustomHeader("Buttons")]
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _outroToggleButton;
        [SerializeField] private Button _pauseButton;

        [CustomHeader("Sprites")]
        [SerializeField] private Sprite _mainMenuButtonPressedSprite;
        [SerializeField] private Sprite _outroDisableButtonSpriteOn;
        [SerializeField] private Sprite _outroDisableButtonSpriteOff;
        [SerializeField] private Sprite _pauseSpriteGameNotPaused;
        [SerializeField] private Sprite _pauseSpriteGamePaused;

        [CustomHeader("Path Elements")]
        [SerializeField] private CanvasGroup _onScreenPathCanvasGroup;
        [SerializeField] private CanvasGroup _uiSheep;
        [SerializeField] private CanvasGroup _flagWebHS;
        [SerializeField] private CanvasGroup _flagPersonalHS;
        [SerializeField] private RectTransform _flagParent;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _buttonAnimationTime;
        [SerializeField] private float _pathTargerWidth;
        [SerializeField] private float _pathAnimationTime;
        [SerializeField] private float _pathIconsAnimationTime;
        [SerializeField] private float _iconAnimationsDelay;
        [SerializeField] private float _otherElementsAnimationTime;

        private int _personalHighscore;
        private int _webHighscore;

        private int _highestOverall
        {
            get
            {
                return _personalHighscore >= _webHighscore ? _personalHighscore : _webHighscore;
            }
        }

        public HealthbarUI HealthbarUI => _healthbarUI;
        public ScoreCounterUI ScoreCounter => _scoreCounter;
        public Button MainMenuButton => _mainMenuButton;
        public Button OutroDisableButton => _outroToggleButton;
        public CloversCollectedUI CloversCollectedUI => _cloversCollectedUI;

        private void Start()
        {
            _mainMenuButton.onClick.AddListener(() =>
            {
                _mainMenuButton.image.sprite = _mainMenuButtonPressedSprite;
                OnMainMenuLoadRequest?.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            });

            _outroToggleButton.onClick.AddListener(() =>
            {
                OnGameOutroToggleRequest?.Invoke();
                _outroToggleButton.image.sprite = !ServiceLocator.Get<GameSettings>().OutroEnabled ? _outroDisableButtonSpriteOff : _outroDisableButtonSpriteOn;
                EventSystem.current.SetSelectedGameObject(null);
            });

            _pauseButton.onClick.AddListener(() =>
            {
                if(ServiceLocator.Get<GameState>().CurrentGameState == EGameState.Pause)
                    _pauseButton.image.sprite = _pauseSpriteGameNotPaused;
                else
                    _pauseButton.image.sprite = _pauseSpriteGamePaused;

                OnPauseButtonPressed?.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            });

            _outroToggleButton.image.sprite = ServiceLocator.Get<GameSettings>().OutroEnabled ? _outroDisableButtonSpriteOn : _outroDisableButtonSpriteOff;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<ScoreCounter>().OnScoreChanged -= ScoreCounter_OnScoreChanged;
        }

        public void ShowUIElements()
        {
            StartCoroutine(ShowUIElementsCoroutin());
        }

        public void HideUIElements()
        {
            _onScreenPathCanvasGroup.alpha = 0f;
            _scoreCounter.gameObject.SetActive(false);
            _healthbarUI.gameObject.SetActive(false);
            _outroToggleButton.gameObject.SetActive(false);
            _mainMenuButton.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(false);
            _cloversCollectedUI.gameObject.SetActive(false);
        }

        private IEnumerator ShowUIElementsCoroutin()
        {
            yield return PlayButtonAnimations().WaitForCompletion();
            yield return ShowPath().WaitForCompletion();

            _scoreCounter.gameObject.SetActive(true);
            _healthbarUI.gameObject.SetActive(true);
            _cloversCollectedUI.gameObject.SetActive(true);

            _scoreCanvasGroup.DOFade(1f, _otherElementsAnimationTime).From(0f).SetEase(Ease.InQuad);
            _scoreCanvasGroup.DOFade(1f, _otherElementsAnimationTime).From(0f).SetEase(Ease.InQuad);
            _scoreCanvasGroup.DOFade(1f, _otherElementsAnimationTime).From(0f).SetEase(Ease.InQuad);
        }

        private Sequence PlayButtonAnimations()
        {
            _pauseButton.gameObject.SetActive(true);
            _outroToggleButton.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(true);

            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(_pauseButton.image.DOFade(1f, _buttonAnimationTime).From(0f).SetEase(Ease.Linear))
                .Join(_pauseButton.transform.DOScale(1f, _buttonAnimationTime).From(.5f).SetEase(Ease.OutBounce))
                .Append(_outroToggleButton.image.DOFade(1f, _buttonAnimationTime).From(0f).SetEase(Ease.Linear))
                .Join(_outroToggleButton.transform.DOScale(1f, _buttonAnimationTime).From(.5f).SetEase(Ease.OutBounce))
                .Append(_mainMenuButton.image.DOFade(1f, _buttonAnimationTime).From(0f).SetEase(Ease.Linear))
                .Join(_mainMenuButton.transform.DOScale(1f, _buttonAnimationTime).From(.5f).SetEase(Ease.OutBounce));

            return sequence;
        }

        private Tween ShowPath()
        {
            _uiSheep.alpha = 0f;
            _flagPersonalHS.alpha = 0f;
            _flagWebHS.alpha = 0f;

            _flagParent.sizeDelta = new Vector2(0f, _flagParent.sizeDelta.y);
            _onScreenPathCanvasGroup.alpha = 1f;

            Sequence sequence = DOTween.Sequence();

            Vector2 pathWidthGoal = new(_pathTargerWidth, _flagParent.sizeDelta.y);
            sequence.Append(_flagParent.DOSizeDelta(pathWidthGoal, _pathAnimationTime));

            _personalHighscore = ServiceLocator.Get<LocalDatabase>().GetHighscore(true);
            _webHighscore = ServiceLocator.Get<LocalDatabase>().GetHighscore(false);

            if(_personalHighscore >= _webHighscore)
            {
                _flagWebHS.gameObject.SetActive(false);
                _flagPersonalHS.transform.localPosition = new Vector2(_pathTargerWidth / 2, _flagPersonalHS.transform.localPosition.y);

                sequence
                    .Append(_flagPersonalHS.DOFade(1f, _pathIconsAnimationTime).SetEase(Ease.Linear))
                    .Join(_flagPersonalHS.transform.DOScale(1f, _pathAnimationTime).From(0f).SetEase(Ease.OutBounce));
            }
            else
            {
                _flagWebHS.transform.localPosition = new Vector2(_pathTargerWidth / 2, _flagWebHS.transform.localPosition.y);

                if (_personalHighscore != 0)
                {
                    float t = (float)_personalHighscore / _webHighscore;

                    _flagPersonalHS.transform.localPosition =
                        Vector2.Lerp(
                            new Vector2(-_pathTargerWidth / 2, _flagPersonalHS.transform.localPosition.y),
                            new Vector2(_pathTargerWidth / 2, _flagPersonalHS.transform.localPosition.y),
                            t);

                    sequence
                        .Append(_flagWebHS.DOFade(1f, _pathIconsAnimationTime).SetEase(Ease.Linear))
                        .Join(_flagWebHS.transform.DOScale(1f, _pathAnimationTime).From(0f).SetEase(Ease.OutBounce))
                        .Insert(_pathAnimationTime + _iconAnimationsDelay, _flagPersonalHS.DOFade(1f, _pathIconsAnimationTime).SetEase(Ease.Linear))
                        .Join(_flagPersonalHS.transform.DOScale(1f, _pathAnimationTime).From(0f).SetEase(Ease.OutBounce));
                }
                else
                {
                    _flagPersonalHS.gameObject.SetActive(false);
                }
            }

            sequence
                .Insert(_pathAnimationTime + _iconAnimationsDelay, _uiSheep.DOFade(1f, _pathIconsAnimationTime)).SetEase(Ease.Linear)
                .Join(_uiSheep.transform.DOScale(new Vector2(-1f, 1f), _pathIconsAnimationTime).From(0f).SetEase(Ease.OutBounce));

            ServiceLocator.Get<ScoreCounter>().OnScoreChanged += ScoreCounter_OnScoreChanged;
            return sequence;
        }

        private void ScoreCounter_OnScoreChanged(int newScore)
        {
            float t = (float)newScore / _highestOverall;

            _uiSheep.transform.localPosition =
                Vector2.Lerp(
                    new Vector2(-_pathTargerWidth / 2, _uiSheep.transform.localPosition.y),
                    new Vector2(_pathTargerWidth / 2, _uiSheep.transform.localPosition.y),
                    t);
        }
    }
}