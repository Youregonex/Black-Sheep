using UnityEngine;
using Youregone.PlayerControls;
using Youregone.LevelGeneration;
using DG.Tweening;

namespace Youregone.EnemyAI
{
    public class Enemy : MovingObject
    {
        private const string ATTACK_TRIGGER = "RAM";

        [Header("Enemy Config")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _triggerRadiusMin;
        [SerializeField] private float _triggerRadiusMax;
        [SerializeField] private float _alertRangeAddition;

        [Header("DOTween Config")]
        [SerializeField] private float _alertSignAnimationDuration;
        [SerializeField] private float _alertSignUpwardsMovementAmount;
        [SerializeField] private float _alertSignFadeTime;

        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _alertSign;

        [Header("Debug")]
        [SerializeField] private bool _triggered = false;
        [SerializeField] private float _alertZoneSize;
        [SerializeField] private float _triggerZoneSize;
        [SerializeField] private Vector2 _sheepVelocity;
        [SerializeField] private CircleCollider2D _triggerCollider;

        private float _baseGravityScale;
        private PlayerController _player;

        protected override void Start()
        {
            base.Start();

            _alertSign.SetActive(false);

            _triggerZoneSize = UnityEngine.Random.Range(_triggerRadiusMin, _triggerRadiusMax);
            _alertZoneSize = _triggerZoneSize + _alertRangeAddition;

            _triggerCollider.radius = _alertZoneSize;

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
                if(_triggered)
                {
                    RamTowardsPlayer();
                    return;
                }

                ShowAlertSignCoroutine();
                _triggerCollider.radius = _triggerZoneSize;
                _triggered = true;
            }
        }

        private void RamTowardsPlayer()
        {
            _sheepVelocity = new Vector2(_moveSpeed, 0f);
            _rigidBody.velocity = new Vector2(-(_player.CurrentSpeed + _sheepVelocity.x), 0f);
            _animator.SetTrigger(ATTACK_TRIGGER);
        }

        private void ShowAlertSignCoroutine()
        {
            _alertSign.SetActive(true);

            _alertSign.transform.DOMoveY(_alertSign.transform.position.y + _alertSignUpwardsMovementAmount, _alertSignAnimationDuration).OnComplete(() =>
            {
                _alertSign.transform.GetChild(0).GetComponent<SpriteRenderer>().DOFade(0f, _alertSignFadeTime).OnComplete(() =>
                {
                    _alertSign.SetActive(false);
                });
            });
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