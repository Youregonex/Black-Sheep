using UnityEngine;
using System.Collections.Generic;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Obstacle Config")]
        [SerializeField] private float _obstacleSpawnTimeMin;
        [SerializeField] private float _obstacleSpawnTimeMax;
        [SerializeField] private float _obstacleSpawnRangeMin;
        [SerializeField] private float _obstacleSpawnRangeMax;
        [SerializeField] private float _obstacleYOffset;

        [SerializeField] private List<Obstacle> _obstacleList;

        [Header("Test")]
        [SerializeField] private float _nextObstacleTimer;
        [SerializeField] private bool _canSpawn = true;

        private void Awake()
        {
            _nextObstacleTimer = UnityEngine.Random.Range(_obstacleSpawnTimeMin, _obstacleSpawnTimeMax);
        }

        private void Start()
        {
            PlayerController.instance.OnDeath += StopSpawning;
        }

        private void Update()
        {
            if (!_canSpawn)
                return;

            if (_nextObstacleTimer > 0)
                _nextObstacleTimer -= Time.deltaTime;
            else
                SpawnObstacle();
        }

        private void OnDestroy()
        {
            PlayerController.instance.OnDeath -= StopSpawning;
        }

        private void StopSpawning()
        {
            _canSpawn = false;
        }

        private void SpawnObstacle()
        {
            _nextObstacleTimer = UnityEngine.Random.Range(_obstacleSpawnTimeMin, _obstacleSpawnTimeMax);
            int randomObstacleIndex = UnityEngine.Random.Range(0, _obstacleList.Count);

            Vector2 obstacleRangeToPlayer = new(UnityEngine.Random.Range(_obstacleSpawnRangeMin, _obstacleSpawnRangeMax), _obstacleYOffset);

            Obstacle spawnedObstacle = Instantiate(_obstacleList[randomObstacleIndex], obstacleRangeToPlayer, Quaternion.identity);
            MovingObjectHandler.instance.AddObject(spawnedObstacle);

            float obstacleMoveSpeed = PlayerController.instance.IsRaming ? PlayerController.instance.RamMoveSpeed : PlayerController.instance.BaseMoveSpeed;
            spawnedObstacle.StartMovement(obstacleMoveSpeed);
        }
    }
}