using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using Youregone.SL;
using Youregone.SaveSystem;

namespace Youregone.UI
{
    public class DeathScreenUI : MonoBehaviour
    {
        public event Action OnTryAgainButtonPressed;
        public event Action OnMainMenuButtonPressed;

        [CustomHeader("Components")]
        [SerializeField] private Button _tryAgainButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private TextMeshProUGUI _currentScoreText;

        private void Awake()
        {
            _tryAgainButton.onClick.AddListener(()=>
            {
                OnTryAgainButtonPressed?.Invoke();
                _tryAgainButton.interactable = false;
                _mainMenuButton.interactable = false;
            });

            _mainMenuButton.onClick.AddListener(() =>
            {
                OnMainMenuButtonPressed?.Invoke();
                _tryAgainButton.interactable = false;
                _mainMenuButton.interactable = false;
            });
        }

        public void ShowWindow()
        {
            gameObject.SetActive(true);

            _currentScoreText.text = ((int)ServiceLocator.Get<GameScreenUI>().ScoreCounter.CurrentScore).ToString();
            _highScoreText.text = ServiceLocator.Get<PlayerPrefsSaverLoader>().GetHighScore().ToString();
        }
    }
}