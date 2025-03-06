using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class PlatformSpawner : MonoBehaviour
    {
        [Header("Preferences")]
        [SerializeField] private float _playerDistanceRemoveChunk;
        [SerializeField] private float _playerDistanceSpawnChunk;

        [Header("Chunk Settings")]
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

            if (_lastChunk == null)
                nextChunkSpawnPosition = Vector2.zero;
            else
                nextChunkSpawnPosition = new Vector2(_lastChunk.EndTransform.position.x + _chunkSpawnOffset, 0f);

            int randomChunkIndex = UnityEngine.Random.Range(0, _chunkPrefabList.Count);
            Chunk spawnedChunk = Instantiate(_chunkPrefabList[randomChunkIndex], nextChunkSpawnPosition, Quaternion.identity);
            MovingObjectHandler.instance.AddObject(spawnedChunk);

            float chunkMoveSpeed = PlayerController.instance.IsRaming ? PlayerController.instance.RamMoveSpeed : PlayerController.instance.BaseMoveSpeed;
            spawnedChunk.StartMovement(chunkMoveSpeed);

            return spawnedChunk;
        }
    }
}