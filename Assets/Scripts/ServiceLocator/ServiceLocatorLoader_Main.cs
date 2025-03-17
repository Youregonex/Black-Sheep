using UnityEngine;
using Youregone.PlayerControls;
using Youregone.GameSystems;
using Youregone.SaveSystem;
using Youregone.LevelGeneration;
using Youregone.UI;

namespace Youregone.SL
{
    public class ServiceLocatorLoader_Main : MonoBehaviour
    {
        [Header("Services to register")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private GameState _gameState;
        [SerializeField] private HighScoreSaver _highScoreSaver;
        [SerializeField] private MovingObjectHandler _movingObjectHandler;
        [SerializeField] private MovingObjectSpawner _movingObjectSpawner;
        [SerializeField] private PauseManager _pauseManager;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private ScoreCounter _scoreCounter;
        [SerializeField] private HealthbarUI _healthbarUI;

        private void Awake()
        {
            ServiceLocator.Register<PlayerController>(_playerController);
            ServiceLocator.Register<GameState>(_gameState);
            ServiceLocator.Register<HighScoreSaver>(_highScoreSaver);
            ServiceLocator.Register<MovingObjectHandler>(_movingObjectHandler);
            ServiceLocator.Register<MovingObjectSpawner>(_movingObjectSpawner);
            ServiceLocator.Register<PauseManager>(_pauseManager);
            ServiceLocator.Register<SoundManager>(_soundManager);
            ServiceLocator.Register<ScoreCounter>(_scoreCounter);
            ServiceLocator.Register<HealthbarUI>(_healthbarUI);
        }
    }
}