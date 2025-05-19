using UnityEngine;
using Youregone.SL;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Youregone.GameSystems;
using Youregone.SaveSystem;
using DG.Tweening;
using System.Collections;
using Youregone.YPlayerController;

namespace Youregone.UI
{
    public class GameScreenUI : MonoBehaviour, IService
    {
        public event Action OnMainMenuLoadRequest;
        public event Action OnPauseToggleRequest;

        [CustomHeader("UI Elements")]
        [SerializeField] private ScoreCounterUI _scoreCounter;
        [SerializeField] private HealthbarUI _healthbarUI;
        [SerializeField] private CloversCollectedUI _cloversCollectedUI;
        [SerializeField] private OutroScenePlayer _outroScenePlayer;
        [SerializeField] private DeathScreenUI _deathScreenUI;
        [SerializeField] private SettingsMenu _settingsMenu;

        [CustomHeader("Canvas Groups")]
        [SerializeField] private CanvasGroup _scoreCanvasGroup;
        [SerializeField] private CanvasGroup _healthCanvasGroup;
        [SerializeField] private CanvasGroup _cloversCanvasGroup;

        [CustomHeader("Buttons")]
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _openSettingsButton;
        [SerializeField] private Button _closeSettingsButton;

        [CustomHeader("Sprites")]
        [SerializeField] private Sprite _mainMenuButtonPressedSprite;
        [SerializeField] private Sprite _pauseSpriteGameNotPaused;
        [SerializeField] private Sprite _pauseSpriteGamePaused;

        [CustomHeader("Path Elements")]
        [SerializeField] private CanvasGroup _onScreenPathCanvasGroup;
        [SerializeField] private CanvasGroup _uiSheep;
        [SerializeField] private CanvasGroup _flagWebHS;
        [SerializeField] private CanvasGroup _flagPersonalHS;
        [SerializeField] private RectTransform _pathWindowRectTransform;
        [SerializeField] private float _pathOffset;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _buttonAnimationTime;
        [SerializeField] private float _pathWindowTargetWidth;
        [SerializeField] private float _pathAnimationTime;
        [SerializeField] private float _pathIconsAnimationTime;
        [SerializeField] private float _iconAnimationsDelay;
        [SerializeField] private float _otherElementsAnimationTime;

        private int _personalHighscore;
        private int _webHighscore;

        private int _highestOverall => Mathf.Max(_personalHighscore, _webHighscore);

        public HealthbarUI HealthbarUI => _healthbarUI;
        public ScoreCounterUI ScoreCounter => _scoreCounter;
        public CloversCollectedUI CloversCollectedUI => _cloversCollectedUI;
        public OutroScenePlayer OutroScenePlayer => _outroScenePlayer;
        public DeathScreenUI DeathScreenUI => _deathScreenUI;
        public Button MainMenuButton => _mainMenuButton;

        private void Start()
        {
            _mainMenuButton.onClick.AddListener(() =>
            {
                _mainMenuButton.image.sprite = _mainMenuButtonPressedSprite;

                OnMainMenuLoadRequest?.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            });

            _openSettingsButton.onClick.AddListener(() =>
            {
                if (_settingsMenu.IsOpened)
                    _settingsMenu.HideWindow();
                else
                    _settingsMenu.ShowWindow();

                OnPauseToggleRequest?.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            });

            _closeSettingsButton.onClick.AddListener(() =>
            {
                if (_settingsMenu.IsOpened)
                    _settingsMenu.HideWindow();

                OnPauseToggleRequest?.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            });

            ServiceLocator.Get<PlayerController>().PlayerCharacterInput.OnPauseButtonPressed += PlayerCharacterInput_OnPauseButtonPressed;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<ScoreCounter>().OnScoreChanged -= ScoreCounter_OnScoreChanged;
            ServiceLocator.Get<PlayerController>().PlayerCharacterInput.OnPauseButtonPressed -= PlayerCharacterInput_OnPauseButtonPressed;
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
            _mainMenuButton.gameObject.SetActive(false);
            _openSettingsButton.gameObject.SetActive(false);
            //_cloversCollectedUI.gameObject.SetActive(false);
        }

        private void PlayerCharacterInput_OnPauseButtonPressed()
        {
            _settingsMenu.Toggle();
        }

        private IEnumerator ShowUIElementsCoroutin()
        {
            yield return PlayButtonAnimations().WaitForCompletion();
            yield return ShowPath().WaitForCompletion();

            _scoreCounter.gameObject.SetActive(true);
            _healthbarUI.gameObject.SetActive(true);
            //_cloversCollectedUI.gameObject.SetActive(true);

            _scoreCanvasGroup.DOFade(1f, _otherElementsAnimationTime).From(0f).SetEase(Ease.InOutQuad);
            _scoreCanvasGroup.transform.DOScale(1f, _otherElementsAnimationTime).From(.75f).SetEase(Ease.OutBounce);

            _healthCanvasGroup.DOFade(1f, _otherElementsAnimationTime).From(0f).SetEase(Ease.InOutQuad);
            _healthCanvasGroup.transform.DOScale(1f, _otherElementsAnimationTime).From(.75f).SetEase(Ease.OutBounce);

            //_cloversCanvasGroup.DOFade(1f, _otherElementsAnimationTime).From(0f).SetEase(Ease.InOutQuad);
            //_cloversCanvasGroup.transform.DOScale(1f, _otherElementsAnimationTime).From(.75f).SetEase(Ease.OutBounce);
        }

        private Sequence PlayButtonAnimations()
        {
            _mainMenuButton.gameObject.SetActive(true);
            _openSettingsButton.gameObject.SetActive(true);

            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(_openSettingsButton.image.DOFade(1f, _buttonAnimationTime).From(0f).SetEase(Ease.Linear))
                .Join(_openSettingsButton.transform.DOScale(1f, _buttonAnimationTime).From(.5f).SetEase(Ease.OutBounce))
                .Append(_mainMenuButton.image.DOFade(1f, _buttonAnimationTime).From(0f).SetEase(Ease.Linear))
                .Join(_mainMenuButton.transform.DOScale(1f, _buttonAnimationTime).From(.5f).SetEase(Ease.OutBounce));

            return sequence;
        }

        private Tween ShowPath()
        {
            _uiSheep.alpha = 0f;
            _flagPersonalHS.alpha = 0f;
            _flagWebHS.alpha = 0f;

            _pathWindowRectTransform.sizeDelta = new Vector2(0f, _pathWindowRectTransform.sizeDelta.y);
            _onScreenPathCanvasGroup.alpha = 1f;

            Sequence sequence = DOTween.Sequence();
            Vector2 pathWidthGoal = new(_pathWindowTargetWidth, _pathWindowRectTransform.sizeDelta.y);
            sequence.Append(_pathWindowRectTransform.DOSizeDelta(pathWidthGoal, _pathAnimationTime)).SetEase(Ease.InOutQuad);

            Vector2 uiSheepStartPosition = new(0f, _uiSheep.GetComponent<RectTransform>().anchoredPosition.y);
            _uiSheep.GetComponent<RectTransform>().anchoredPosition = uiSheepStartPosition;

            _personalHighscore = ServiceLocator.Get<LocalDatabase>().GetHighscore(true);
            _webHighscore = ServiceLocator.Get<LocalDatabase>().GetHighscore(false);

            if(_personalHighscore >= _webHighscore)
            {
                _flagWebHS.gameObject.SetActive(false);
                _flagPersonalHS.transform.localPosition = new Vector2(_pathWindowTargetWidth / 2f - _pathOffset, _flagPersonalHS.transform.localPosition.y);

                sequence
                    .Append(_flagPersonalHS.DOFade(1f, _pathIconsAnimationTime).SetEase(Ease.Linear))
                    .Join(_flagPersonalHS.transform.DOScale(1f, _pathAnimationTime).From(0f).SetEase(Ease.OutBounce));
            }
            else
            {
                _flagWebHS.transform.localPosition = new Vector2(_pathWindowTargetWidth / 2f - _pathOffset, _flagWebHS.transform.localPosition.y);

                if (_personalHighscore != 0)
                {
                    float t = (float)_personalHighscore / _webHighscore;

                    _flagPersonalHS.transform.localPosition =
                        Vector2.Lerp(
                            new Vector2(-_pathWindowTargetWidth / 2f + _pathOffset, _flagPersonalHS.transform.localPosition.y),
                            new Vector2(_pathWindowTargetWidth / 2f - _pathOffset, _flagPersonalHS.transform.localPosition.y),
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
                .Join(_uiSheep.transform.DOScale(new Vector2(1f, 1f), _pathIconsAnimationTime).From(0f).SetEase(Ease.OutBounce));

            ServiceLocator.Get<ScoreCounter>().OnScoreChanged += ScoreCounter_OnScoreChanged;
            return sequence;
        }

        private void ScoreCounter_OnScoreChanged(int newScore)
        {
            float t = (float)newScore / _highestOverall;

            _uiSheep.transform.localPosition =
                Vector2.Lerp(
                    new Vector2(-_pathWindowTargetWidth / 2f + _pathOffset, _uiSheep.transform.localPosition.y),
                    new Vector2(_pathWindowTargetWidth / 2f - _pathOffset, _uiSheep.transform.localPosition.y),
                    t);
        }
    }
}