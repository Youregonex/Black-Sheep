using UnityEngine;
using Youregone.YPlayerController;
using System.Collections;
using Youregone.GameSystems;
using System;
using Youregone.SL;
using DG.Tweening;

namespace Youregone.LevelGeneration
{
    public class Collectable : MovingObject
    {
        public event Action<Collectable> OnDestraction;

        private const string START_ANIMATION = "STARTANIMATION";

        [CustomHeader("Collectable Config")]
        [SerializeField] private bool _isRareCollectable;
        [SerializeField] private int _pointsBonus;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Animator _animator;
        [SerializeField] private AudioClip _pickUpAudioClip;
        [SerializeField] private GameObject _lightGameObject;
        [SerializeField] private Collider2D _triggerZone;

        [CustomHeader("Sin Wave Movement Config")]
        [SerializeField] private float _amplitude;
        [SerializeField] private float _frequency;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _yMoveAmount;

        [CustomHeader("Debug")]
        [SerializeField] private float _yOrigin;
        [SerializeField] private float _randomSinWaveOffset;
        [SerializeField] private float _timeUnpaused;

        private GameState _gameState;
        private SoundManager _soundManager;
        private Coroutine _currentCoroutine;

        private bool _isTriggered;
        private float _randomAnimationDelay;

        public bool IsRareCollectable => _isRareCollectable;

        protected override void Awake()
        {
            base.Awake();

            UpdateYOrigin();
            _randomSinWaveOffset = UnityEngine.Random.Range(-1f, 1f);
            _randomAnimationDelay = UnityEngine.Random.Range(0f, 1f);
        }

        private void OnEnable()
        {
            if (_soundManager == null)
                _soundManager = ServiceLocator.Get<SoundManager>();

            _isTriggered = false;
            _triggerZone.enabled = true;
            _lightGameObject.SetActive(true);
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }

            _currentCoroutine = StartCoroutine(DelayedAnimationCoroutine(_randomAnimationDelay));
        }

        protected override void Start()
        {
            base.Start();

            _gameState = ServiceLocator.Get<GameState>();
        }

        private void FixedUpdate()
        {
            if (_gameState.CurrentGameState != EGameState.Gameplay)
                return;

            SinMovement();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<PlayerController>() && !_isTriggered)
            {
                _lightGameObject.SetActive(false);
                _triggerZone.enabled = false;   
                _isTriggered = true;

                ServiceLocator.Get<ScoreCounter>().AddPoints(_pointsBonus);
                ServiceLocator.Get<PlayerCloversCollected>().CollectClover(_isRareCollectable);

                _soundManager.PlayCollectablePickupClip(transform.position);

                _rigidBody.velocity = Vector2.zero;

                Sequence sequence = DOTween.Sequence();
                sequence
                    .Append(transform.DOMove(Vector2.up * _yMoveAmount, _animationDuration))
                    .Join(transform.DORotate(new Vector3(0f, 0f, 360f), _animationDuration, RotateMode.FastBeyond360))
                    .Join(_spriteRenderer.DOFade(0f, _animationDuration * 1.5f).SetEase(Ease.OutQuad))
                    .OnComplete(() =>
                    {
                        OnDestraction?.Invoke(this);
                    })
                    .Play();

                return;
            }

            if (collision.transform.GetComponent<MovingObjectDestroyer>())
                OnDestraction?.Invoke(this);
        }

        private void OnDisable()
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null;
            }
        }

        public override void Pause()
        {
            base.Pause();

            if(_animator != null)
                _animator.speed = 0f;
        }

        public override void Unpause()
        {
            base.Unpause();

            if (_animator != null)
                _animator.speed = 1f;
        }

        public void UpdateYOrigin()
        {
            _yOrigin = transform.position.y;
        }

        private void SinMovement()
        {
            _timeUnpaused += Time.deltaTime;

            Vector2 position = transform.position;

            float sin = Mathf.Sin((_timeUnpaused + _randomSinWaveOffset) * _frequency) * _amplitude;
            position.y = _yOrigin + sin;

            transform.position = position;
        }

        private IEnumerator DelayedAnimationCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            _animator.SetTrigger(START_ANIMATION);
        }
    }
}