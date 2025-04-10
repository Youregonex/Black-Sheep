using DG.Tweening;
using UnityEngine;
using Youregone.SL;
using Youregone.YPlayerController;

namespace Youregone.LevelGeneration
{
    public abstract class Buff : MovingObject
    {
        [CustomHeader("Buff Settings")]
        [SerializeField] protected Vector2 _direction;
        [SerializeField] protected float _forceMultiplier;
        [SerializeField] protected Collider2D _collider;

        [CustomHeader("DOTween Config")]
        [SerializeField] protected float _slowdownDuration;

        private Vector2 _prePauseVelocity;
        private float _prePauseGravityScale;
        private float _prePauseAngularVelocity;
        private Tween _slowDownTween;

        private void OnEnable()
        {
            ApplyForce();

            Vector2 newVelocity = new(ServiceLocator.Get<PlayerController>().CurrentSpeed, _rigidBody.velocity.y);
            ChangeVelocity(newVelocity);
        }

        protected abstract void Apply(PlayerController player);

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);

            if(collision.transform.TryGetComponent(out PlayerController player))
            {
                _collider.enabled = false;
                Apply(player);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if(_slowDownTween != null)
                _slowDownTween.Complete();
        }

        public override void Pause()
        {
            _prePauseVelocity = _rigidBody.velocity;
            _prePauseGravityScale = _rigidBody.gravityScale;
            _prePauseAngularVelocity = _rigidBody.angularVelocity;

            if (_slowDownTween != null)
                _slowDownTween.Pause();

            _rigidBody.velocity = Vector2.zero;
            _rigidBody.gravityScale = 0f;
            _rigidBody.angularVelocity = 0f;
        }

        public override void Unpause()
        {
            if (_slowDownTween != null)
                _slowDownTween.Play();

            _rigidBody.velocity = _prePauseVelocity;
            _rigidBody.gravityScale = _prePauseGravityScale;
            _rigidBody.angularVelocity = _prePauseAngularVelocity;
        }

        public override void ChangeVelocity(Vector2 newVelocity)
        {
            if (_slowDownTween != null)
            {
                _slowDownTween.Kill();
                _slowDownTween = null;
            }

            _slowDownTween = DOTween.To(
                () => _rigidBody.velocity.x,
                x => _rigidBody.velocity = new Vector2(x, _rigidBody.velocity.y),
                -newVelocity.x,
                _slowdownDuration
            ).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                _slowDownTween = null;
            });
        }

        private void ApplyForce()
        {
            float randomXForceModifier;
            float randomYForceModifier;

            if (ServiceLocator.Get<PlayerController>().IsRaming)
            {
                randomXForceModifier = UnityEngine.Random.Range(1.25f, 2f);
                randomYForceModifier = UnityEngine.Random.Range(1.25f, 2f);
            }
            else
            {
                randomXForceModifier = UnityEngine.Random.Range(1f, 1.75f);
                randomYForceModifier = UnityEngine.Random.Range(1f, 1.75f);
            }

            _rigidBody.AddForce(new Vector2(_direction.x * _forceMultiplier * randomXForceModifier, _direction.y * _forceMultiplier * randomYForceModifier), ForceMode2D.Impulse);
        }
    }
}