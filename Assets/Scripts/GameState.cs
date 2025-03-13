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

namespace Youregone.State
{
    public class GameState : MonoBehaviour
    {
        public static GameState instance;

        [Header("Config")]
        [SerializeField] private bool _testMode;
        [SerializeField] private CameraGameStartSequence _camera;
        [SerializeField] private SpriteRenderer _transitionPrefab;
        [SerializeField] private float _sceneReloadDelay;
        [Header("UI Elements")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private CanvasGroup _highScoreCanvasGroup;
        [SerializeField] private TextMeshProUGUI _highScoreText;

        [Header("DOTWeen Config")]
        [SerializeField] private float _transitionDuration;
        [SerializeField] private float _buttonFadeTime;

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

        private void Start()
        {
            PlayerController.instance.OnDeath += PlayerController_OnDeath;

            _transition = Instantiate(_transitionPrefab);

            ResetTransition();
            SetupButtons();
            _highScoreCanvasGroup.alpha = 0;

            if (_testMode)
            {
                if (_startGameCoroutine != null)
                    return;

                _startGameCoroutine = StartCoroutine(StartGame());
                return;
            }

            _camera.PlayCameraIntroSequence();
        }

        private void OnDestroy()
        {
            _camera.OnCameraInPosition -= CameraGameStartSequence_OnCameraInPosition;
            PlayerController.instance.OnDeath -= PlayerController_OnDeath;
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

                _startGameCoroutine = StartCoroutine(StartGame());
            });

            _exitButton.onClick.RemoveAllListeners();
            _exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }

        private void CameraGameStartSequence_OnCameraInPosition()
        {
            int highScore = HighScoreSaver.instance.GetHighScore();

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_playButton.image.DOFade(1f, _buttonFadeTime));
            sequence.Append(_exitButton.image.DOFade(1f, _buttonFadeTime));

            if(highScore != 0)
            {
                _highScoreText.text = highScore.ToString();
                sequence.Append(_highScoreCanvasGroup.DOFade(1f, _buttonFadeTime));
            }

            sequence.OnComplete(() =>
            {
                _playButton.interactable = true;
                _exitButton.interactable = true;

            });

            sequence.Play();
        }

        private IEnumerator SceneReloadDelay()
        {
            yield return new WaitForSeconds(_sceneReloadDelay);

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private IEnumerator StartGame()
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

            Vector3 transitionGoalPosition;

            if (_transition != null)
            {
                _transition.gameObject.SetActive(true);
                transitionGoalPosition = new(_camera.transform.position.x, _camera.transform.position.y, 0f);
                _transition.transform.DOMove(transitionGoalPosition, _transitionDuration);
            }

            yield return new WaitForSeconds(_transitionDuration);

            if(_sosal)
            {
                _sosalText.gameObject.SetActive(true);
                yield return new WaitForSeconds(_sosalDuration);

                _sosalText.gameObject.SetActive(false);
                yield return new WaitForSeconds(1f);
            }

            _transition.transform.position = new Vector3(_camera.CameraGamePoint.position.x, _camera.CameraGamePoint.position.y, 0f);
            _camera.MoveCamraToGamePoint();

            Vector3 transitionYOffset = new(0f, 18f, 0f);
            transitionGoalPosition = _transition.transform.position - transitionYOffset;
            _transition.transform.DOMove(transitionGoalPosition, _transitionDuration);

            yield return new WaitForSeconds(_transitionDuration);

            Destroy(_transition.gameObject);

            UIManager.instance.ScoreCounter.gameObject.SetActive(true);
            UIManager.instance.HealthbarUI.gameObject.SetActive(true);

            _currentGameState = EGameState.Gameplay;
            _startGameCoroutine = null;
        }

        private void ResetTransition()
        {
            _transition.transform.position = new Vector3(_camera.CameraStartPoint.position.x, _camera.CameraStartPoint.position.y, 0f);
            _transition.gameObject.SetActive(false);
        }

        private void PlayerController_OnDeath()
        {
            int currentScore = (int)UIManager.instance.ScoreCounter.CurrentScore;
            if (currentScore > HighScoreSaver.instance.GetHighScore())
                HighScoreSaver.instance.SaveHighScore(currentScore);
            
            _currentGameState = EGameState.Outro;
            StartCoroutine(SceneReloadDelay());
        }
    }

    public enum EGameState
    {
        Intro,
        Gameplay,
        Outro
    }
}