using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class PlatformSpawner : MonoBehaviour
    {
        private const string OBSTACLE_PARENT_NAME = "Rock";


        [Header("Preferences")]
        [SerializeField] private float _playerDistanceSpawnChunk;

        [Header("Chunk Settings")]
        [SerializeField] private Chunk _startingChunk;
        [SerializeField] private List<Chunk> _chunkPrefabList;
        [SerializeField] private float _chunkSpawnOffset;

        [Header("Test")]
        [SerializeField] private Chunk _lastChunk;
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
                int randomChunkIndex = UnityEngine.Random.Range(0, _chunkPrefabList.Count);
                chunkToSpawn = _chunkPrefabList[randomChunkIndex];
                nextChunkSpawnPosition = new Vector2(_lastChunk.EndTransform.position.x + _chunkSpawnOffset, 0f);
            }

            Chunk spawnedChunk = Instantiate(chunkToSpawn, nextChunkSpawnPosition, Quaternion.identity);
            MovingObjectHandler.instance.AddObject(spawnedChunk);

            float chunkMoveSpeed = PlayerController.instance.IsRaming ? PlayerController.instance.RamMoveSpeed : PlayerController.instance.BaseMoveSpeed;
            spawnedChunk.StartMovement(chunkMoveSpeed);

            PlaceObstacles();

            return spawnedChunk;
        }

        private void PlaceObstacles()
        {
            Transform obstacleParent = transform.GetChild(0).GetChild(0).GetChild(0).Find(OBSTACLE_PARENT_NAME);

            if (obstacleParent == null || obstacleParent.GetChild(0) == null)
                return;

            foreach(Transform child in obstacleParent)
            {
                ObstacleSpawner.instance.SpawnObstacle(child.position);
            }
        }
    }
}