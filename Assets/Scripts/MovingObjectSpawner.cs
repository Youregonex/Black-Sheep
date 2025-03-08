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

        [Header("Collectable Config")]
        [SerializeField] private Collectable _collectablePrefab;

        private void Awake()
        {
            instance = this;
        }

        public void SpawnObstacle(Vector2 position)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= .5f)
                return;

            int randomObstacleIndex = UnityEngine.Random.Range(0, _obstaclePrefabList.Count);

            Obstacle spawnedObstacle = Instantiate(_obstaclePrefabList[randomObstacleIndex], position, Quaternion.identity);
            MovingObjectHandler.instance.AddObject(spawnedObstacle);

            float obstacleMoveSpeed = PlayerController.instance.IsRaming ? PlayerController.instance.RamMoveSpeed : PlayerController.instance.BaseMoveSpeed;

            spawnedObstacle.StartMovement(obstacleMoveSpeed);
        }

        public void SpawnCollectable(Vector2 position)
        {
            if (UnityEngine.Random.Range(0f, 1f) <= .75f)
                return;

            Collectable spawnedCollectable = Instantiate(_collectablePrefab, position, Quaternion.identity);
            MovingObjectHandler.instance.AddObject(spawnedCollectable);

            float spawnedCollectableSpeed = PlayerController.instance.IsRaming ? PlayerController.instance.RamMoveSpeed : PlayerController.instance.BaseMoveSpeed;

            spawnedCollectable.StartMovement(spawnedCollectableSpeed);
        }
    }
}