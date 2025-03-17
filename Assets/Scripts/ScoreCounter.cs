using UnityEngine;
using TMPro;
using Youregone.PlayerControls;
using Youregone.GameSystems;
using Youregone.SL;

namespace Youregone.UI
{
    public class ScoreCounter : PausableMonoBehaviour, IUpdateObserver, IService
    {
        [Header("Config")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Debug")]
        [SerializeField] private float _score;
        [SerializeField] private bool _isPlayerDead = false;

        public float CurrentScore => _score;

        private GameState _gameState;
        private PlayerController _player;

        private void OnEnable()
        {
            UpdateManager.RegisterUpdateObserver(this);
        }

        protected override void Start()
        {
            base.Start();

            _gameState = ServiceLocator.Get<GameState>();
            _player = ServiceLocator.Get<PlayerController>();
            _player.OnDeath += Death;
            _score = 0;
        }

        public void ObservedUpdate()
        {
            if (_player == null)
                return;

            if (_isPlayerDead || _player.CurrentSpeed <= 0 || _gameState.CurrentGameState == EGameState.Pause)
                return;

            _score += Time.deltaTime;
            _scoreText.text = ((int)_score).ToString();
        }

        private void OnDisable()
        {
            UpdateManager.UnregisterUpdateObserver(this);
        }

        private void OnDestroy()
        {
            _player.OnDeath -= Death;
        }

        public void AddPoints(int points) => _score += points;
        public override void Pause() { }
        public override void UnPause() { }

        private void Death()
        {
            _isPlayerDead = true;
        }
    }
}