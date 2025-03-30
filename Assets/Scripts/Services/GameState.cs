using UnityEngine;
using Youregone.UI;
using Youregone.YCamera;
using Youregone.YPlayerController;
using Youregone.SaveSystem;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Youregone.SL;

namespace Youregone.GameSystems
{
    public class GameState : PausableMonoBehaviour, IService
    {
        [CustomHeader("Config")]
        [SerializeField] private GameCamera _camera;
        [SerializeField] private List<OutroScene> _outroScenes;
        [SerializeField] private float _outroDelay;
        [SerializeField] private float _sceneReloadDelay;

        [CustomHeader("UI Elements")]
        [SerializeField] private DeathScreenUI _deathScreenUI;

        [CustomHeader("DOTWeen Config")]
        [SerializeField] private float _outroTransitionDuration;

        [CustomHeader("Debug")]
        [SerializeField] private EGameState _currentGameState;

        private Transition _transition;

        public EGameState CurrentGameState => _currentGameState;

        private void Initialize()
        {
            _transition = ServiceLocator.Get<Transition>();
            ServiceLocator.Get<PlayerController>().OnDeath += PlayerController_OnDeath;
            ServiceLocator.Get<GameScreenUI>().OnMainMenuLoadRequest += GameScreenUI_OnMainMenuLoadRequest;

            _deathScreenUI.OnTryAgainButtonPressed += DeathScreenUI_OnTryAgainButtonPressed;
            _deathScreenUI.OnMainMenuButtonPressed += DeathScreenUI_OnMainMenuButtonPressed;
        }

        private void Awake()
        {
            _currentGameState = EGameState.PreGameplay;
        }

        protected override void Start()
        {
            base.Start();

            HideUI();
            Initialize();

            StartCoroutine(StartGameCoroutine());
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().OnDeath -= PlayerController_OnDeath;
            ServiceLocator.Get<GameScreenUI>().OnMainMenuLoadRequest -= GameScreenUI_OnMainMenuLoadRequest;

            _deathScreenUI.OnTryAgainButtonPressed += DeathScreenUI_OnTryAgainButtonPressed;
            _deathScreenUI.OnMainMenuButtonPressed += DeathScreenUI_OnMainMenuButtonPressed;
        }

        public override void Pause()
        {
            if (_currentGameState != EGameState.Gameplay)
                return;

            _currentGameState = EGameState.Pause;
        }

        public override void Unpause()
        {
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

        private IEnumerator SceneReloadDelayCoroutine()
        {
            yield return new WaitForSeconds(_sceneReloadDelay);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void ReloadScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator StartGameCoroutine()
        {
            yield return StartCoroutine(_transition.PlayTransitionEnd());

            ShowUI();
            _currentGameState = EGameState.Gameplay;
        }

        private IEnumerator PlayOutroCoroutine()
        {
            HideUI();

            yield return _transition.StartCoroutine(_transition.PlayTransitionStart());

            foreach (OutroScene outroScene in _outroScenes)
            {
                OutroScene currentOutroScene = Instantiate(outroScene);
                currentOutroScene.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f);

                yield return _transition.StartCoroutine(_transition.PlayTransitionEnd());

                yield return StartCoroutine(currentOutroScene.ShowTextCoroutine());

                yield return new WaitUntil(() => Input.anyKeyDown);

                yield return _transition.StartCoroutine(_transition.PlayTransitionStart());

                Destroy(currentOutroScene.gameObject);
            }

            StartCoroutine(SceneReloadDelayCoroutine());
        }

        private void PlayerController_OnDeath()
        {
            StartCoroutine(PlayerController_OnDeath_Coroutine());
        }

        private void ShowUI()
        {
            ServiceLocator.Get<GameScreenUI>().ScoreCounter.gameObject.SetActive(true);
            ServiceLocator.Get<GameScreenUI>().HealthbarUI.gameObject.SetActive(true);
            ServiceLocator.Get<GameScreenUI>().GameOutroDisableButton.gameObject.SetActive(true);
            ServiceLocator.Get<GameScreenUI>().MainMenuButton.gameObject.SetActive(true);
        }

        private void HideUI()
        {
            ServiceLocator.Get<GameScreenUI>().ScoreCounter.gameObject.SetActive(false);
            ServiceLocator.Get<GameScreenUI>().HealthbarUI.gameObject.SetActive(false);
            ServiceLocator.Get<GameScreenUI>().GameOutroDisableButton.gameObject.SetActive(false);
            ServiceLocator.Get<GameScreenUI>().MainMenuButton.gameObject.SetActive(false);
        }

        private IEnumerator PlayerController_OnDeath_Coroutine()
        {
            _currentGameState = EGameState.Outro;

            yield return new WaitForSeconds(_outroDelay);

            if (!ServiceLocator.Get<GameSettings>().OutroEnabled)
            {
                _deathScreenUI.ShowWindow();
                SaveHighScore();

                yield break;
            }

            SaveHighScore();
            StartCoroutine(PlayOutroCoroutine());
        }

        private void SaveHighScore()
        {
            int currentScore = (int)ServiceLocator.Get<GameScreenUI>().ScoreCounter.CurrentScore;
            if (currentScore > ServiceLocator.Get<PlayerPrefsSaverLoader>().GetHighScore())
                ServiceLocator.Get<PlayerPrefsSaverLoader>().SaveHighScore(currentScore);
        }
    }
}