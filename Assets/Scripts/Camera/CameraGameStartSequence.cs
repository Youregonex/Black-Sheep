using UnityEngine;
using DG.Tweening;
using System.Collections;
using Youregone.SL;
using Youregone.SaveSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Youregone.UI;
using Youregone.SoundFX;

namespace Youregone.YCamera
{
    public class CameraGameStartSequence : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private float _artMovementDelay;
        [SerializeField] private CanvasGroup _stihCanvasGroup;
        [SerializeField] private RectTransform _artRectTransform;

        [CustomHeader("UI Elements")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _exitButton;
        //[SerializeField] private Button _shopButton;
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private CanvasGroup _highScoreCanvasGroup;
        [SerializeField] private ShopUI _shopUI;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _artMoveDuration;
        [SerializeField] private float _objectFadeTime;

        private Tween _currentTween;
        private bool _introSkipped = false;
        private Transition _transition;
        private Music _music;

        private void Awake()
        {
            DOTween.KillAll();
            _highScoreCanvasGroup.alpha = 0;
            _stihCanvasGroup.alpha = 0f;

            SetupButtons();
        }

        private void Start()
        {
            _transition = ServiceLocator.Get<Transition>();
            _music = ServiceLocator.Get<Music>();

            StartCoroutine(PlayArtIntroSequence());
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && !_introSkipped)
                ForceCompleteCurrentTween();
        }

        private void OnDestroy()
        {
            _currentTween?.Kill();
        }

        private IEnumerator PlayArtIntroSequence()
        {
            yield return StartCoroutine(_transition.PlayTransitionEnd());
            _music.StartMusicWithFadeIn();
            StartCoroutine(ArtMovementCoroutine(_artMovementDelay));
        }

        private void ForceCompleteCurrentTween()
        {
            if (_currentTween == null)
                return;

            _introSkipped = true;
            _currentTween.Kill();
            PlayArtMovementAnimation(_artMoveDuration / 3f);
        }

        private void SetupButtons()
        {
            _playButton.interactable = false;
            _exitButton.interactable = false;
            //_shopButton.interactable = false;

            //_shopButton.onClick.AddListener(() =>
            //{
            //    if (_shopUI.ShopOpened)
            //        return;

            //    _shopUI.ShowWindow();
            //});

            _playButton.onClick.RemoveAllListeners();
            _playButton.onClick.AddListener(() =>
            {
                _playButton.interactable = false;
                _exitButton.interactable = false;
                StartCoroutine(StartGame());
            });

            _exitButton.onClick.RemoveAllListeners();
            _exitButton.onClick.AddListener(() =>
            {
                _playButton.interactable = false;
                _exitButton.interactable = false;
                Application.Quit();
            });
        }

        private IEnumerator StartGame()
        {
            _music.FadeOutMusic();
            yield return StartCoroutine(_transition.PlayTransitionStart());
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }

        private void OnArtInPosition()
        {
            _stihCanvasGroup.DOFade(1f, _objectFadeTime);

            int highScore = ServiceLocator.Get<LocalDatabase>().Highscore;

            if(highScore == 0)
            {
                if(ServiceLocator.Get<LocalDatabase>().PersonalResults.Count != 0)
                    ServiceLocator.Get<LocalDatabase>().OnLocalDatabaseUpdated += UpdateHighscoreUI;
            }
            else
                UpdateHighscoreUI();
            
            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(_playButton.image.DOFade(1f, _objectFadeTime))
                .Join(_exitButton.image.DOFade(1f, _objectFadeTime))
                //.Join(_shopButton.image.DOFade(1f, _objectFadeTime))
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    _playButton.interactable = true;
                    _exitButton.interactable = true;
                    //_shopButton.interactable = true;
                });
        }

        private void UpdateHighscoreUI()
        {
            ServiceLocator.Get<LocalDatabase>().OnLocalDatabaseUpdated -= UpdateHighscoreUI;

            int highScore = ServiceLocator.Get<LocalDatabase>().Highscore;
            _highScoreText.text = highScore.ToString();
            _highScoreCanvasGroup.DOFade(1f, _objectFadeTime).From(0f);
        }

        private IEnumerator ArtMovementCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);
            PlayArtMovementAnimation(_artMoveDuration);
        }

        private void PlayArtMovementAnimation(float duration)
        {
            float artStartPositionY = _artRectTransform.anchoredPosition.y;
            _currentTween = _artRectTransform
                .DOAnchorPos(Vector3.zero, duration)
                .From(new Vector3(0f, artStartPositionY))
                .OnComplete(() =>
                {
                    _currentTween = null;
                    OnArtInPosition();
                });
        }
    }
}