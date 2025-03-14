using UnityEngine;
using Youregone.PlayerControls;
using Youregone.LevelGeneration;
using System.Collections;

namespace Youregone.EnemyAI
{
    public class Enemy : MovingObject
    {
        private const string ATTACK_TRIGGER = "RAM";

        [Header("Enemy Config")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _triggerRadiusMin;
        [SerializeField] private float _triggerRadiusMax;
        [SerializeField] private float _alertSignTime;

        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _alertSign;

        [Header("Debug")]
        [SerializeField] private Vector2 _sheepVelocity;
        [SerializeField] private CircleCollider2D _triggerCollider;

        private float _baseGravityScale;
        private PlayerController _player;

        protected override void Start()
        {
            base.Start();

            _alertSign.SetActive(false);

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
                RamTowardsPlayer();
        }

        private void RamTowardsPlayer()
        {
            _sheepVelocity = new Vector2(_moveSpeed, 0f);
            _rigidBody.velocity = new Vector2(-(_player.CurrentSpeed + _sheepVelocity.x), 0f);
            _animator.SetTrigger(ATTACK_TRIGGER);

            StartCoroutine(ShowAlertSignCoroutine());
        }

        private IEnumerator ShowAlertSignCoroutine()
        {
            _alertSign.SetActive(true);

            yield return new WaitForSeconds(_alertSignTime);

            _alertSign.SetActive(false);
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