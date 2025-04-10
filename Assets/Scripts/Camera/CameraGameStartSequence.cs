using UnityEngine;
using DG.Tweening;
using System.Collections;
using Youregone.SL;
using Youregone.SaveSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Youregone.UI;

namespace Youregone.YCamera
{
    public class CameraGameStartSequence : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private float _cameraSequenceDelay;
        [SerializeField] private float _cameraZOffset;
        [SerializeField] private Transform _cameraStartPoint;
        [SerializeField] private Transform _cameraEndPoint;
        [SerializeField] private CanvasGroup _stihCanvasGroup;

        [CustomHeader("UI Elements")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private CanvasGroup _highScoreCanvasGroup;
        [SerializeField] private ShopUI _shopUI;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _cameraMoveDuration;
        [SerializeField] private float _objectFadeTime;

        public Transform CameraStartPoint => _cameraStartPoint;
        public Transform CameraEndPoint => _cameraEndPoint;

        private Tween _currentTween;
        private bool _introSkipped = false;

        private void Awake()
        {
            SetupButtons();
            _highScoreCanvasGroup.alpha = 0;
            _stihCanvasGroup.alpha = 0f;
        }

        private void Start()
        {
            StartCoroutine(PlayCameraIntroSequence());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !_introSkipped)
                ForceCompleteCurrentTween();
        }

        private IEnumerator PlayCameraIntroSequence()
        {
            transform.position = new Vector3(_cameraStartPoint.position.x, _cameraStartPoint.position.y, _cameraZOffset);
            yield return StartCoroutine(ServiceLocator.Get<Transition>().PlayTransitionEnd());

            StartCoroutine(CameraIntroSequenceCoroutine(_cameraSequenceDelay));
        }

        private void ForceCompleteCurrentTween()
        {
            if (_currentTween == null)
                return;

            _introSkipped = true;
            _currentTween.Kill();

            Vector3 cameraDestination = new(_cameraEndPoint.position.x, _cameraEndPoint.position.y, _cameraZOffset);
            _currentTween = transform.DOMove(cameraDestination, _cameraMoveDuration / 3f).OnComplete(() =>
            {
                _currentTween = null;
                OnCameraInPosition();
            });
        }

        private void SetupButtons()
        {
            _playButton.interactable = false;
            _exitButton.interactable = false;
            _shopButton.interactable = false;

            _shopButton.onClick.AddListener(() =>
            {
                if (_shopUI.ShopOpened)
                    return;

                _shopUI.ShowWindow();
            });

            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(() =>
            {
                StartCoroutine(StartGame());
            });

            _exitButton.onClick.RemoveAllListeners();
            _exitButton.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }

        private IEnumerator StartGame()
        {
            yield return StartCoroutine(ServiceLocator.Get<Transition>().PlayTransitionStart());
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private void OnCameraInPosition()
        {
            _stihCanvasGroup.DOFade(1f, _objectFadeTime);

            int highScore = ServiceLocator.Get<LocalDatabase>().Highscore;

            if(highScore == 0)
            {
                if(ServiceLocator.Get<LocalDatabase>().PersonalResults.Count != 0)
                {
                    ServiceLocator.Get<LocalDatabase>().OnLocalDatabaseUpdated += UpdateHighscoreUI;
                }
            }
            else
            {
                UpdateHighscoreUI();
            }

            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(_playButton.image.DOFade(1f, _objectFadeTime))
                .Join(_exitButton.image.DOFade(1f, _objectFadeTime))
                .Join(_shopButton.image.DOFade(1f, _objectFadeTime))
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _playButton.interactable = true;
                    _exitButton.interactable = true;
                    _shopButton.interactable = true;
                });
        }

        private void UpdateHighscoreUI()
        {
            ServiceLocator.Get<LocalDatabase>().OnLocalDatabaseUpdated -= UpdateHighscoreUI;

            int highScore = ServiceLocator.Get<LocalDatabase>().Highscore;
            _highScoreText.text = highScore.ToString();
            _highScoreCanvasGroup.DOFade(1f, _objectFadeTime).From(0f);
        }

        private IEnumerator CameraIntroSequenceCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector3 cameraDestination = new(_cameraEndPoint.position.x, _cameraEndPoint.position.y, _cameraZOffset);
            _currentTween = transform.DOMove(cameraDestination, _cameraMoveDuration).OnComplete(() =>
            {
                _currentTween = null;
                OnCameraInPosition();
            });
        }
    }
}