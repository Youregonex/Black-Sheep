using UnityEngine;
using TMPro;
using Youregone.PlayerControls;

namespace Youregone.UI
{
    public class ScoreCounter : MonoBehaviour
    {
        public static ScoreCounter instance;

        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private float score = 0;
        [SerializeField] private bool _isPlayerDead = false;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            PlayerController.instance.OnDeath += Death;
        }

        private void Update()
        {
            if (_isPlayerDead)
                return;

            score += Time.deltaTime;
            _scoreText.text = ((int)score).ToString();
        }

        private void OnDestroy()
        {
            PlayerController.instance.OnDeath -= Death;
        }

        public void AddPoints(int points) => score += points;

        private void Death()
        {
            _isPlayerDead = true;
        }
    }
}