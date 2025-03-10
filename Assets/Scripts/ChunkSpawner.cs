using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class PlatformSpawner : MonoBehaviour
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
        [SerializeField] private List<Chunk> _chunkPrefabList;

        [Header("Test")]
        [SerializeField] private Chunk _lastChunk;
        [SerializeField] private Chunk _nextChunk;
        [SerializeField] private PlayerController _player;
        [SerializeField] private bool _canSpawn;

        private void Start()
        {
            _player = PlayerController.instance;
            _lastChunk = SpawnNextChunk();

            PlayerController.instance.OnDeath += StopSpawning;
        }

        private void Update()
        {
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
            PlayerController.instance.OnDeath -= StopSpawning;
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
            MovingObjectHandler.instance.AddObject(spawnedChunk);

            spawnedChunk.StartMovement(_player.CurrentSpeed);

            PlaceObstacles(spawnedChunk);
            PlaceCollectables(spawnedChunk);

            _nextChunk = PickNextChunk(spawnedChunk);

            return spawnedChunk;
        }

        private Chunk PickNextChunk(Chunk lastChunk)
        {
            if(lastChunk.ChunkType == ChunkType.Pit || lastChunk.ChunkType == ChunkType.Bridge)
            {
                List<Chunk> possibleChunks = new();
                possibleChunks.AddRange(_pitChunkPrefabList);
                possibleChunks.AddRange(_platformChunkPrefabList);

                return possibleChunks[UnityEngine.Random.Range(0, possibleChunks.Count)];
            }

            return _chunkPrefabList[UnityEngine.Random.Range(0, _chunkPrefabList.Count)];
        }

        private void PlaceObstacles(Chunk chunk)
        {
            Transform obstacleParent = FindChunkPointsOfInterest(chunk, OBSTACLE_PARENT_NAME);

            if (obstacleParent == null || obstacleParent.childCount == 0)
                return;

            foreach(Transform child in obstacleParent)
            {
                MovingObjectSpawner.instance.SpawnObstacle(child.position);
            }
        }

        private void PlaceCollectables(Chunk chunk)
        {
            Transform collectableParent = FindChunkPointsOfInterest(chunk, COLLECTABLES_PARENT_NAME);

            if (collectableParent == null || collectableParent.childCount == 0)
                return;

            foreach (Transform child in collectableParent)
            {
                MovingObjectSpawner.instance.SpawnCollectable(child.position);
            }
        }

        private Transform FindChunkPointsOfInterest(Chunk chunk, string parentName)
        {
            Transform parent = chunk.transform.GetChild(0).GetChild(0).GetChild(0).Find(parentName);
            return parent;
        }
    }
}