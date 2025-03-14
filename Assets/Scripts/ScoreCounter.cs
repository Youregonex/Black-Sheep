using UnityEngine;
using TMPro;
using Youregone.PlayerControls;
using Youregone.GameSystems;

namespace Youregone.UI
{
    public class ScoreCounter : PausableMonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Debug")]
        [SerializeField] private float _score;
        [SerializeField] private bool _isPlayerDead = false;

        public float CurrentScore => _score;

        private GameState _gameState;
        private PlayerController _player;

        protected override void Start()
        {
            base.Start();

            _gameState = GameState.instance;
            _player = PlayerController.instance;
            _player.OnDeath += Death;
            _score = 0;
        }

        private void Update()
        {
            if (_player == null)
                return;

            if (_isPlayerDead || _player.CurrentSpeed <= 0 || _gameState.CurrentGameState == EGameState.Pause)
                return;

            _score += Time.deltaTime;
            _scoreText.text = ((int)_score).ToString();
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