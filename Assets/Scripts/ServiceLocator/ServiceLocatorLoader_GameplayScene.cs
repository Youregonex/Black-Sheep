using UnityEngine;
using Youregone.YPlayerController;
using Youregone.GameSystems;
using Youregone.LevelGeneration;
using Youregone.UI;
using Youregone.SaveSystem;

namespace Youregone.SL
{
    public class ServiceLocatorLoader_GameplayScene : MonoBehaviour
    {
        [Header("Services to register")]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private GameState _gameState;
        [SerializeField] private MovingObjectHandler _movingObjectHandler;
        [SerializeField] private MovingObjectSpawner _movingObjectSpawner;
        [SerializeField] private PauseManager _pauseManager;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private GameScreenUI _gameScreenUI;
        [SerializeField] private GameSettings _gameSettings;
        [SerializeField] private PlayerPrefsSaverLoader _playerPrefsSaverLoader;
        [SerializeField] private Transition _transition;
        [SerializeField] private ScoreCounter _scoreCounter;
        [SerializeField] private LocalDatabase _localDatabase;
        [SerializeField] private PlayerCloversCollected _playerCloversCollected;

        private void Awake()
        {
            ServiceLocator.Register<PlayerController>(_playerController);
            ServiceLocator.Register<GameState>(_gameState);
            ServiceLocator.Register<MovingObjectHandler>(_movingObjectHandler);
            ServiceLocator.Register<MovingObjectSpawner>(_movingObjectSpawner);
            ServiceLocator.Register<PauseManager>(_pauseManager);
            ServiceLocator.Register<SoundManager>(_soundManager);
            ServiceLocator.Register<GameScreenUI>(_gameScreenUI);
            ServiceLocator.Register<GameSettings>(_gameSettings);
            ServiceLocator.Register<PlayerPrefsSaverLoader>(_playerPrefsSaverLoader);
            ServiceLocator.Register<Transition>(_transition);
            ServiceLocator.Register<ScoreCounter>(_scoreCounter);
            ServiceLocator.Register<LocalDatabase>(_localDatabase);
            ServiceLocator.Register<PlayerCloversCollected>(_playerCloversCollected);
        }
    }
}