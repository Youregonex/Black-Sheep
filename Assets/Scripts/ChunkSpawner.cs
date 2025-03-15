using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;
using Youregone.GameSystems;

namespace Youregone.LevelGeneration
{
    public class PlatformSpawner : MonoBehaviour, IUpdateObserver
    {
        private const string OBSTACLE_PARENT_NAME = "Rock";
        private const string COLLECTABLES_PARENT_NAME = "PickUp";

        [Header("Preferences")]
        [SerializeField] private float _playerDistanceSpawnChunk;

        [Header("Chunk Settings")]
        [SerializeField] private Chunk _startingChunk;
        [SerializeField] private List<Chunk> _pitChunkPrefabList;
        [SerializeField] private List<Chunk> _bridgeChunkPrefab;
        [SerializeField] private List<Chunk> _platformChunkPrefabList;
        [SerializeField, Range(0f, 1f)] private float _pitChunkSpawnChance;
        [SerializeField] private float _bridgeChunkSpawnCooldown;
        [SerializeField] private Transform _chunkParentTransform;

        [Header("Debug")]
        [SerializeField] private Chunk _lastChunk;
        [SerializeField] private Chunk _nextChunk;
        [SerializeField] private bool _canSpawn;
        [SerializeField] private float _bridgeChunkSpawnCooldownCurrent;

        [Header("Test")]
        [SerializeField] private float _pitSpawnChanceMidDifficulty;
        [SerializeField] private float _bridgeCooldownMidDifficulty;
        [SerializeField] private float _pitSpawnChanceMaxDifficulty;
        [SerializeField] private float _bridgeCooldownMaxDifficulty;

        private PlayerController _player;
        private MovingObjectSpawner _movingObjectSpawner;

        public void OnEnable()
        {
            UpdateManager.RegisterUpdateObserver(this);
        }

        private void Start()
        {
            _player = PlayerController.instance;
            _player.OnDeath += StopSpawning;

            _movingObjectSpawner = MovingObjectSpawner.instance;
            _movingObjectSpawner.OnMaxDifficultyReached += MovingObjectSpawner_OnMaxDifficultyReached;
            _movingObjectSpawner.OnMidDifficultyReached += MovingObjectSpawner_OnMidDifficultyReached;

            _bridgeChunkSpawnCooldownCurrent = _bridgeChunkSpawnCooldown;
            _lastChunk = SpawnNextChunk();
        }

        public void ObservedUpdate()
        {
            if (_bridgeChunkSpawnCooldownCurrent > 0)
                _bridgeChunkSpawnCooldownCurrent -= Time.deltaTime;

            if (!_canSpawn)
                return;

            if (_player == null)
                return;

            if (_lastChunk == null)
                return;

            if (Vector2.Distance(_player.transform.position, _lastChunk.EndTransform.position) <= _playerDistanceSpawnChunk)
            {
                _lastChunk = SpawnNextChunk();
            }
        }

        public void OnDisable()
        {
            UpdateManager.UnregisterUpdateObserver(this);
        }

        private void OnDestroy()
        {
            _player.OnDeath -= StopSpawning;
            _movingObjectSpawner.OnMaxDifficultyReached -= MovingObjectSpawner_OnMaxDifficultyReached;
            _movingObjectSpawner.OnMidDifficultyReached -= MovingObjectSpawner_OnMidDifficultyReached;
        }

        private void MovingObjectSpawner_OnMidDifficultyReached()
        {
            _pitChunkSpawnChance = _pitSpawnChanceMidDifficulty;
            _bridgeChunkSpawnCooldown = _bridgeCooldownMidDifficulty;
        }

        private void MovingObjectSpawner_OnMaxDifficultyReached()
        {
            _pitChunkSpawnChance = _pitSpawnChanceMaxDifficulty;
            _bridgeChunkSpawnCooldown = _bridgeCooldownMaxDifficulty;
        }

        private void StopSpawning()
        {
            _canSpawn = false;
        }

        private Chunk SpawnNextChunk()
        {
            Vector2 nextChunkSpawnPosition;
            Chunk chunkToSpawn;

            if (_lastChunk == null)
            {
                chunkToSpawn = _startingChunk;
                nextChunkSpawnPosition = Vector2.zero;
            }
            else
            {
                chunkToSpawn = _nextChunk;
                nextChunkSpawnPosition = new Vector2(_lastChunk.EndTransform.position.x, 0f);
            }

            Chunk spawnedChunk = Instantiate(chunkToSpawn, nextChunkSpawnPosition, Quaternion.identity);
            spawnedChunk.transform.parent = _chunkParentTransform;
            spawnedChunk.StartMovement(_player.CurrentSpeed);

            _nextChunk = PickNextChunk(spawnedChunk);

            if(_nextChunk.ChunkType != ChunkType.Bridge)
                PlaceObstacles(spawnedChunk);

            PlaceCollectables(spawnedChunk);

            return spawnedChunk;
        }

        private Chunk PickNextChunk(Chunk lastChunk)
        {
            if (lastChunk.ChunkType != ChunkType.Pit && lastChunk.ChunkType != ChunkType.Bridge && _bridgeChunkSpawnCooldownCurrent <= 0)
            {
                _bridgeChunkSpawnCooldownCurrent = _bridgeChunkSpawnCooldown;
                return _bridgeChunkPrefab[UnityEngine.Random.Range(0, _bridgeChunkPrefab.Count)];
            }

            if(UnityEngine.Random.Range(0f, 1f) <= _pitChunkSpawnChance)
                return _pitChunkPrefabList[UnityEngine.Random.Range(0, _pitChunkPrefabList.Count)];

            return _platformChunkPrefabList[UnityEngine.Random.Range(0, _platformChunkPrefabList.Count)];
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
    }
}