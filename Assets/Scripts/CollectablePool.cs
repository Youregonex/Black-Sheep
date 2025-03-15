using Youregone.LevelGeneration;
using System.Collections.Generic;
using Youregone.Factories;
using UnityEngine;

namespace Youregone.ObjectPooling
{
    public class CollectablePool
    {
        private Queue<Collectable> _regularCollectableQueue = new();
        private Queue<Collectable> _rareCollectableQueue = new();
        private CollectableFactory _collectableFactory;
        private Collectable _regularCollectablePrefab;
        private Collectable _rareCollectablePrefab;

        public CollectablePool(CollectableFactory collectableFactory, Collectable regularCollectablePrefab, Collectable rareCollectablePrefab)
        {
            _collectableFactory = collectableFactory;
            _regularCollectablePrefab = regularCollectablePrefab;
            _rareCollectablePrefab = rareCollectablePrefab;
        }

        public void CollectableEnqueue(Collectable collectable, bool rareCollectable)
        {
            collectable.gameObject.SetActive(false);

            if (rareCollectable)
                _rareCollectableQueue.Enqueue(collectable);
            else
                _regularCollectableQueue.Enqueue(collectable);
        }

        public Collectable CollectableDequeue(bool rareCollectable, Transform parent = null)
        {
            Collectable collectable;

            if(rareCollectable)
            {
                if (_rareCollectableQueue.Count == 0)
                    collectable = _collectableFactory.CreateCollectable(_rareCollectablePrefab);
                else
                    collectable = _rareCollectableQueue.Dequeue();
            }
            else
            {
                if (_regularCollectableQueue.Count == 0)
                    collectable = _collectableFactory.CreateCollectable(_regularCollectablePrefab);
                else
                    collectable = _regularCollectableQueue.Dequeue();
            }

            if (parent != null)
                collectable.transform.parent = parent;

            collectable.gameObject.SetActive(true);
            return collectable;
        }
    }
}