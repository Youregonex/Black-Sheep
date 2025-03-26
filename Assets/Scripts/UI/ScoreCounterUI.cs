using UnityEngine;
using TMPro;
using Youregone.PlayerControls;
using Youregone.GameSystems;
using Youregone.SL;

namespace Youregone.UI
{
    public class ScoreCounterUI : PausableMonoBehaviour, IUpdateObserver, IService
    {
        [CustomHeader("Config")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [CustomHeader("Debug")]
        [SerializeField] private float _score;

        public float CurrentScore => _score;

        private GameState _gameState;
        private PlayerController _player;

        private void Initialize()
        {
            _gameState = ServiceLocator.Get<GameState>();
            _player = ServiceLocator.Get<PlayerController>();
            _score = 0;
        }

        private void OnEnable()
        {
            UpdateManager.RegisterUpdateObserver(this);
        }

        protected override void Start()
        {
            base.Start();
            Initialize();
        }

        public void ObservedUpdate()
        {
            if (_player == null || _gameState.CurrentGameState != EGameState.Gameplay)
                return;

            float scoreModifier = .5f;
            _score += Time.deltaTime * (_player.CurrentSpeed * scoreModifier);
            _scoreText.text = ((int)_score).ToString();
        }

        private void OnDisable()
        {
            UpdateManager.UnregisterUpdateObserver(this);
        }

        public void AddPoints(int points) => _score += points;
        public override void Pause() { }
        public override void Unpause() { }
    }
}