using UnityEngine;
using Youregone.SL;
using UnityEngine.UI;
using System;
using Youregone.SaveSystem;

namespace Youregone.UI
{
    public class GameScreenUI : MonoBehaviour, IService
    {
        public event Action OnGameReloadRequested;
        public event Action OnGameOutroToggleRequest;

        [Header("Config")]
        [SerializeField] private ScoreCounter _scoreCounter;
        [SerializeField] private HealthbarUI _healthbarUI;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _outroDisableButton;
        [SerializeField] private Sprite _outroDisableButtonSpriteOn;
        [SerializeField] private Sprite _outroDisableButtonSpriteOff;

        public HealthbarUI HealthbarUI => _healthbarUI;
        public ScoreCounter ScoreCounter => _scoreCounter;
        public Button MainMenuButton => _mainMenuButton;
        public Button GameOutroDisableButton => _outroDisableButton;

        private void Start()
        {
            _mainMenuButton.onClick.AddListener(() =>
            {
                OnGameReloadRequested?.Invoke();
            });

            _outroDisableButton.onClick.AddListener(() =>
            {
                OnGameOutroToggleRequest?.Invoke();
                _outroDisableButton.image.sprite = ServiceLocator.Get<GameSettings>().ShowOutro ? _outroDisableButtonSpriteOn : _outroDisableButtonSpriteOff;
            });

            _outroDisableButton.image.sprite = ServiceLocator.Get<GameSettings>().ShowOutro ? _outroDisableButtonSpriteOn : _outroDisableButtonSpriteOff;
        }
    }
}