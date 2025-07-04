using System.Collections.Generic;
using UnityEngine;
using Youregone.YPlayerController;
using Youregone.SL;
using Youregone.UI;
using Youregone.SoundFX;

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
        private Music _music;
        private List<PausableMonoBehaviour> _pausableObjectList = new();

        private void Start()
        {
            _playerController = ServiceLocator.Get<PlayerController>();

            _playerInput = _playerController.PlayerCharacterInput;
            _playerInput.OnPauseButtonPressed += PlayerCharacterInput_OnPauseButtonPressed;

            _gameState = ServiceLocator.Get<GameState>();

            _music = ServiceLocator.Get<Music>();
            ServiceLocator.Get<GameScreenUI>().OnPauseToggleRequest += GameScreenUI_OnPauseToggleRequest;
        }

        private void GameScreenUI_OnPauseToggleRequest()
        {
            if(_gameState.GameStarted)
                TogglePause();
        }

        private void PlayerCharacterInput_OnPauseButtonPressed()
        {
            if(_gameState.GameStarted)
                TogglePause();
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
            if (_gamePaused) return;

            _music.PauseMusic();
            _gamePaused = true;
            _pauseBackground.gameObject.SetActive(true);

            foreach (PausableMonoBehaviour pausable in _pausableObjectList)
                pausable.Pause();
        }

        private void UnpauseGame()
        {
            if (!_gamePaused) return;

            _music.ResumeMusic();
            _gamePaused = false;
            _pauseBackground.gameObject.SetActive(false);

            foreach (PausableMonoBehaviour pausable in _pausableObjectList)
                pausable.Unpause();
        }

        private void TogglePause()
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