using UnityEngine;
using Youregone.PlayerControls;
using Youregone.GameSystems;

namespace Youregone.LevelGeneration
{
    public class Bird : MovingObject
    {
        [Header("Bird Config")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Vector2 _birdFlyVelocity;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _destructionDelay;

        private bool _isFlying;
        private Vector2 _birdVelocity;
        private GameState _gameState;

        protected override void Start()
        {
            base.Start();

            _gameState = GameState.instance;
        }

        private void Update()
        {
            if (_isFlying && !(_gameState.CurrentGameState == EGameState.Pause))
                _destructionDelay -= Time.deltaTime;

            if (_destructionDelay <= 0)
                Destroy(gameObject);
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
            _isFlying = true;

            MovingObjectHandler.instance.RemoveObject(this);

            transform.parent = null;
            _animator.SetTrigger("FLY");

            _birdVelocity = new(UnityEngine.Random.Range(-_birdFlyVelocity.x, _birdFlyVelocity.x), _birdFlyVelocity.y);

            if (_birdVelocity.x < 0)
            {
                float velocityModifier = 1.5f;
                _birdVelocity.x -= PlayerController.instance.CurrentSpeed * velocityModifier;
            }
            else
                _spriteRenderer.flipX = true;

            _rigidBody.velocity = _birdVelocity;
        }
    }
}
