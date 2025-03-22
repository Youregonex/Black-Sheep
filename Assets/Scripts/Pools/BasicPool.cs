using System.Collections.Generic;
using UnityEngine;
using Youregone.Factories;

namespace Youregone.ObjectPooling
{
    public class BasicPool<T> where T : MonoBehaviour
    {
        private Queue<T> _poolQueue = new();

        private Factory<T> _poolFactory;
        private T _poolObjectPrefab;
        private Transform _parentTransform;

        public BasicPool(Factory<T> poolFactory, T poolObjectPrefab, Transform parentTransform, int initialSize = 0)
        {
            _poolFactory = poolFactory;
            _poolObjectPrefab = poolObjectPrefab;
            _parentTransform = parentTransform;

            if (initialSize > 0)
                CreateInitialPoolObjects(initialSize);
        }

        public void Enqueue(T poolObject)
        {
            poolObject.gameObject.SetActive(false);
            _poolQueue.Enqueue(poolObject);
        }

        public T Dequeue()
        {
            T poolObject;

            if (_poolQueue.Count == 0)
            {
                poolObject = _poolFactory.Create(_poolObjectPrefab);

                if(_parentTransform != null)
                    poolObject.transform.parent = _parentTransform;
            }
            else
                poolObject = _poolQueue.Dequeue();

            poolObject.gameObject.SetActive(true);
            return poolObject;
        }

        private void CreateInitialPoolObjects(int initialSize)
        {
            for (int i = 0; i < initialSize; i++)
            {
                T poolObject = _poolFactory.Create(_poolObjectPrefab);

                if (_parentTransform != null)
                    poolObject.transform.parent = _parentTransform;

                Enqueue(poolObject);
            }
        }
    }
}