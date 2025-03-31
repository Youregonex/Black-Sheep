using UnityEngine;
using System.Collections.Generic;
using Youregone.YPlayerController;
using Youregone.UI;
using System;
using Youregone.Factories;
using Youregone.ObjectPooling;
using Youregone.GameSystems;
using Youregone.SL;

namespace Youregone.LevelGeneration
{
    public class MovingObjectSpawner : MonoBehaviour, IUpdateObserver, IService
    {
        public event Action OnMaxDifficultyReached;
        public event Action OnMidDifficultyReached;

        [CustomHeader("Obstacle Config")]
        [SerializeField] private List<Obstacle> _obstaclePrefabList;
        [SerializeField] private Transform _obstacleParentTransform;

        [CustomHeader("Props config")]
        [SerializeField] private Bird _birdPrefab;
        [SerializeField] private Transform _propsParent;
        [SerializeField] private RockBreakPiece _rockBreakPiecePrefab;
        [SerializeField] private Transform _rockBreakPiecesParent;

        [CustomHeader("Collectable Config")]
        [SerializeField] private Collectable _regularCollectablePrefab;
        [SerializeField] private Collectable _rareCollectablePrefab;
        [SerializeField] private Transform _collectableParentTransform;

        [CustomHeader("Pool config")]
        [SerializeField] private int _initialBirdPoolSize;
        [SerializeField] private int _initialRockBreakPiecePoolSize;

        [CustomHeader("Debug")]
        [SerializeField] private bool _maxDifficultyReached;
        [SerializeField] private bool _midDifficultyReached;

        private float _obstacleSpawnChance;
        private int _maxDifficultyScore;
        private float _midDifficultyScore => _maxDifficultyScore / 2;

        private GameSettings _gameSettings;
        private ScoreCounterUI _scoreCounter;
        private PlayerController _player;
        private Factory<Obstacle> _obstacleFactory = new();
        private Factory<Collectable> _collectableFactory = new();
        private Factory<RockBreakPiece> _breakPieceFactory = new();
        private Factory<Bird> _birdFactory = new();
        private BasicPool<Bird> _birdPool;
        private CollectablePool _collectablPool;
        private ObstaclePool _obstaclePool;
        private BasicPool<RockBreakPiece> _rockBreakPiecePool;

        private void Initialize()
        {
            _gameSettings = ServiceLocator.Get<GameSettings>();
            _maxDifficultyScore = _gameSettings.MaxDifficultyScore;
            _obstacleSpawnChance = _gameSettings.ObstacleSpawnChance;

            _player = ServiceLocator.Get<PlayerController>();
            _scoreCounter = ServiceLocator.Get<GameScreenUI>().ScoreCounter;
        }

        private void Start()
        {
            Initialize();
            CreatePools();

            if (_gameSettings.ProgressiveDifficultyEnabled)
                UpdateManager.RegisterUpdateObserver(this);
        }

        public void ObservedUpdate()
        {
            _obstacleSpawnChance = _scoreCounter.CurrentScore / _maxDifficultyScore;

            if (_scoreCounter.CurrentScore / _midDifficultyScore >= .5f && !_midDifficultyReached)
            {
                _midDifficultyReached = true;
                OnMidDifficultyReached?.Invoke();
            }

            if (_scoreCounter.CurrentScore / _maxDifficultyScore >= 1f && !_maxDifficultyReached)
            {
                _obstacleSpawnChance = 1f;
                _maxDifficultyReached = true;
                OnMaxDifficultyReached?.Invoke();
                UpdateManager.UnregisterUpdateObserver(this);
            }
        }

        private void OnDestroy()
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
            Vector2 currentGameVelocity = new(_player.CurrentSpeed, 0f);
            pooledObstacle.ChangeVelocity(currentGameVelocity);

            SpawnBirdsOnObstacle(pooledObstacle);
        }

        public void SpawnCollectable(Vector2 position)
        {
            if (UnityEngine.Random.Range(0f, 1f) > _gameSettings.CollectableSpawnChance)
                return;

            Collectable pooledCollectable;

            float rareSpawnChance = UnityEngine.Random.Range(0f, 1f);
            if (rareSpawnChance < _gameSettings.RareCollectableSpawnChance)
            {
                pooledCollectable = _collectablPool.DequeueCollectable(true, _collectableParentTransform);
                Debug.Log($"Spawning rare | chance: {_gameSettings.RareCollectableSpawnChance} | rolled: {rareSpawnChance}");
            }    
            
            else
            {
                pooledCollectable = _collectablPool.DequeueCollectable(false, _collectableParentTransform);
            }

            pooledCollectable.transform.position = position;
            pooledCollectable.UpdateYOrigin();
            pooledCollectable.OnDestraction += Collectable_OnDestraction;
            Vector2 collectableVelocity = new(_player.CurrentSpeed, 0f);
            pooledCollectable.ChangeVelocity(collectableVelocity);
        }

        private void SpawnBirdsOnObstacle(Obstacle obstacle)
        {
            List<Transform> birdSpawnPositions = GetAllChildObjects(obstacle.BirdSpawnPointsParent);

            foreach(Transform birdSpawnPoint in birdSpawnPositions)
            {
                if (UnityEngine.Random.Range(0f, 1f) >= _gameSettings.BirdSpawnChance)
                    continue;

                Bird bird = _birdPool.Dequeue();
                bird.transform.position = birdSpawnPoint.position;
                bird.OnDestruction += Bird_OnDestruction;
                Vector2 birdVelocity = new(_player.CurrentSpeed, 0f);
                bird.ChangeVelocity(birdVelocity);
            }
        }

        private void Bird_OnDestruction(Bird bird)
        {
            bird.OnDestruction -= Bird_OnDestruction;
            _birdPool.Enqueue(bird);
        }

        private List<Transform> GetAllChildObjects(Transform parent)
        {
            List<Transform> children = new();

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                children.Add(child.transform);
            }

            return children;
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

            int piecesCount = obstacle.GetBreakPiecesAmount();

            for (int i = 0; i < piecesCount; i++)
            {
                RockBreakPiece rockBreakPiece = _rockBreakPiecePool.Dequeue();
                rockBreakPiece.transform.position = obstacle.transform.position;
                rockBreakPiece.OnDestruction += RockBreakPiece_OnDestruction;
            }
        }

        private void RockBreakPiece_OnDestruction(RockBreakPiece rockBreakPiece)
        {
            rockBreakPiece.OnDestruction -= RockBreakPiece_OnDestruction;
            _rockBreakPiecePool.Enqueue(rockBreakPiece);
        }

        private void CreatePools()
        {
            _collectablPool = new(_collectableFactory, _regularCollectablePrefab, _rareCollectablePrefab);
            _obstaclePool = new(_obstacleFactory, _obstaclePrefabList);
            _birdPool = new(_birdFactory, _birdPrefab, _propsParent, _initialBirdPoolSize);
            _rockBreakPiecePool = new(_breakPieceFactory, _rockBreakPiecePrefab, _rockBreakPiecesParent, _initialRockBreakPiecePoolSize);
        }
    }
}