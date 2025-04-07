using UnityEngine;
using System.Collections.Generic;
using Youregone.YPlayerController;
using Youregone.SL;
using Youregone.GameSystems;
using Youregone.ObjectPooling;
using Youregone.Factories;
using DG.Tweening;

namespace Youregone.LevelGeneration
{
    public class ChunkSpawner : PausableMonoBehaviour
    {
        private const string OBSTACLE_PARENT_NAME = "Rock";
        private const string COLLECTABLES_PARENT_NAME = "PickUp";

        [CustomHeader("Settings")]
        [SerializeField] private float _playerTriggerSpawnRange;

        [CustomHeader("Chunk Settings")]
        [SerializeField] private Chunk _startingChunk;
        [SerializeField] private List<Chunk> _pitChunkPrefabList;
        [SerializeField] private List<Chunk> _bridgeChunkPrefab;
        [SerializeField] private List<Chunk> _platformChunkPrefabList;
        [SerializeField] private Transform _chunkParentTransform;

        [CustomHeader("Debug")]
        [SerializeField] private Chunk _lastChunk;
        [SerializeField] private Chunk _nextChunk;
        [SerializeField] private float _bridgeChunkSpawnCooldownCurrent;

        private float _pitSpawnChance;
        private float _bridgeSpawnCooldownMax;

        private GameSettings _gameSettings;
        private PlayerController _player;
        private MovingObjectSpawner _movingObjectSpawner;
        private ChunkPool _chunkPool;
        private Factory<Chunk> _chunkFactory;

        private Tween _bridgeCooldownTimerTween;

        protected override void Start()
        {
            base.Start();

            InitializePool();

            _player = ServiceLocator.Get<PlayerController>();

            _gameSettings = ServiceLocator.Get<GameSettings>();
            _pitSpawnChance = _gameSettings.PitSpawnChanceStart;
            _bridgeSpawnCooldownMax = _gameSettings.BridgeSpawnCooldownStart;

            _movingObjectSpawner = ServiceLocator.Get<MovingObjectSpawner>();
            _movingObjectSpawner.OnMidDifficultyReached += MovingObjectSpawner_OnMidDifficultyReached;
            _movingObjectSpawner.OnMaxDifficultyReached += MovingObjectSpawner_OnMaxDifficultyReached;

            RestartBridgeCooldownTimer();
            _lastChunk = SpawnNextChunk();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _movingObjectSpawner.OnMidDifficultyReached -= MovingObjectSpawner_OnMidDifficultyReached;
            _movingObjectSpawner.OnMaxDifficultyReached -= MovingObjectSpawner_OnMaxDifficultyReached;
        }

        public override void Pause()
        {
            if (_bridgeCooldownTimerTween != null)
                _bridgeCooldownTimerTween.Pause();
        }

        public override void Unpause()
        {
            if (_bridgeCooldownTimerTween != null)
                _bridgeCooldownTimerTween.Play();
        }

        private void MovingObjectSpawner_OnMidDifficultyReached()
        {
            _pitSpawnChance = _gameSettings.PitSpawnChanceMidGame;
            _bridgeSpawnCooldownMax = _gameSettings.BridgeSpawnCooldownMidGame;
        }

        private void MovingObjectSpawner_OnMaxDifficultyReached()
        {
            _pitSpawnChance = _gameSettings.PitSpawnChanceMax;
            _bridgeSpawnCooldownMax = _gameSettings.BridgeSpawnCooldownMax;
        }

        private Chunk SpawnNextChunk()
        {
            Vector2 chunkSpawnPosition;
            Chunk chunkToSpawn;

            if (_lastChunk == null)
            {
                chunkToSpawn = _startingChunk;
                float cameraHeight = Camera.main.orthographicSize;
                float cameraWidth = cameraHeight * Camera.main.aspect;

                float leftEdge = Camera.main.transform.position.x - cameraWidth;
                chunkSpawnPosition = new Vector2(leftEdge, transform.position.y);
            }
            else
            {
                chunkToSpawn = _nextChunk;
                chunkSpawnPosition = new Vector2(_lastChunk.EndTransform.position.x, 0f);
            }

            Chunk pooledChunk = null;

            if (chunkToSpawn == _startingChunk)
            {
                pooledChunk = Instantiate(chunkToSpawn, chunkSpawnPosition, Quaternion.identity);
                pooledChunk.transform.SetParent(_chunkParentTransform);
                Vector2 chunkVelocity = new(_player.CurrentSpeed, 0f);
                pooledChunk.ChangeVelocity(chunkVelocity);
            }
            else
            {
                pooledChunk = _chunkPool.Dequeue(chunkToSpawn.ChunkType, chunkToSpawn.ChunkID);
                pooledChunk.transform.position = chunkSpawnPosition;
                Vector2 chunkVelocity = new(_player.CurrentSpeed, 0f);
                pooledChunk.ChangeVelocity(chunkVelocity);
                pooledChunk.OnDestruction += EnqueueChunk;
            }


            _nextChunk = PickNextChunk(pooledChunk);

            if(_nextChunk.ChunkType != EChunkType.Bridge)
                PlaceObstacles(pooledChunk);

            PlaceCollectables(pooledChunk);

            pooledChunk.OnPlayerInRange += Chunk_OnPlayerInRange;
            return pooledChunk;
        }

        private void Chunk_OnPlayerInRange(Chunk chunk)
        {
            chunk.OnPlayerInRange -= Chunk_OnPlayerInRange;
            _lastChunk = SpawnNextChunk();
        }

        private void EnqueueChunk(Chunk chunk)
        {
            chunk.OnDestruction -= EnqueueChunk;
            _chunkPool.Enqueue(chunk);
        }

        private Chunk PickNextChunk(Chunk lastChunk)
        {
            if (lastChunk.ChunkType != EChunkType.Pit && lastChunk.ChunkType != EChunkType.Bridge && _bridgeChunkSpawnCooldownCurrent <= 0)
            {
                RestartBridgeCooldownTimer();
                return _bridgeChunkPrefab[UnityEngine.Random.Range(0, _bridgeChunkPrefab.Count)];
            }

            if(UnityEngine.Random.Range(0f, 1f) <= _pitSpawnChance)
                return _pitChunkPrefabList[UnityEngine.Random.Range(0, _pitChunkPrefabList.Count)];

            return _platformChunkPrefabList[UnityEngine.Random.Range(0, _platformChunkPrefabList.Count)];
        }

        private void RestartBridgeCooldownTimer()
        {
            _bridgeChunkSpawnCooldownCurrent = _bridgeSpawnCooldownMax;

            _bridgeCooldownTimerTween = DOTween
            .To(
                () => _bridgeChunkSpawnCooldownCurrent,
                value => _bridgeChunkSpawnCooldownCurrent = value,
                0f,
                _bridgeSpawnCooldownMax)
            .SetEase(Ease.Linear)
            .OnComplete(() => _bridgeCooldownTimerTween = null);
        }

        private void PlaceObstacles(Chunk chunk)
        {
            Transform obstacleParent = FindChunkPointsOfInterest(chunk, OBSTACLE_PARENT_NAME);

            if (obstacleParent == null || obstacleParent.childCount == 0)
                return;

            foreach(Transform child in obstacleParent)
                _movingObjectSpawner.SpawnObstacle(child.position);
        }

        private void PlaceCollectables(Chunk chunk)
        {
            Transform collectableParent = FindChunkPointsOfInterest(chunk, COLLECTABLES_PARENT_NAME);

            if (collectableParent == null || collectableParent.childCount == 0)
                return;

            foreach (Transform child in collectableParent)
            {
                _movingObjectSpawner.SpawnCollectable(child.position);
            }
        }

        private Transform FindChunkPointsOfInterest(Chunk chunk, string parentName)
        {
            Transform parent = chunk.transform.GetChild(0).GetChild(0).GetChild(0).Find(parentName);
            return parent;
        }

        private void InitializePool()
        {
            _chunkFactory = new();

            Dictionary<EChunkType, List<Chunk>> prefabs = new();
            prefabs.Add(_pitChunkPrefabList[0].ChunkType, _pitChunkPrefabList);
            prefabs.Add(_bridgeChunkPrefab[0].ChunkType, _bridgeChunkPrefab);
            prefabs.Add(_platformChunkPrefabList[0].ChunkType, _platformChunkPrefabList);

            _chunkPool = new(prefabs, _chunkFactory, _playerTriggerSpawnRange, _chunkParentTransform);
        }
    }
}