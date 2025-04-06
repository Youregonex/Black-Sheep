using UnityEngine;
using Youregone.SL;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Youregone.GameSystems;

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

        public HealthbarUI HealthbarUI => _healthbarUI;
        public ScoreCounterUI ScoreCounter => _scoreCounter;
        public Button MainMenuButton => _mainMenuButton;
        public Button OutroDisableButton => _outroDisableButton;

        private void Start()
        {
            _mainMenuButton.onClick.AddListener(() =>
            {
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

        public void ShowScreenUI()
        {
            _scoreCounter.gameObject.SetActive(true);
            _healthbarUI.gameObject.SetActive(true);
            _outroDisableButton.gameObject.SetActive(true);
            _mainMenuButton.gameObject.SetActive(true);
            _pauseButton.gameObject.SetActive(true);
        }

        public void HideScreenUI()
        {
            _scoreCounter.gameObject.SetActive(false);
            _healthbarUI.gameObject.SetActive(false);
            _outroDisableButton.gameObject.SetActive(false);
            _mainMenuButton.gameObject.SetActive(false);
            _pauseButton.gameObject.SetActive(false);
        }
    }
}