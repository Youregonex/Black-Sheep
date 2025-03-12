using UnityEngine;
using UnityEngine.UI;
using Youregone.Camera;
using Youregone.UI;
using Youregone.PlayerControls;

namespace Youregone.State
{
    public class GameState : MonoBehaviour
    {
        public static GameState instance;

        [Header("Config")]
        [SerializeField] private Button _playButton;
        [SerializeField] private CameraGameStartSequence _camera;

        [Header("Test")]
        [SerializeField] private EGameState _currentGameState;

        public EGameState CurrentGameState => _currentGameState;

        private void Awake()
        {
            instance = this;

            _currentGameState = EGameState.Intro;
        }

        private void Start()
        {
            PlayerController.instance.OnDeath += PlayerController_OnDeath;

            _playButton.onClick.AddListener(() =>
            {
                _camera.StartGame();
                UIManager.instance.ScoreCounter.gameObject.SetActive(true);
                UIManager.instance.HealthbarUI.gameObject.SetActive(true);

                _currentGameState = EGameState.Gameplay;
            });
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