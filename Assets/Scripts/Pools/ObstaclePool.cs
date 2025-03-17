using System.Collections.Generic;
using UnityEngine;
using Youregone.LevelGeneration;
using Youregone.Factories;

namespace Youregone.ObjectPooling
{
    public class ObstaclePool
    {
        private Queue<Obstacle> _rockSmallPool = new();
        private Queue<Obstacle> _rockSmallMidPool = new();
        private Queue<Obstacle> _rockMidPool = new();
        private Queue<Obstacle> _rockMidBigPool = new();
        private Queue<Obstacle> _rockBigPool = new();

        private Obstacle _rockSmallPrefab;
        private Obstacle _rockSmallMidPrefab;
        private Obstacle _rockMidPrefab;
        private Obstacle _rockMidBigPrefab;
        private Obstacle _rockBigPrefab;

        private Factory<Obstacle> _obstacleFactory;

        // Order of items in obstaclePrefabs is crucial (small -> big)
        public ObstaclePool(Factory<Obstacle> factory, List<Obstacle> obstaclePrefabs)
        {
            _obstacleFactory = factory;

            _rockSmallPrefab = obstaclePrefabs[0];
            _rockSmallMidPrefab = obstaclePrefabs[1];
            _rockMidPrefab = obstaclePrefabs[2];
            _rockMidBigPrefab = obstaclePrefabs[3];
            _rockBigPrefab = obstaclePrefabs[4];
        }

        public void EnqueueObstacle(Obstacle obstacle)
        {
            obstacle.gameObject.SetActive(false);

            switch (obstacle.ObstacleSO.obstacleType)
            {
                case EObstacleType.Rock_Small:

                    _rockSmallPool.Enqueue(obstacle);

                    break;

                case EObstacleType.Rock_SmallMid:

                    _rockSmallMidPool.Enqueue(obstacle);

                    break;

                case EObstacleType.Rock_Mid:

                    _rockMidPool.Enqueue(obstacle);

                    break;

                case EObstacleType.Rock_MidBig:

                    _rockMidBigPool.Enqueue(obstacle);

                    break;

                case EObstacleType.Rock_Big:

                    _rockBigPool.Enqueue(obstacle);

                    break;

                default:
                    Debug.LogError("Wrong Obstacle Type");
                    break;
            }
        }

        public Obstacle DequeueObstacle(EObstacleType obstacleType, Transform parent = null)
        {
            Obstacle pooledObstacle;

            switch(obstacleType)
            {
                case EObstacleType.Rock_Small:

                    if (_rockSmallPool.Count == 0)
                        pooledObstacle = _obstacleFactory.Create(_rockSmallPrefab);
                    else
                        pooledObstacle = _rockSmallPool.Dequeue();

                    break;

                case EObstacleType.Rock_SmallMid:

                    if (_rockSmallMidPool.Count == 0)
                        pooledObstacle = _obstacleFactory.Create(_rockSmallMidPrefab);
                    else
                        pooledObstacle = _rockSmallMidPool.Dequeue();

                    break;

                case EObstacleType.Rock_Mid:

                    if (_rockMidPool.Count == 0)
                        pooledObstacle = _obstacleFactory.Create(_rockMidPrefab);
                    else
                        pooledObstacle = _rockMidPool.Dequeue();

                    break;

                case EObstacleType.Rock_MidBig:

                    if (_rockMidBigPool.Count == 0)
                        pooledObstacle = _obstacleFactory.Create(_rockMidBigPrefab);
                    else
                        pooledObstacle = _rockMidBigPool.Dequeue();

                    break;

                case EObstacleType.Rock_Big:

                    if (_rockBigPool.Count == 0)
                        pooledObstacle = _obstacleFactory.Create(_rockBigPrefab);
                    else
                        pooledObstacle = _rockBigPool.Dequeue();

                    break;

                default:
                    Debug.LogError("Wrong Obstacle Type");
                    pooledObstacle = null;
                    break;
            }

            if(parent != null)
                pooledObstacle.transform.parent = parent;

            pooledObstacle.gameObject.SetActive(true);
            return pooledObstacle;
        }
    }
}