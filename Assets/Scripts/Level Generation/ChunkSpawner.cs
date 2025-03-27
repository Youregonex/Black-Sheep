using UnityEngine;
using System.Collections.Generic;
using Youregone.YPlayerController;
using Youregone.SL;
using Youregone.GameSystems;

namespace Youregone.LevelGeneration
{
    public class ChunkSpawner : MonoBehaviour
    {
        private const string OBSTACLE_PARENT_NAME = "Rock";
        private const string COLLECTABLES_PARENT_NAME = "PickUp";

        [CustomHeader("Settings")]
        [SerializeField] private float _playerDistanceSpawnChunk;

        [CustomHeader("Chunk Settings")]
        [SerializeField] private Chunk _startingChunk;
        [SerializeField] private List<Chunk> _pitChunkPrefabList;
        [SerializeField] private List<Chunk> _bridgeChunkPrefab;
        [SerializeField] private List<Chunk> _platformChunkPrefabList;
        [SerializeField] private Transform _chunkParentTransform;

        [CustomHeader("Debug")]
        [SerializeField] private Chunk _lastChunk;
        [SerializeField] private Chunk _nextChunk;
        [SerializeField] private bool _canSpawn;
        [SerializeField] private float _bridgeChunkSpawnCooldownCurrent;

        private float _pitSpawnChance;
        private float _bridgeSpawnCooldown;

        private GameSettings _gameSettings;
        private PlayerController _player;
        private MovingObjectSpawner _movingObjectSpawner;


        private void Start()
        {
            _player = ServiceLocator.Get<PlayerController>();
            _player.OnDeath += StopSpawning;

            _gameSettings = ServiceLocator.Get<GameSettings>();
            _pitSpawnChance = _gameSettings.PitSpawnChanceStart;
            _bridgeSpawnCooldown = _gameSettings.BridgeSpawnCooldownStart;
            _bridgeChunkSpawnCooldownCurrent = _bridgeSpawnCooldown;

            _movingObjectSpawner = ServiceLocator.Get<MovingObjectSpawner>();
            _movingObjectSpawner.OnMidDifficultyReached += MovingObjectSpawner_OnMidDifficultyReached;
            _movingObjectSpawner.OnMaxDifficultyReached += MovingObjectSpawner_OnMaxDifficultyReached;

            _lastChunk = SpawnNextChunk();
        }

        private void Update()
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


        private void OnDestroy()
        {
            _player.OnDeath -= StopSpawning;
            _movingObjectSpawner.OnMidDifficultyReached -= MovingObjectSpawner_OnMidDifficultyReached;
            _movingObjectSpawner.OnMaxDifficultyReached -= MovingObjectSpawner_OnMaxDifficultyReached;
        }

        private void MovingObjectSpawner_OnMidDifficultyReached()
        {
            _pitSpawnChance = _gameSettings.PitSpawnChanceMidGame;
            _bridgeSpawnCooldown = _gameSettings.BridgeSpawnCooldownMidGame;
        }

        private void MovingObjectSpawner_OnMaxDifficultyReached()
        {
            _pitSpawnChance = _gameSettings.PitSpawnChanceMax;
            _bridgeSpawnCooldown = _gameSettings.BridgeSpawnCooldownMax;
        }

        private void StopSpawning()
        {
            _canSpawn = false;
        }

        private Chunk SpawnNextChunk()
        {
            Vector2 chunkSpawnPosition;
            Chunk chunkToSpawn;

            if (_lastChunk == null)
            {
                chunkToSpawn = _startingChunk;
                chunkSpawnPosition = Vector2.zero;
            }
            else
            {
                chunkToSpawn = _nextChunk;
                chunkSpawnPosition = new Vector2(_lastChunk.EndTransform.position.x, 0f);
            }

            Chunk spawnedChunk = Instantiate(chunkToSpawn, chunkSpawnPosition, Quaternion.identity);
            Vector2 chunkVelocity = new(_player.CurrentSpeed, 0f);
            spawnedChunk.ChangeVelocity(chunkVelocity);
            spawnedChunk.transform.parent = _chunkParentTransform;

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
                _bridgeChunkSpawnCooldownCurrent = _bridgeSpawnCooldown;
                return _bridgeChunkPrefab[UnityEngine.Random.Range(0, _bridgeChunkPrefab.Count)];
            }

            if(UnityEngine.Random.Range(0f, 1f) <= _pitSpawnChance)
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