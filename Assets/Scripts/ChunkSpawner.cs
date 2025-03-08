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
        [SerializeField] private List<Chunk> _chunkPrefabList;

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
                nextChunkSpawnPosition = new Vector2(_lastChunk.EndTransform.position.x, 0f);
            }

            Chunk spawnedChunk = Instantiate(chunkToSpawn, nextChunkSpawnPosition, Quaternion.identity);
            MovingObjectHandler.instance.AddObject(spawnedChunk);

            float chunkMoveSpeed = PlayerController.instance.IsRaming ? PlayerController.instance.RamMoveSpeed : PlayerController.instance.BaseMoveSpeed;
            spawnedChunk.StartMovement(chunkMoveSpeed);

            PlaceObstacles(spawnedChunk);
            PlaceCollectables(spawnedChunk);

            return spawnedChunk;
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