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

        [Header("Components")]
        [SerializeField] private Animator _animator;

        [Header("Test")]
        [SerializeField] private Vector2 _sheepVelocity;
        [SerializeField] private float _triggerRadiusMin;
        [SerializeField] private float _triggerRadiusMax;
        [SerializeField] private CircleCollider2D _triggerCollider;

        private void Start()
        {
            MovingObjectHandler.instance.AddObject(this);

            float randomRadius = UnityEngine.Random.Range(_triggerRadiusMin, _triggerRadiusMax);
            _triggerCollider.radius = randomRadius;

            _sheepVelocity = Vector2.zero;
            StartMovement(PlayerController.instance.CurrentSpeed);
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
                _rigidBody2D.velocity = new Vector2(-(PlayerController.instance.CurrentSpeed + _sheepVelocity.x), 0f);
                _animator.SetTrigger(ATTACK_TRIGGER);
            }
        }

        public override void ChangeVelocity(Vector2 newVelocity)
        {
            _rigidBody2D.velocity = -(newVelocity + _sheepVelocity);
        }
    }
}