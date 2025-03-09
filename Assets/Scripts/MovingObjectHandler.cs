using System.Collections.Generic;
using UnityEngine;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class MovingObjectHandler : MonoBehaviour
    {
        public static MovingObjectHandler instance;

        [Header("Test")]
        [SerializeField] private List<MovingObject> _spawnedObjects;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            PlayerController.instance.OnRamStart += ChangeVelocity;
            PlayerController.instance.OnRamStop += ChangeVelocity;
            PlayerController.instance.OnDeath += StopObjects;
        }

        private void OnDestroy()
        {
            PlayerController.instance.OnRamStart -= ChangeVelocity;
            PlayerController.instance.OnRamStop -= ChangeVelocity;
            PlayerController.instance.OnDeath -= StopObjects;
        }

        private void StopObjects()
        {
            foreach (MovingObject movingObject in _spawnedObjects)
                movingObject.StopMovement();
        }

        private void ChangeVelocity()
        {
            foreach (MovingObject movingObject in _spawnedObjects)
            {
                Vector2 newObjectVelocity = new(PlayerController.instance.CurrentSpeed, 0f);
                movingObject.ChangeVelocity(newObjectVelocity);
            }
        }

        public void AddObject(MovingObject movingObject)
        {
            if (_spawnedObjects.Contains(movingObject))
                return;

            _spawnedObjects.Add(movingObject);
        }

        public void RemoveObject(MovingObject movingObject)
        {
            if (!_spawnedObjects.Contains(movingObject))
                return;

            _spawnedObjects.Remove(movingObject);
        }
    }
}