using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class ObstacleSpawner : MonoBehaviour
    {
        public static ObstacleSpawner instance;

        [Header("Obstacle Config")]
        [SerializeField] private float _obstacleYOffset;

        [SerializeField] private List<Obstacle> _obstacleList;

        [Header("Test")]
        [SerializeField] private float _nextObstacleTimer;
        [SerializeField] private bool _canSpawn = true;

        private void Awake()
        {
            instance = this;
        }

        private void StopSpawning()
        {
            _canSpawn = false;
        }

        public void SpawnObstacle(Vector2 position)
        {
            int randomObstacleIndex = UnityEngine.Random.Range(0, _obstacleList.Count);

            Obstacle spawnedObstacle = Instantiate(_obstacleList[randomObstacleIndex], position, Quaternion.identity);
            MovingObjectHandler.instance.AddObject(spawnedObstacle);

            float obstacleMoveSpeed = PlayerController.instance.IsRaming ? PlayerController.instance.RamMoveSpeed : PlayerController.instance.BaseMoveSpeed;

            spawnedObstacle.StartMovement(obstacleMoveSpeed);
        }
    }
}