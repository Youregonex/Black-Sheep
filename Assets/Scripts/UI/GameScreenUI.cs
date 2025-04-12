using UnityEngine;
using Youregone.SL;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Youregone.GameSystems;
using Youregone.SaveSystem;

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
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _outroDisableButton;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Sprite _outroDisableButtonSpriteOn;
        [SerializeField] private Sprite _outroDisableButtonSpriteOff;
        [SerializeField] private CloversCollectedUI _cloversCollectedUI;
        [SerializeField] private Sprite _mainMenuButtonPressedSprite;
        [SerializeField] private CanvasGroup _onScreenPathCanvasGroup;
        [SerializeField] private RectTransform _uiSheep;
        [SerializeField] private RectTransform _flagWebHS;
        [SerializeField] private RectTransform _flagPersonalHS;
        [SerializeField] private RectTransform _flagParent;

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
        public Button OutroDisableButton => _outroDisableButton;
        public CloversCollectedUI CloversCollectedUI => _cloversCollectedUI;

        private void Start()
        {
            _mainMenuButton.onClick.AddListener(() =>
            {
                _mainMenuButton.image.sprite = _mainMenuButtonPressedSprite;
                OnMainMenuLoadRequest?.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            });

            _outroDisableButton.onClick.AddListener(() =>
            {
                OnGameOutroToggleRequest?.Invoke();
                _outroDisableButton.image.sprite = !ServiceLocator.Get<GameSettings>().OutroEnabled ? _outroDisableButtonSpriteOff : _outroDisableButtonSpriteOn;
                EventSystem.current.SetSelectedGameObject(null);
            });

            _pauseButton.onClick.AddListener(() =>
            {
                OnPauseButtonPressed?.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            });

            _outroDisableButton.image.sprite = ServiceLocator.Get<GameSettings>().OutroEnabled ? _outroDisableButtonSpriteOn : _outroDisableButtonSpriteOff;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<ScoreCounter>().OnScoreChanged -= ScoreCounter_OnScoreChanged;
        }

        public void ShowScreenUI()
        {
            _scoreCounter.gameObject.SetActive(true);
            _healthbarUI.gameObject.SetActive(true);
            _outroDisableButton.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(true);
            _pauseButton.gameObject.SetActive(true);
            _cloversCollectedUI.gameObject.SetActive(true);

            ShowPath();
        }

        public void HideScreenUI()
        {
            _onScreenPathCanvasGroup.alpha = 0f;
            _scoreCounter.gameObject.SetActive(false);
            _healthbarUI.gameObject.SetActive(false);
            _outroDisableButton.gameObject.SetActive(false);
            _mainMenuButton.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(false);
            _cloversCollectedUI.gameObject.SetActive(false);
        }

        private void ShowPath()
        {
            _onScreenPathCanvasGroup.alpha = 1f;

            _personalHighscore = ServiceLocator.Get<LocalDatabase>().GetHighscore(true);
            _webHighscore = ServiceLocator.Get<LocalDatabase>().GetHighscore(false);

            if(_personalHighscore >= _webHighscore)
            {
                _flagPersonalHS.transform.localPosition = new Vector2(_flagParent.sizeDelta.x / 2, _flagPersonalHS.transform.localPosition.y);

                _flagWebHS.gameObject.SetActive(false);
            }
            else
            {
                _flagWebHS.transform.localPosition = new Vector2(_flagParent.sizeDelta.x / 2, _flagWebHS.transform.localPosition.y);

                float t = (float)_personalHighscore / _webHighscore;

                _flagPersonalHS.transform.localPosition = 
                    Vector2.Lerp(
                        new Vector2(-_flagParent.sizeDelta.x / 2, _flagPersonalHS.localPosition.y),
                        new Vector2(_flagParent.sizeDelta.x / 2, _flagPersonalHS.localPosition.y),
                        t);
            }

            ServiceLocator.Get<ScoreCounter>().OnScoreChanged += ScoreCounter_OnScoreChanged;
        }

        private void ScoreCounter_OnScoreChanged(int newScore)
        {
            float t = (float)newScore / _highestOverall;

            _uiSheep.transform.localPosition =
                Vector2.Lerp(
                    new Vector2(-_flagParent.sizeDelta.x / 2, _uiSheep.localPosition.y),
                    new Vector2(_flagParent.sizeDelta.x / 2, _uiSheep.localPosition.y),
                    t);
        }
    }
}