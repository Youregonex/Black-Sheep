using UnityEngine;
using Youregone.PlayerControls;
using Youregone.UI;
using System.Collections;
using Youregone.GameSystems;
using System;
using Youregone.SL;

namespace Youregone.LevelGeneration
{
    public class Collectable : MovingObject
    {
        public event Action<Collectable> OnDestraction;

        private const string START_ANIMATION = "STARTANIMATION";

        [Header("Collectable Config")]
        [SerializeField] private bool _rareCollectable;
        [SerializeField] private int _pointsBonus;
        [SerializeField] private Animator _animator;
        [SerializeField] private AudioClip _pickUpAudioClip;

        [Header("Sin Wave Config")]
        [SerializeField] private float _amplitude;
        [SerializeField] private float _frequency;

        [Header("Debug")]
        [SerializeField] private float _yOrigin;
        [SerializeField] private float _randomSinWaveOffset;
        [SerializeField] private float _timeUnpaused;

        public bool IsRareCollectable => _rareCollectable;

        private GameState _gameState;
        private Coroutine _coroutine;
        private float _randomAnimationDelay;

        protected override void Awake()
        {
            base.Awake();

            _randomSinWaveOffset = UnityEngine.Random.Range(-1f, 1f);
            UpdateYOrigin();
            _randomAnimationDelay = UnityEngine.Random.Range(0f, 1f);
        }

        private void OnEnable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
            }

            _coroutine = StartCoroutine(DelayedAnimationCoroutine(_randomAnimationDelay));
        }

        protected override void Start()
        {
            base.Start();

            _gameState = ServiceLocator.Get<GameState>();
        }

        private void FixedUpdate()
        {
            if (_gameState.CurrentGameState == EGameState.Pause)
                return;

            _timeUnpaused += Time.deltaTime;

            Vector2 position = transform.position;

            float sin = Mathf.Sin((_timeUnpaused + _randomSinWaveOffset) * _frequency) * _amplitude;
            position.y = _yOrigin + sin;

            transform.position = position;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<PlayerController>())
            {
                ServiceLocator.Get<ScoreCounter>().AddPoints(_pointsBonus);
                ServiceLocator.Get<SoundManager>().PlaySoundAtPosition(_pickUpAudioClip, .05f, transform.position);

                OnDestraction?.Invoke(this);
            }

            if (collision.transform.GetComponent<MovingObjectDestroyer>())
                OnDestraction?.Invoke(this);
        }

        private void OnDisable()
        {
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
                _coroutine = null;
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

        private IEnumerator DelayedAnimationCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            _animator.SetTrigger(START_ANIMATION);
        }
    }
}