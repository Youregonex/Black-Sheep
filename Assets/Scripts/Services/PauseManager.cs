using System.Collections.Generic;
using UnityEngine;
using Youregone.YPlayerController;
using Youregone.SL;
using Youregone.UI;

namespace Youregone.GameSystems
{
    public class PauseManager : MonoBehaviour, IService
    {
        [CustomHeader("Config")]
        [SerializeField] private GameObject _pauseBackground;

        [CustomHeader("Debug")]
        [SerializeField] private bool _gamePaused;

        private PlayerCharacterInput _playerInput;
        private PlayerController _playerController;
        private GameState _gameState;
        private List<PausableMonoBehaviour> _pausableObjectList = new();


        private void Start()
        {
            _playerController = ServiceLocator.Get<PlayerController>();

            _playerInput = _playerController.PlayerCharacterInput;
            _playerInput.OnPauseButtonPressed += PlayerCharacterInput_OnPauseButtonPressed;

            _gameState = ServiceLocator.Get<GameState>();

            ServiceLocator.Get<GameScreenUI>().OnPauseButtonPressed += GameScreenUI_OnPauseButtonPressed;
        }

        private void GameScreenUI_OnPauseButtonPressed()
        {
            PauseTriggered();
        }

        private void PlayerCharacterInput_OnPauseButtonPressed()
        {
            PauseTriggered();
        }

        private void OnDestroy()
        {
            _playerInput.OnPauseButtonPressed -= PlayerCharacterInput_OnPauseButtonPressed;
        }

        public void AddPausableObject(PausableMonoBehaviour pausable)
        {
            if (_pausableObjectList.Contains(pausable))
                return;

            _pausableObjectList.Add(pausable);
        }

        public void RemovePausableObject(PausableMonoBehaviour pausable)
        {
            if (!_pausableObjectList.Contains(pausable))
                return;

            _pausableObjectList.Remove(pausable);
        }

        private void PauseGame()
        {
            _gamePaused = true;
            _pauseBackground.gameObject.SetActive(true);

            foreach (PausableMonoBehaviour pausable in _pausableObjectList)
                pausable.Pause();
        }

        private void UnpauseGame()
        {
            _gamePaused = false;
            _pauseBackground.gameObject.SetActive(false);

            foreach (PausableMonoBehaviour pausable in _pausableObjectList)
                pausable.Unpause();
        }

        private void PauseTriggered()
        {
            if (!(_gameState.CurrentGameState == EGameState.Gameplay || _gameState.CurrentGameState == EGameState.Pause))
                return;

            if (_gamePaused)
            {
                UnpauseGame();
                return;
            }

            PauseGame();
        }
    }
}