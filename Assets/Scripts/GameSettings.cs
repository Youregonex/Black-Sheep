using UnityEngine;
using Youregone.SL;
using Youregone.UI;

namespace Youregone.SaveSystem
{
    public class GameSettings : MonoBehaviour, IService
    {
        [Header("Config")]
        [SerializeField] private bool _showIntro;
        [SerializeField] private bool _showOutro;

        private PlayerPrefsSaverLoader _playerPrefsSaverLoader;
        public bool ShowOutro
        {
            get
            {
                if (_playerPrefsSaverLoader == null)
                    _playerPrefsSaverLoader = ServiceLocator.Get<PlayerPrefsSaverLoader>();

                return _playerPrefsSaverLoader.OutroEnabled;
            }
        } 

        private void Start()
        {
            if(_playerPrefsSaverLoader == null)
                _playerPrefsSaverLoader = ServiceLocator.Get<PlayerPrefsSaverLoader>();

            ServiceLocator.Get<GameScreenUI>().OnGameOutroToggleRequest += GameScreenUI_OnGameOutroToggleRequest;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<GameScreenUI>().OnGameOutroToggleRequest -= GameScreenUI_OnGameOutroToggleRequest;
        }

        private void GameScreenUI_OnGameOutroToggleRequest()
        {
            _playerPrefsSaverLoader.ToggleOutroEnable();
        }
    }
}