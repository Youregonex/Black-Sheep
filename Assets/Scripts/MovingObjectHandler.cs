using System.Collections.Generic;
using UnityEngine;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class MovingObjectHandler : MonoBehaviour
    {
        public static MovingObjectHandler instance;

        [Header("Debug")]
        [SerializeField] private List<MovingObject> _spawnedObjects;

        private PlayerController _player;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            _player = PlayerController.instance;

            _player.OnRamStart += ChangeVelocity;
            _player.OnRamStop += ChangeVelocity;
            _player.OnDeath += StopObjects;
        }

        private void OnDestroy()
        {
            _player.OnRamStart -= ChangeVelocity;
            _player.OnRamStop -= ChangeVelocity;
            _player.OnDeath -= StopObjects;
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
                Vector2 newObjectVelocity = new(_player.CurrentSpeed, 0f);
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