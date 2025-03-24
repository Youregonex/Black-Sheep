using UnityEngine;
using UnityEngine.UI;
using Youregone.UI;
using Youregone.GameCamera;
using Youregone.PlayerControls;
using Youregone.SaveSystem;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Youregone.SL;

namespace Youregone.GameSystems
{
    public class GameState : PausableMonoBehaviour, IService
    {
        private const float TRANSITION_CAMERA_Y_OFFSET = 48; 

        [CustomHeader("Config")]
        [SerializeField] private CameraGameStartSequence _camera;
        [SerializeField] private SpriteRenderer _transitionPrefab;
        [SerializeField] private GameObject _introGameObject;
        [SerializeField] private List<OutroScene> _outroScenes;
        [SerializeField] private float _outroDelay;
        [SerializeField] private float _sceneReloadDelay;

        [CustomHeader("UI Elements")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private CanvasGroup _highScoreCanvasGroup;
        [SerializeField] private TextMeshProUGUI _highScoreText;

        [CustomHeader("DOTWeen Config")]
        [SerializeField] private float _introTransitionDuration;
        [SerializeField] private float _introButtonFadeTime;
        [SerializeField] private float _outroTransitionDuration;

        [CustomHeader("Debug")]
        [SerializeField] private bool _sosal;
        [SerializeField] private TextMeshProUGUI _sosalText;
        [SerializeField] private float _sosalDuration;
        [SerializeField] private EGameState _currentGameState;
        [SerializeField] private SpriteRenderer _transition;

        private Coroutine _startGameCoroutine;
        private Coroutine _sceneReloadCoroutine;
        private GameSettings _gameSettings;

        public EGameState CurrentGameState => _currentGameState;

        private void Awake()
        {
            _currentGameState = EGameState.Intro;
            _camera.OnCameraInPosition += CameraGameStartSequence_OnCameraInPosition;
        }

        protected override void Start()
        {
            base.Start();

            _gameSettings = ServiceLocator.Get<GameSettings>();
            ServiceLocator.Get<PlayerController>().OnDeath += PlayerController_OnDeath;
            ServiceLocator.Get<GameScreenUI>().OnGameReloadRequested += GameScreenUI_OnGameReloadRequested;

            SetupButtons();
            _highScoreCanvasGroup.alpha = 0;

            if (_gameSettings.SkipIntro)
            {
                if (_startGameCoroutine != null)
                    return;

                _startGameCoroutine = StartCoroutine(StartGameSequenceCoroutine());
                return;
            }

            _camera.PlayCameraIntroSequence();
        }

        private void OnDestroy()
        {
            _camera.OnCameraInPosition -= CameraGameStartSequence_OnCameraInPosition;
            ServiceLocator.Get<PlayerController>().OnDeath -= PlayerController_OnDeath;
            ServiceLocator.Get<GameScreenUI>().OnGameReloadRequested -= GameScreenUI_OnGameReloadRequested;
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

        private void GameScreenUI_OnGameReloadRequested()
        {
            if (_sceneReloadCoroutine != null)
            {
                StopCoroutine(_sceneReloadCoroutine);
                _sceneReloadCoroutine = null;
            }

            SceneReload();
        }

        private void SetupButtons()
        {
            _playButton.interactable = false;
            _exitButton.interactable = false;

            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(() =>
            {
                if (_startGameCoroutine != null)
                    return;

                _startGameCoroutine = StartCoroutine(StartGameSequenceCoroutine());
            });

            _exitButton.onClick.RemoveAllListeners();
            _exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }

        private IEnumerator PlayTransitionStartCoroutine()
        {
            _transition = Instantiate(_transitionPrefab);
            _transition.transform.position = new Vector3(_camera.transform.position.x,
                                                         _camera.transform.position.y + TRANSITION_CAMERA_Y_OFFSET,
                                                         0f);

            yield return null;

            Vector3 transitionGoalPosition = new(_camera.transform.position.x, _camera.transform.position.y, 0f);
            _transition.transform.DOMove(transitionGoalPosition, _introTransitionDuration);

            yield return new WaitForSeconds(_introTransitionDuration);
        }

        private IEnumerator PlayTransitionEndCoroutine()
        {
            if (_transition == null)
                yield break;

            Vector3 transitionYOffset = new(0f, 18f, 0f);
            Vector3 transitionGoalPosition = _transition.transform.position - transitionYOffset;
            _transition.transform.DOMove(transitionGoalPosition, _introTransitionDuration);

            yield return new WaitForSeconds(_introTransitionDuration);

            Destroy(_transition.gameObject);

            _transition = null;
        }

        private IEnumerator PlayFullTransitionCoroutine()
        {
            yield return StartCoroutine(PlayTransitionStartCoroutine());
            yield return StartCoroutine(PlayTransitionEndCoroutine());
        }

        private void CameraGameStartSequence_OnCameraInPosition()
        {
            int highScore = ServiceLocator.Get<PlayerPrefsSaverLoader>().GetHighScore();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_playButton.image.DOFade(1f, _introButtonFadeTime).SetEase(Ease.Linear));
            sequence.Append(_exitButton.image.DOFade(1f, _introButtonFadeTime).SetEase(Ease.Linear));

            if(highScore != 0)
            {
                _highScoreText.text = highScore.ToString();
                sequence.Append(_highScoreCanvasGroup.DOFade(1f, _introButtonFadeTime));
            }

            sequence.OnComplete(() =>
            {
                _playButton.interactable = true;
                _exitButton.interactable = true;

            });

            sequence.Play();
        }

        private IEnumerator SceneReloadDelayCoroutine()
        {
            yield return new WaitForSeconds(_sceneReloadDelay);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void SceneReload()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private IEnumerator StartGameSequenceCoroutine()
        {
            if (_currentGameState == EGameState.Gameplay)
                yield break;

            if(_gameSettings.SkipIntro)
            {
                ShowUI();
                _currentGameState = EGameState.Gameplay;

                _camera.MoveCameraToGamePoint();
                Destroy(_introGameObject);
                yield break;
            }

            yield return StartCoroutine(PlayTransitionStartCoroutine());

            if (_sosal)
            {
                _sosalText.gameObject.SetActive(true);
                yield return new WaitForSeconds(_sosalDuration);

                _sosalText.gameObject.SetActive(false);
                yield return new WaitForSeconds(1f);
            }

            if(_transition != null)
                _transition.transform.position = new Vector3(_camera.CameraGamePoint.position.x, _camera.CameraGamePoint.position.y, 0f);

            _camera.MoveCameraToGamePoint();

            yield return StartCoroutine(PlayTransitionEndCoroutine());

            ShowUI();

            _currentGameState = EGameState.Gameplay;
            _startGameCoroutine = null;
            Destroy(_introGameObject); 
        }

        private IEnumerator PlayOutroCoroutine()
        {
            HideUI();

            yield return StartCoroutine(PlayTransitionStartCoroutine());

            foreach (OutroScene outroScene in _outroScenes)
            {
                OutroScene currentOutroScene = Instantiate(outroScene);
                currentOutroScene.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f);

                yield return StartCoroutine(PlayTransitionEndCoroutine());

                yield return StartCoroutine(currentOutroScene.ShowTextCoroutine());

                yield return new WaitUntil(() => Input.anyKeyDown);

                yield return StartCoroutine(PlayTransitionStartCoroutine());

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
            int currentScore = (int)ServiceLocator.Get<GameScreenUI>().ScoreCounter.CurrentScore;
            if (currentScore > ServiceLocator.Get<PlayerPrefsSaverLoader>().GetHighScore())
                ServiceLocator.Get<PlayerPrefsSaverLoader>().SaveHighScore(currentScore);

            _currentGameState = EGameState.Outro;

            yield return new WaitForSeconds(_outroDelay);

            if(!ServiceLocator.Get<GameSettings>().ShowOutro)
            {
                _sceneReloadCoroutine = StartCoroutine(SceneReloadDelayCoroutine());
                yield break;
            }

            StartCoroutine(PlayOutroCoroutine());
        }
    }
}