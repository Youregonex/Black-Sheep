using System.Collections.Generic;
using UnityEngine;
using Youregone.PlayerControls;
using Youregone.GameSystems;

namespace Youregone.LevelGeneration
{
    public class MovingObjectHandler : PausableMonoBehaviour
    {
        public static MovingObjectHandler instance;

        [Header("Debug")]
        [SerializeField] private List<MovingObject> _spawnedObjects;

        private PlayerController _player;

        private void Awake()
        {
            instance = this;
        }

        protected override void Start()
        {
            base.Start();

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

        public override void Pause()
        {
            StopObjects();
        }

        public override void UnPause()
        {
            ChangeVelocity();
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
    }
}