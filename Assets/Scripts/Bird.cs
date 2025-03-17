using UnityEngine;
using Youregone.PlayerControls;
using Youregone.GameSystems;
using Youregone.SL;
using System;
using System.Collections;

namespace Youregone.LevelGeneration
{
    public class Bird : MovingObject
    {
        public event Action<Bird> OnDestruction;

        [Header("Bird Config")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Vector2 _birdFlyVelocity;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _destructionDelay;

        private Vector2 _birdVelocity;

        private void OnEnable()
        {
            _birdVelocity = Vector2.zero;
        }

        protected override void Start()
        {
            base.Start();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerController>())
                FlyAway();
        }

        public override void Pause()
        {
            base.Pause();

            if (_animator != null)
                _animator.speed = 0f;

            if (_rigidBody != null)
                _rigidBody.velocity = Vector2.zero;
        }

        public override void UnPause()
        {
            base.UnPause();

            if (_animator != null)
                _animator.speed = 1f;

            if(_rigidBody != null)
                _rigidBody.velocity = _birdFlyVelocity;
        }

        private void FlyAway()
        {
            ServiceLocator.Get<MovingObjectHandler>().RemoveObject(this);
            _animator.SetTrigger("FLY");

            _birdVelocity = new(UnityEngine.Random.Range(-_birdFlyVelocity.x, _birdFlyVelocity.x), _birdFlyVelocity.y);

            if (_birdVelocity.x < 0)
            {
                float velocityModifier = 1.5f;
                _birdVelocity.x -= ServiceLocator.Get<PlayerController>().CurrentSpeed * velocityModifier;
            }
            else
                _spriteRenderer.flipX = true;

            _rigidBody.velocity = _birdVelocity;
            StartCoroutine(DelayedDestructionCoroutine());
        }

        private IEnumerator DelayedDestructionCoroutine()
        {
            yield return new WaitForSeconds(_destructionDelay);
            OnDestruction?.Invoke(this);
        }
    }
}
