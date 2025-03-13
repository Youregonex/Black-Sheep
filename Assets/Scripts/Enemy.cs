using UnityEngine;
using Youregone.PlayerControls;
using Youregone.LevelGeneration;

namespace Youregone.EnemyAI
{
    public class Enemy : MovingObject
    {
        private const string ATTACK_TRIGGER = "RAM";

        [Header("Enemy Config")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _triggerRadiusMin;
        [SerializeField] private float _triggerRadiusMax;

        [Header("Components")]
        [SerializeField] private Animator _animator;

        [Header("Debug")]
        [SerializeField] private Vector2 _sheepVelocity;
        [SerializeField] private CircleCollider2D _triggerCollider;

        private float _baseGravityScale;
        private PlayerController _player;

        protected override void Start()
        {
            base.Start();

            float randomRadius = UnityEngine.Random.Range(_triggerRadiusMin, _triggerRadiusMax);
            _triggerCollider.radius = randomRadius;

            _sheepVelocity = Vector2.zero;
            _baseGravityScale = _rigidBody.gravityScale;

            _player = PlayerController.instance;
            StartMovement(_player.CurrentSpeed);
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);

            if (collision.transform.GetComponent<PlayerController>())
                Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.transform.GetComponent<PlayerController>())
            {
                _sheepVelocity = new Vector2(_moveSpeed, 0f);                
                _rigidBody.velocity = new Vector2(-(_player.CurrentSpeed + _sheepVelocity.x), 0f);
                _animator.SetTrigger(ATTACK_TRIGGER);
            }
        }

        public override void Pause()
        {
            base.Pause();

            if (_rigidBody != null)
                _rigidBody.gravityScale = 0f;

            if (_animator != null)
                _animator.speed = 0f;
        }

        public override void UnPause()
        {
            base.UnPause();

            if(_rigidBody != null)
                _rigidBody.gravityScale = _baseGravityScale;

            if (_animator != null)
                _animator.speed = 1f;
        }

        public override void ChangeVelocity(Vector2 newVelocity)
        {
            _rigidBody.velocity = -(newVelocity + _sheepVelocity);
        }
    }
}