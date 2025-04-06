using System.Collections.Generic;
using UnityEngine;
using Youregone.YPlayerController;
using Youregone.GameSystems;
using Youregone.SL;

namespace Youregone.LevelGeneration
{
    public class MovingObjectHandler : PausableMonoBehaviour, IService
    {
        [CustomHeader("Debug")]
        [SerializeField] private List<MovingObject> _spawnedObjects;

        private PlayerController _player;


        protected override void Start()
        {
            base.Start();

            _player = ServiceLocator.Get<PlayerController>();
            _player.OnSpeedChanged += PlayerController_OnSpeedChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _player.OnSpeedChanged -= PlayerController_OnSpeedChanged;
        }

        private void PlayerController_OnSpeedChanged(float currentSpeed)
        {
            ChangeVelocityToCurrentSpeed(currentSpeed);
        }

        public override void Pause()
        {
            ChangeVelocityToCurrentSpeed(0f);
        }

        public override void Unpause()
        {
            ChangeVelocityToCurrentSpeed(_player.CurrentSpeed);
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

        private void ChangeVelocityToCurrentSpeed(float currentSpeed)
        {
            foreach (MovingObject movingObject in _spawnedObjects)
            {
                Vector2 newObjectVelocity = new(currentSpeed, 0f);
                movingObject.ChangeVelocity(newObjectVelocity);
            }
        }
    }
}