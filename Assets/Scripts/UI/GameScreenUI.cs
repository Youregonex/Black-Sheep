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
        public event Action OnGameReloadRequested;
        public event Action OnGameOutroToggleRequest;

        [CustomHeader("Config")]
        [SerializeField] private ScoreCounterUI _scoreCounter;
        [SerializeField] private HealthbarUI _healthbarUI;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private Button _outroDisableButton;
        [SerializeField] private Sprite _outroDisableButtonSpriteOn;
        [SerializeField] private Sprite _outroDisableButtonSpriteOff;

        public HealthbarUI HealthbarUI => _healthbarUI;
        public ScoreCounterUI ScoreCounter => _scoreCounter;
        public Button MainMenuButton => _mainMenuButton;
        public Button GameOutroDisableButton => _outroDisableButton;

        private void Start()
        {
            _mainMenuButton.onClick.AddListener(() =>
            {
                OnGameReloadRequested?.Invoke();
                EventSystem.current.SetSelectedGameObject(null);
            });

            _outroDisableButton.onClick.AddListener(() =>
            {
                OnGameOutroToggleRequest?.Invoke();
                _outroDisableButton.image.sprite = !ServiceLocator.Get<GameSettings>().ShowOutro ? _outroDisableButtonSpriteOff : _outroDisableButtonSpriteOn;
                EventSystem.current.SetSelectedGameObject(null);
            });

            _outroDisableButton.image.sprite = ServiceLocator.Get<GameSettings>().ShowOutro ? _outroDisableButtonSpriteOn : _outroDisableButtonSpriteOff;
        }
    }
}