using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;
using Youregone.UI;
using System;

namespace Youregone.LevelGeneration
{
    public class MovingObjectSpawner : MonoBehaviour
    {
        public static MovingObjectSpawner instance;

        public event Action OnMaxDifficultyReached;
        public event Action OnMidDifficultyReached;

        [Header("Obstacle Config")]
        [SerializeField] private List<Obstacle> _obstaclePrefabList;
        [SerializeField, Range(0, 1f)] private float _obstacleSpawnChance;

        [Header("Collectable Config")]
        [SerializeField] private Collectable _collectablePrefab;
        [SerializeField, Range(0, 1f)] private float _collectableSpawnChance;
        [SerializeField] private Collectable _rareCollectablePrefab;
        [SerializeField, Range(0f, 1f)] private float _rareCollectableSpawnChance;

        [Header("Test")]
        [SerializeField] private bool _progressiveDifficulty;
        [SerializeField] private int _maxDifficultyScore;

        [Header("Debug")]
        [SerializeField] private bool _maxDifficultyReached;
        [SerializeField] private bool _midDifficultyReached;

        private float _midDifficultyScore => _maxDifficultyScore / 2;
        private ScoreCounter _scoreCounter;
        private PlayerController _player;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            _player = PlayerController.instance;
            _scoreCounter = UIManager.instance.ScoreCounter;
        }

        private void Update()
        {
            if (_progressiveDifficulty && !_maxDifficultyReached)
            {
                _obstacleSpawnChance = _scoreCounter.CurrentScore / _maxDifficultyScore;

                if (_scoreCounter.CurrentScore / _midDifficultyScore > 1f && !_midDifficultyReached)
                {
                    _midDifficultyReached = true;
                    OnMidDifficultyReached?.Invoke();
                }

                if (_scoreCounter.CurrentScore / _maxDifficultyScore > 1f)
                {
                    _obstacleSpawnChance = 1f;
                    _maxDifficultyReached = true;
                    OnMaxDifficultyReached?.Invoke();
                }
            }
        }

        public void SpawnObstacle(Vector2 position)
        {
            if (UnityEngine.Random.Range(0f, 1f) > _obstacleSpawnChance)
                return;

            int randomObstacleIndex = UnityEngine.Random.Range(0, _obstaclePrefabList.Count);

            Obstacle spawnedObstacle = Instantiate(_obstaclePrefabList[randomObstacleIndex], position, Quaternion.identity);
            spawnedObstacle.StartMovement(_player.CurrentSpeed);
        }

        public void SpawnCollectable(Vector2 position)
        {
            if (UnityEngine.Random.Range(0f, 1f) > _collectableSpawnChance)
                return;

            Collectable collectableToSpawn;

            if (UnityEngine.Random.Range(0f, 1f) <= _rareCollectableSpawnChance)
                collectableToSpawn = _rareCollectablePrefab;
            else
                collectableToSpawn = _collectablePrefab;

            Collectable spawnedCollectable = Instantiate(collectableToSpawn, position, Quaternion.identity);
            spawnedCollectable.StartMovement(_player.CurrentSpeed);
        }
    }
}