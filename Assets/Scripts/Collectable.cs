using UnityEngine;
using Youregone.PlayerControls;
using Youregone.UI;
using System.Collections;
using Youregone.GameSystems;
using System;

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

        private GameState _gameState;

        public bool IsRareCollectable => _rareCollectable;

        protected override void Start()
        {
            base.Start();

            _gameState = GameState.instance;
            _randomSinWaveOffset = UnityEngine.Random.Range(-1f, 1f);
            _yOrigin = transform.position.y;
            float randomAnimationDelay = UnityEngine.Random.Range(0f, 1f);
            StartCoroutine(DelayedAnimationCoroutine(randomAnimationDelay));
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
                UIManager.instance.ScoreCounter.AddPoints(_pointsBonus);
                SoundManager.instance.PlaySoundAtPosition(_pickUpAudioClip, .05f, transform.position);

                OnDestraction?.Invoke(this);
            }

            if (collision.transform.GetComponent<MovingObjectDestroyer>())
                OnDestraction?.Invoke(this);
        }

        public override void Pause()
        {
            base.Pause();

            if(_animator != null)
                _animator.speed = 0f;
        }

        public override void UnPause()
        {
            base.UnPause();

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