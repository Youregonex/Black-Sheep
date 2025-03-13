using UnityEngine;
using Youregone.PlayerControls;
using Youregone.UI;
using System.Collections;
using Youregone.GameSystems;

namespace Youregone.LevelGeneration
{
    public class Collectable : MovingObject
    {
        private const string START_ANIMATION = "STARTANIMATION";

        [Header("Collectable Config")]
        [SerializeField] private int _pointsBonus;
        [SerializeField] private Animator _animator;

        [Header("Sin Wave Config")]
        [SerializeField] private float _amplitude;
        [SerializeField] private float _frequency;

        [Header("Debug")]
        [SerializeField] private float _yOrigin;
        [SerializeField] private float _randomSinWaveOffset;
        [SerializeField] private float _timeUnpaused;

        private GameState _gameState;

        protected override void Start()
        {
            base.Start();

            _gameState = GameState.instance;

            _yOrigin = transform.position.y;
            _randomSinWaveOffset = UnityEngine.Random.Range(-1f, 1f);

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
                Destroy(gameObject);
            }

            if (collision.transform.GetComponent<MovingObjectDestroyer>())
                Destroy(gameObject);
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

        private IEnumerator DelayedAnimationCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            _animator.SetTrigger(START_ANIMATION);
        }
    }
}