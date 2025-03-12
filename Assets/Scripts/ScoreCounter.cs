using UnityEngine;
using TMPro;
using Youregone.PlayerControls;

namespace Youregone.UI
{
    public class ScoreCounter : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Debug")]
        [SerializeField] private float _score;
        [SerializeField] private bool _isPlayerDead = false;

        public float CurrentScore => _score;

        private PlayerController _player;

        private void Start()
        {
            _player = PlayerController.instance;
            _player.OnDeath += Death;
            _score = 0;
        }

        private void Update()
        {
            if (_isPlayerDead || _player.CurrentSpeed <= 0)
                return;

            _score += Time.deltaTime;
            _scoreText.text = ((int)_score).ToString();
        }

        private void OnDestroy()
        {
            _player.OnDeath -= Death;
        }

        public void AddPoints(int points) => _score += points;

        private void Death()
        {
            _isPlayerDead = true;
        }
    }
}