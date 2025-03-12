using UnityEngine;
using UnityEngine.UI;
using Youregone.UI;
using Youregone.Camera;
using Youregone.PlayerControls;
using DG.Tweening;
using System.Collections;

namespace Youregone.State
{
    public class GameState : MonoBehaviour
    {
        public static GameState instance;

        [Header("Config")]
        [SerializeField] private bool _testMode;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private CameraGameStartSequence _camera;
        [SerializeField] private SpriteRenderer _transition;

        [Header("DOTWeen Cofig")]
        [SerializeField] private float _transitionDuration;
        [SerializeField] private float _transitionScaleMax;
        [SerializeField] private float _transitionScaleMin;

        [Header("Test")]
        [SerializeField] private EGameState _currentGameState;

        private Coroutine _startGameCoroutine;

        public EGameState CurrentGameState => _currentGameState;

        private void Awake()
        {
            instance = this;

            _currentGameState = EGameState.Intro;
        }

        private void Start()
        {
            PlayerController.instance.OnDeath += PlayerController_OnDeath;

            ResetTransition();

            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(() =>
            {
                if (_startGameCoroutine != null)
                    return;

                _startGameCoroutine = StartCoroutine(StartGame());
            });

            _exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });

            if (_testMode)
            {
                if (_startGameCoroutine != null)
                    return;

                _startGameCoroutine = StartCoroutine(StartGame());
                return;
            }

            _camera.StartCameraSequence();
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

                _camera.StartGame();
                yield break;
            }

            _transition.gameObject.SetActive(true);
            _transition.transform.DOScale(_transitionScaleMax, _transitionDuration);
            yield return new WaitForSeconds(_transitionDuration);

            _transition.transform.position = new Vector3(_camera.CameraGamePoint.position.x, _camera.CameraGamePoint.position.y, 0f);
            _camera.StartGame();

            _transition.transform.DOScale(_transitionScaleMin, _transitionDuration);
            yield return new WaitForSeconds(_transitionDuration);

            _transition.gameObject.SetActive(false);

            UIManager.instance.ScoreCounter.gameObject.SetActive(true);
            UIManager.instance.HealthbarUI.gameObject.SetActive(true);
            _currentGameState = EGameState.Gameplay;

            Destroy(_transition.gameObject);

            _startGameCoroutine = null;
        }

        private void ResetTransition()
        {
            _transition.transform.position = new Vector3(_camera.CameraEndPoint.position.x, _camera.CameraEndPoint.position.y, 0f);
            _transition.gameObject.SetActive(false);
            _transition.transform.localScale = new Vector3(_transitionScaleMin, _transitionScaleMin, _transitionScaleMin);
        }

        private void PlayerController_OnDeath()
        {
            _currentGameState = EGameState.Outro;
        }
    }

    public enum EGameState
    {
        Intro,
        Gameplay,
        Outro
    }
}