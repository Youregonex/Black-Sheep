using UnityEngine;
using UnityEngine.UI;
using Youregone.UI;
using Youregone.Camera;
using Youregone.PlayerControls;
using Youregone.HighScore;
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Youregone.GameSystems
{
    public enum EGameState
    {
        Intro,
        Gameplay,
        Pause,
        Outro
    }

    public class GameState : PausableMonoBehaviour
    {
        public static GameState instance;

        private const float TRANSITION_CAMERA_Y_OFFSET = 48; 

        [Header("Config")]
        [SerializeField] private bool _testMode;
        [SerializeField] private CameraGameStartSequence _camera;
        [SerializeField] private SpriteRenderer _transitionPrefab;
        [SerializeField] private GameObject _introGameObject;
        [SerializeField] private List<OutroScene> _outroScenes;
        [SerializeField] private float _outroDelay;
        [SerializeField] private float _sceneReloadDelay;

        [Header("UI Elements")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private CanvasGroup _highScoreCanvasGroup;
        [SerializeField] private TextMeshProUGUI _highScoreText;

        [Header("DOTWeen Config")]
        [SerializeField] private float _introTransitionDuration;
        [SerializeField] private float _introButtonFadeTime;
        [SerializeField] private float _outroTransitionDuration;

        [Header("Debug")]
        [SerializeField] private bool _sosal;
        [SerializeField] private TextMeshProUGUI _sosalText;
        [SerializeField] private float _sosalDuration;
        [SerializeField] private EGameState _currentGameState;
        [SerializeField] private SpriteRenderer _transition;

        private Coroutine _startGameCoroutine;

        public EGameState CurrentGameState => _currentGameState;

        private void Awake()
        {
            instance = this;

            _currentGameState = EGameState.Intro;
            _camera.OnCameraInPosition += CameraGameStartSequence_OnCameraInPosition;
        }

        protected override void Start()
        {
            base.Start();

            PlayerController.instance.OnDeath += PlayerController_OnDeath;

            SetupButtons();
            _highScoreCanvasGroup.alpha = 0;

            if (_testMode)
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
            PlayerController.instance.OnDeath -= PlayerController_OnDeath;
        }

        public override void Pause()
        {
            if (_currentGameState != EGameState.Gameplay)
                return;

            _currentGameState = EGameState.Pause;
        }

        public override void UnPause()
        {
            _currentGameState = EGameState.Gameplay;
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

        private IEnumerator PlayTransitionStart()
        {
            _transition = Instantiate(_transitionPrefab);
            _transition.transform.position = new Vector3(_camera.transform.position.x,
                                                         _camera.transform.position.y + TRANSITION_CAMERA_Y_OFFSET,
                                                         0f);

            Vector3 transitionGoalPosition = new(_camera.transform.position.x, _camera.transform.position.y, 0f);
            _transition.transform.DOMove(transitionGoalPosition, _introTransitionDuration);

            yield return new WaitForSeconds(_introTransitionDuration);
        }

        private IEnumerator PlayTransitionEnd()
        {
            Vector3 transitionYOffset = new(0f, 18f, 0f);
            Vector3 transitionGoalPosition = _transition.transform.position - transitionYOffset;
            _transition.transform.DOMove(transitionGoalPosition, _introTransitionDuration);

            yield return new WaitForSeconds(_introTransitionDuration);

            Destroy(_transition.gameObject);
        }

        private IEnumerator PlayTransition()
        {
            yield return StartCoroutine(PlayTransitionStart());
            yield return StartCoroutine(PlayTransitionEnd());
        }

        private void CameraGameStartSequence_OnCameraInPosition()
        {
            int highScore = HighScoreSaver.instance.GetHighScore();

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

        private IEnumerator StartGameSequenceCoroutine()
        {
            if (_currentGameState == EGameState.Gameplay)
                yield break;

            if(_testMode)
            {
                UIManager.instance.ScoreCounter.gameObject.SetActive(true);
                UIManager.instance.HealthbarUI.gameObject.SetActive(true);
                _currentGameState = EGameState.Gameplay;

                _camera.MoveCamraToGamePoint();
                yield break;
            }

            yield return StartCoroutine(PlayTransitionStart());

            if (_sosal)
            {
                _sosalText.gameObject.SetActive(true);
                yield return new WaitForSeconds(_sosalDuration);

                _sosalText.gameObject.SetActive(false);
                yield return new WaitForSeconds(1f);
            }

            _transition.transform.position = new Vector3(_camera.CameraGamePoint.position.x, _camera.CameraGamePoint.position.y, 0f);
            _camera.MoveCamraToGamePoint();

            yield return StartCoroutine(PlayTransitionEnd());

            UIManager.instance.ScoreCounter.gameObject.SetActive(true);
            UIManager.instance.HealthbarUI.gameObject.SetActive(true);

            _currentGameState = EGameState.Gameplay;
            _startGameCoroutine = null;
        }

        private IEnumerator PlayOutroCoroutine()
        {
            UIManager.instance.ScoreCounter.gameObject.SetActive(false);
            UIManager.instance.HealthbarUI.gameObject.SetActive(false);

            yield return StartCoroutine(PlayTransitionStart());

            foreach (OutroScene outroScene in _outroScenes)
            {
                OutroScene currentOutroScene = Instantiate(outroScene);
                currentOutroScene.transform.position = new Vector3(_camera.transform.position.x, _camera.transform.position.y, 0f);

                yield return StartCoroutine(PlayTransitionEnd());

                yield return StartCoroutine(currentOutroScene.ShowTextCoroutine());

                yield return new WaitUntil(() => Input.anyKeyDown);

                yield return StartCoroutine(PlayTransitionStart());

                Destroy(currentOutroScene.gameObject);
            }

            StartCoroutine(SceneReloadDelayCoroutine());
        }

        private void PlayerController_OnDeath()
        {
            StartCoroutine(PlayerController_OnDeath_Coroutine());
        }

        private IEnumerator PlayerController_OnDeath_Coroutine()
        {
            int currentScore = (int)UIManager.instance.ScoreCounter.CurrentScore;
            if (currentScore > HighScoreSaver.instance.GetHighScore())
                HighScoreSaver.instance.SaveHighScore(currentScore);

            _currentGameState = EGameState.Outro;

            yield return new WaitForSeconds(_outroDelay);

            StartCoroutine(PlayOutroCoroutine());
        }
    }
}