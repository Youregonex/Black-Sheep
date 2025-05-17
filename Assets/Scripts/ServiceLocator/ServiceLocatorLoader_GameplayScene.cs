using UnityEngine;
using Youregone.YPlayerController;
using Youregone.GameSystems;
using Youregone.LevelGeneration;
using Youregone.UI;
using Youregone.SaveSystem;
using Youregone.SoundFX;

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
        [SerializeField] private Music _mainMusic;

        private void Awake()
        {
            ServiceLocator.Register(_playerController);
            ServiceLocator.Register(_gameState);
            ServiceLocator.Register(_movingObjectHandler);
            ServiceLocator.Register(_movingObjectSpawner);
            ServiceLocator.Register(_pauseManager);
            ServiceLocator.Register(_soundManager);
            ServiceLocator.Register(_gameScreenUI);
            ServiceLocator.Register(_gameSettings);
            ServiceLocator.Register(_playerPrefsSaverLoader);
            ServiceLocator.Register(_transition);
            ServiceLocator.Register(_scoreCounter);
            ServiceLocator.Register(_localDatabase);
            ServiceLocator.Register(_playerCloversCollected);
            ServiceLocator.Register(_mainMusic);
        }
    }
}