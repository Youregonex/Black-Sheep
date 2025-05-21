using UnityEngine;
using Youregone.UI;
using Youregone.YCamera;
using Youregone.YPlayerController;
using System;
using System.Collections;
using UnityEngine.SceneManagement;
using Youregone.SL;
using Youregone.SaveSystem;
using Youregone.SoundFX;

namespace Youregone.GameSystems
{
    public class GameState : PausableMonoBehaviour, IService
    {
        public event Action OnGameStarted;

        [CustomHeader("Config")]
        [SerializeField] private GameCamera _camera;
        [SerializeField] private float _outroDelay;
        [SerializeField] private float _sceneReloadDelay;

        [CustomHeader("Debug")]
        [SerializeField] private EGameState _currentGameState;

        private DeathScreenUI _deathScreenUI;
        private Transition _transition;
        private GameScreenUI _gameScreenUI;
        private Music _music;

        private bool _gameStarted;

        public bool GameStarted => _gameStarted;
        public EGameState CurrentGameState => _currentGameState;

        private void Initialize()
        {
            _transition = ServiceLocator.Get<Transition>();
            _gameScreenUI = ServiceLocator.Get<GameScreenUI>();
            _music = ServiceLocator.Get<Music>();
            _gameScreenUI.OnMainMenuLoadRequest += GameScreenUI_OnMainMenuLoadRequest;

            _deathScreenUI = _gameScreenUI.DeathScreenUI;
            _deathScreenUI.OnTryAgainButtonPressed += DeathScreenUI_OnTryAgainButtonPressed;
            _deathScreenUI.OnMainMenuButtonPressed += DeathScreenUI_OnMainMenuButtonPressed;

            ServiceLocator.Get<PlayerController>().OnDeath += PlayerController_OnDeath;
        }

        private void Awake()
        {
            _currentGameState = EGameState.PreGameplay;
        }

        protected override void Start()
        {
            base.Start();

            Initialize();
            HideUI();

            StartCoroutine(StartGameCoroutine());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ServiceLocator.Get<PlayerController>().OnDeath -= PlayerController_OnDeath;
            _gameScreenUI.OnMainMenuLoadRequest -= GameScreenUI_OnMainMenuLoadRequest;

            _deathScreenUI.OnTryAgainButtonPressed -= DeathScreenUI_OnTryAgainButtonPressed;
            _deathScreenUI.OnMainMenuButtonPressed -= DeathScreenUI_OnMainMenuButtonPressed;
        }

        public override void Pause()
        {
            if (_currentGameState != EGameState.Gameplay)
                return;

            _currentGameState = EGameState.Pause;
        }

        public override void Unpause()
        {
            if (_currentGameState != EGameState.Pause)
                return;

            _currentGameState = EGameState.Gameplay;
        }

        private void DeathScreenUI_OnMainMenuButtonPressed()
        {
            StartCoroutine(LoadMainMenuCoroutine());
        }

        private IEnumerator LoadMainMenuCoroutine()
        {
            yield return StartCoroutine(_transition.PlayTransitionStart());
            SceneManager.LoadScene(0);
        }

        private void DeathScreenUI_OnTryAgainButtonPressed()
        {
            StartCoroutine(ReloadSceneCoroutine());
        }

        private IEnumerator ReloadSceneCoroutine()
        {
            yield return StartCoroutine(_transition.PlayTransitionStart());
            ReloadScene();
        }

        private void GameScreenUI_OnMainMenuLoadRequest()
        {
            StartCoroutine(LoadMainMenuCoroutine());
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator StartGameCoroutine()
        {
            if(!ServiceLocator.Get<LocalDatabase>().DatabaseUpdated)
                yield return new WaitUntil(() => ServiceLocator.Get<LocalDatabase>().DatabaseUpdated); 
            
            yield return StartCoroutine(_transition.PlayTransitionEnd());
            _music.StartMusicWithFadeIn();
            ShowUI();
            yield return new WaitUntil(() => Input.anyKeyDown);

            _currentGameState = EGameState.Gameplay;
            _gameStarted = true;
            OnGameStarted?.Invoke();
        }

        private void LocalDatabase_OnLocalDatabaseUpdated()
        {
            ServiceLocator.Get<LocalDatabase>().OnLocalDatabaseUpdated -= LocalDatabase_OnLocalDatabaseUpdated;
        }

        private IEnumerator PlayOutroCoroutine()
        {
            HideUI();

            yield return StartCoroutine(_gameScreenUI.OutroScenePlayer.PlayOutroCoroutine());

            _deathScreenUI.ShowWindow();
        }

        private void PlayerController_OnDeath()
        {
            StartCoroutine(PlayerController_OnDeath_Coroutine());
        }

        private void ShowUI() => _gameScreenUI.ShowUIElements();
        private void HideUI() => _gameScreenUI.HideUIElements();

        private IEnumerator PlayerController_OnDeath_Coroutine()
        {
            _music.PlayGameOverClip();

            _currentGameState = EGameState.Outro;
            yield return new WaitForSeconds(_outroDelay);

            if (!ServiceLocator.Get<GameSettings>().OutroEnabled)
            {
                _deathScreenUI.ShowWindow();
                yield break;
            }

            StartCoroutine(PlayOutroCoroutine());
        }
    }
}