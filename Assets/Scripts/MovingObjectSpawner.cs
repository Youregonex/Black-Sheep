using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class MovingObjectSpawner : MonoBehaviour
    {
        public static MovingObjectSpawner instance;

        [Header("Obstacle Config")]
        [SerializeField] private List<Obstacle> _obstaclePrefabList;
        [SerializeField, Range(0, 1f)] private float _obstacleSpawnChance;

        [Header("Collectable Config")]
        [SerializeField] private Collectable _collectablePrefab;
        [SerializeField, Range(0, 1f)] private float _collectableSpawnChance;

        private void Awake()
        {
            instance = this;
        }

        public void SpawnObstacle(Vector2 position)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= _obstacleSpawnChance)
                return;

            int randomObstacleIndex = UnityEngine.Random.Range(0, _obstaclePrefabList.Count);

            Obstacle spawnedObstacle = Instantiate(_obstaclePrefabList[randomObstacleIndex], position, Quaternion.identity);
            MovingObjectHandler.instance.AddObject(spawnedObstacle);

            spawnedObstacle.StartMovement(PlayerController.instance.CurrentSpeed);
        }

        public void SpawnCollectable(Vector2 position)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= _collectableSpawnChance)
                return;

            Collectable spawnedCollectable = Instantiate(_collectablePrefab, position, Quaternion.identity);
            MovingObjectHandler.instance.AddObject(spawnedCollectable);

            spawnedCollectable.StartMovement(PlayerController.instance.CurrentSpeed);
        }
    }
}