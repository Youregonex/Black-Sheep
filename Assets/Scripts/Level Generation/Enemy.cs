using UnityEngine;
using Youregone.YPlayerController;
using Youregone.LevelGeneration;
using DG.Tweening;
using Youregone.SL;
using Youregone.GameSystems;

namespace Youregone.EnemyAI
{
    public class Enemy : MovingObject
    {
        private const string ATTACK_TRIGGER = "RAM";

        [CustomHeader("Enemy Config")]
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _triggerRadiusMin;
        [SerializeField] private float _triggerRadiusMax;
        [SerializeField] private float _alertRangeAddition;

        [CustomHeader("DOTween Config")]
        [SerializeField] private float _alertSignAnimationDuration;
        [SerializeField] private float _alertSignUpwardsMovementAmount;
        [SerializeField] private float _alertSignFadeTime;

        [CustomHeader("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private GameObject _alertSign;

        [CustomHeader("Debug")]
        [SerializeField] private bool _triggered = false;
        [SerializeField] private float _alertZoneSize;
        [SerializeField] private float _triggerZoneSize;
        [SerializeField] private Vector2 _sheepVelocity;
        [SerializeField] private CircleCollider2D _triggerCollider;

        private PlayerController _player;
        private Tween _currentTween;
        private Animator _alertSignAnimator;
        private SoundManager _soundManager;

        private float _baseGravityScale;

        private void OnEnable()
        {
            if (_soundManager == null)
                _soundManager = ServiceLocator.Get<SoundManager>();
        }

        protected override void Start()
        {
            base.Start();

            _alertSign.SetActive(false);
            _alertSignAnimator = _alertSign.transform.GetChild(0).GetComponent<Animator>();
            _triggerZoneSize = UnityEngine.Random.Range(_triggerRadiusMin, _triggerRadiusMax);
            _alertZoneSize = _triggerZoneSize + _alertRangeAddition;

            _triggerCollider.radius = _alertZoneSize;

            _sheepVelocity = Vector2.zero;
            _baseGravityScale = _rigidBody.gravityScale;

            _player = ServiceLocator.Get<PlayerController>();
            Vector2 enemyVelocity = new(_player.CurrentSpeed, 0f);
            ChangeVelocity(enemyVelocity);
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
                    _soundManager.PlaySheepSound(transform.position);
                    _triggered = false;
                    return;
                }

                ShowAlertSign();
                _triggerCollider.radius = _triggerZoneSize;
                _triggered = true;
            }
        }

        protected override void OnDestroy()
        {
            if (_currentTween != null)
                _currentTween.Kill();

            base.OnDestroy();
        }

        public override void Pause()
        {
            base.Pause();

            if (_rigidBody != null)
                _rigidBody.gravityScale = 0f;

            if (_animator != null)
                _animator.speed = 0f;

            if (_currentTween != null)
                _currentTween.Pause();

            if(_alertSignAnimator != null)
                _alertSignAnimator.speed = 0f;

            _rigidBody.velocity = Vector2.zero;
        }

        public override void Unpause()
        {
            base.Unpause();

            if (_rigidBody != null)
                _rigidBody.gravityScale = _baseGravityScale;

            if (_animator != null)
                _animator.speed = 1f;

            if (_currentTween != null)
                _currentTween.Play();

            if (_alertSignAnimator != null)
                _alertSignAnimator.speed = 1f;

            if(_triggered)
                _rigidBody.velocity = new Vector2(-(_player.CurrentSpeed + _sheepVelocity.x), 0f);
            else
                _rigidBody.velocity = new Vector2(-_player.CurrentSpeed, 0f);
        }

        public override void ChangeVelocity(Vector2 newVelocity)
        {
            _rigidBody.velocity = -(newVelocity + _sheepVelocity);
        }

        private void RamTowardsPlayer()
        {
            transform.SetParent(null);
            _sheepVelocity = new Vector2(_moveSpeed, 0f);
            _rigidBody.velocity = new Vector2(-(_player.CurrentSpeed + _sheepVelocity.x), 0f);
            _animator.SetTrigger(ATTACK_TRIGGER);
        }

        private void ShowAlertSign()
        {
            _alertSign.SetActive(true);

            _currentTween = _alertSign.transform
                .DOMoveY(_alertSign.transform.position.y + _alertSignUpwardsMovementAmount, _alertSignAnimationDuration)
                .OnComplete(() =>
                {
                    _currentTween = _alertSign.transform
                    .GetChild(0)
                    .GetComponent<SpriteRenderer>()
                    .DOFade(0f, _alertSignFadeTime)
                    .OnComplete(() =>
                    {
                        _alertSign.transform.GetChild(1).gameObject.SetActive(false);
                        _alertSign.SetActive(false);
                        _currentTween = null;
                    })
                    .OnUpdate(() =>
                    {
                        if (_alertSign == null)
                            _currentTween.Kill();
                    });
                })
                .OnUpdate(() =>
                {
                    if (_alertSign == null)
                        _currentTween.Kill();
                }); ;
        }
    }
}