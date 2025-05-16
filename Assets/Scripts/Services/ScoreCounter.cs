using UnityEngine;
using System;
using Youregone.SL;
using Youregone.YPlayerController;

namespace Youregone.GameSystems
{
    public class ScoreCounter : MonoBehaviour, IService, IUpdateObserver
    {
        public event Action<int> OnScoreChanged;
        public event Action OnComboPointsAwarded;

        [CustomHeader("Settings")]
        [SerializeField] private float _scoreModifier;
        [SerializeField] private float _pointsPerCombo;

        private float _currentScore;
        private GameState _gameState;
        private PlayerController _player;

        public float CurrentScore
        {
            get => _currentScore;
            private set
            {
                if (value <= 0)
                    return;

                _currentScore = value;
                OnScoreChanged?.Invoke((int)_currentScore);
            }
        }

        private void Initialize()
        {
            _gameState = ServiceLocator.Get<GameState>();
            _player = ServiceLocator.Get<PlayerController>();
        }

        private void OnEnable()
        {
            UpdateManager.RegisterUpdateObserver(this);
        }

        private void Start()
        {
            Initialize();

            _player.OnComboFinished += AwardComboPoints;
        }

        public void ObservedUpdate()
        {
            if (_player == null || _gameState.CurrentGameState != EGameState.Gameplay)
                return;

            AddPassivePoints(Time.deltaTime * (_player.CurrentSpeed * _scoreModifier));
        }

        private void OnDisable()
        {
            UpdateManager.UnregisterUpdateObserver(this);
        }

        private void OnDestroy()
        {
            _player.OnComboFinished -= AwardComboPoints;
        }

        public void AddPoints(float points)
        {
            CurrentScore += points;
        }

        private void AwardComboPoints(int comboLength)
        {
            if (comboLength == 1)
                return;

            CurrentScore += _pointsPerCombo * comboLength;
            OnComboPointsAwarded?.Invoke();
        }

        private void AddPassivePoints(float points)
        {
            CurrentScore += points;
        }
    }
}