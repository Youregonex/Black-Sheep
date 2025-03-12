using UnityEngine;
using TMPro;
using Youregone.PlayerControls;

namespace Youregone.UI
{
    public class ScoreCounter : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [Header("Test")]
        [SerializeField] private float score = 0;
        [SerializeField] private bool _isPlayerDead = false;

        private PlayerController _player;

        private void Start()
        {
            _player = PlayerController.instance;
            _player.OnDeath += Death;
        }

        private void Update()
        {
            if (_isPlayerDead || _player.CurrentSpeed <= 0)
                return;

            score += Time.deltaTime;
            _scoreText.text = ((int)score).ToString();
        }

        private void OnDestroy()
        {
            _player.OnDeath -= Death;
        }

        public void AddPoints(int points) => score += points;

        private void Death()
        {
            _isPlayerDead = true;
        }
    }
}