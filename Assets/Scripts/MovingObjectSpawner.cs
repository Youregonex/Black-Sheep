using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;
using Youregone.UI;
using System;
using Youregone.Factories;
using Youregone.ObjectPooling;

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
        [SerializeField] private Collectable _regularCollectablePrefab;
        [SerializeField] private Collectable _rareCollectablePrefab;
        [SerializeField, Range(0, 1f)] private float _collectableSpawnChance;
        [SerializeField, Range(0f, 1f)] private float _rareCollectableSpawnChance;
        [SerializeField] private Transform _collectableParentTransform;

        [Header("Test")]
        [SerializeField] private bool _progressiveDifficulty;
        [SerializeField] private int _maxDifficultyScore;

        [Header("Debug")]
        [SerializeField] private bool _maxDifficultyReached;
        [SerializeField] private bool _midDifficultyReached;

        private float _midDifficultyScore => _maxDifficultyScore / 2;
        private ScoreCounter _scoreCounter;
        private PlayerController _player;
        private CollectableFactory _collectableFactory = new();
        private CollectablePool _collectablPool;

        private void Awake()
        {
            instance = this;

            _collectablPool = new(_collectableFactory, _regularCollectablePrefab, _rareCollectablePrefab);
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

            Collectable pooledCollectable;

            if (UnityEngine.Random.Range(0f, 1f) <= _rareCollectableSpawnChance)
                pooledCollectable = _collectablPool.CollectableDequeue(true, _collectableParentTransform);
            
            else
                pooledCollectable = _collectablPool.CollectableDequeue(false, _collectableParentTransform);

            pooledCollectable.transform.position = position;
            pooledCollectable.UpdateYOrigin();
            pooledCollectable.OnDestraction += Collectable_OnDestraction;
            pooledCollectable.StartMovement(_player.CurrentSpeed);
        }

        private void Collectable_OnDestraction(Collectable collectable)
        {
            collectable.OnDestraction -= Collectable_OnDestraction;
            _collectablPool.CollectableEnqueue(collectable, collectable.IsRareCollectable);
        }
    }
}