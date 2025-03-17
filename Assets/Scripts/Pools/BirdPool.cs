using System.Collections.Generic;
using UnityEngine;
using Youregone.Factories;
using Youregone.LevelGeneration;

namespace Youregone.ObjectPooling
{
    public class BirdPool
    {
        private Queue<Bird> _birdQueue = new();

        private Factory<Bird> _birdFactory;
        private Bird _birdPrefab;

        public BirdPool(Factory<Bird> collectableFactory, Bird birdPrefab)
        {
            _birdFactory = collectableFactory;
            _birdPrefab = birdPrefab;
        }

        public void Enqueue(Bird bird)
        {
            bird.gameObject.SetActive(false);
            _birdQueue.Enqueue(bird);
        }

        public Bird Dequeue(Transform parent = null)
        {
            Bird bird;

            if (_birdQueue.Count == 0)
                bird = _birdFactory.Create(_birdPrefab);
            else
                bird = _birdQueue.Dequeue();

            if (parent != null)
                bird.transform.parent = parent;

            bird.gameObject.SetActive(true);
            return bird;
        }
    }
}