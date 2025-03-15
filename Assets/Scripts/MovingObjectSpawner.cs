using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;
using Youregone.UI;
using System;
using Youregone.Factories;
using Youregone.ObjectPooling;
using Youregone.GameSystems;

namespace Youregone.LevelGeneration
{
    public class MovingObjectSpawner : MonoBehaviour, IUpdateObserver
    {
        public static MovingObjectSpawner instance;

        public event Action OnMaxDifficultyReached;
        public event Action OnMidDifficultyReached;

        [Header("Obstacle Config")]
        [SerializeField] private List<Obstacle> _obstaclePrefabList;
        [SerializeField, Range(0, 1f)] private float _obstacleSpawnChance;
        [SerializeField] private Transform _obstacleParentTransform;

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
        private Factory<Collectable> _collectableFactory = new();
        private CollectablePool _collectablPool;
        private Factory<Obstacle> _obstacleFactory = new();
        private ObstaclePool _obstaclePool;

        private void Awake()
        {
            instance = this;

            _collectablPool = new(_collectableFactory, _regularCollectablePrefab, _rareCollectablePrefab);
            _obstaclePool = new(_obstacleFactory, _obstaclePrefabList);
        }

        private void OnEnable()
        {
            UpdateManager.RegisterUpdateObserver(this);
        }

        private void Start()
        {
            _player = PlayerController.instance;
            _scoreCounter = UIManager.instance.ScoreCounter;
        }

        public void ObservedUpdate()
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

        private void OnDisable()
        {
            UpdateManager.UnregisterUpdateObserver(this);
        }

        public void SpawnObstacle(Vector2 position)
        {
            if (UnityEngine.Random.Range(0f, 1f) > _obstacleSpawnChance)
                return;

            int randomObstacleIndex = UnityEngine.Random.Range(0, _obstaclePrefabList.Count);

            Obstacle pooledObstacle = _obstaclePool.DequeueObstacle(_obstaclePrefabList[randomObstacleIndex].ObstacleSO.obstacleType, _obstacleParentTransform);
            pooledObstacle.transform.position = position;
            pooledObstacle.OnDestruction += Obstacle_OnDestruction;
            pooledObstacle.transform.rotation = Quaternion.identity;
            pooledObstacle.StartMovement(_player.CurrentSpeed);
        }

        public void SpawnCollectable(Vector2 position)
        {
            if (UnityEngine.Random.Range(0f, 1f) > _collectableSpawnChance)
                return;

            Collectable pooledCollectable;

            if (UnityEngine.Random.Range(0f, 1f) <= _rareCollectableSpawnChance)
                pooledCollectable = _collectablPool.DequeueCollectable(true, _collectableParentTransform);
            
            else
                pooledCollectable = _collectablPool.DequeueCollectable(false, _collectableParentTransform);

            pooledCollectable.transform.position = position;
            pooledCollectable.UpdateYOrigin();
            pooledCollectable.OnDestraction += Collectable_OnDestraction;
            pooledCollectable.StartMovement(_player.CurrentSpeed);
        }

        private void Collectable_OnDestraction(Collectable collectable)
        {
            collectable.OnDestraction -= Collectable_OnDestraction;
            _collectablPool.EnqueueCollectable(collectable, collectable.IsRareCollectable);
        }

        private void Obstacle_OnDestruction(Obstacle obstacle)
        {
            obstacle.OnDestruction -= Obstacle_OnDestruction;
            _obstaclePool.EnqueueObstacle(obstacle);
        }
    }
}