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

        private float _currentSpeed
        {
            get
            {
                if (_player != null)
                    return _player.CurrentSpeed;
                else
                    return 0;
            }
        }

        private PlayerController _player;

        private void Awake()
        {
            instance = this;
        }

        protected override void Start()
        {
            base.Start();

            _player = PlayerController.instance;

            _player.OnRamStart += ChangeVelocityToCurrentSpeed;
            _player.OnRamStop += ChangeVelocityToCurrentSpeed;
            _player.OnDeath += StopObjects;
        }

        private void OnDestroy()
        {
            _player.OnRamStart -= ChangeVelocityToCurrentSpeed;
            _player.OnRamStop -= ChangeVelocityToCurrentSpeed;
            _player.OnDeath -= StopObjects;
        }

        public override void Pause()
        {
            StopObjects();
        }

        public override void UnPause()
        {
            ChangeVelocityToCurrentSpeed();
        }

        public void AddObject(MovingObject movingObject)
        {
            if (_spawnedObjects.Contains(movingObject))
                return;

            _spawnedObjects.Add(movingObject);

            //if(_player != null)
            //    movingObject.ChangeVelocity(new Vector3 (_player.CurrentSpeed, 0f));
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

        private void ChangeVelocityToCurrentSpeed()
        {
            foreach (MovingObject movingObject in _spawnedObjects)
            {
                Vector2 newObjectVelocity = new(_currentSpeed, 0f);
                movingObject.ChangeVelocity(newObjectVelocity);
            }
        }
    }
}