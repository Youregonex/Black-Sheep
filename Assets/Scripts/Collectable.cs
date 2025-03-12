using UnityEngine;
using Youregone.PlayerControls;
using Youregone.UI;
using System.Collections;

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

        [Header("Test")]
        [SerializeField] private float _yOrigin;
        [SerializeField] private float _randomSinWaveOffset;

        protected override void Start()
        {
            base.Start();

            _yOrigin = transform.position.y;
            _randomSinWaveOffset = UnityEngine.Random.Range(-1f, 1f);

            float randomAnimationDelay = UnityEngine.Random.Range(0f, 1f);
            StartCoroutine(DelayedAnimation(randomAnimationDelay));
        }

        private void FixedUpdate()
        {
            Vector2 position = transform.position;

            float sin = Mathf.Sin((Time.time + _randomSinWaveOffset) * _frequency) * _amplitude;
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

        private IEnumerator DelayedAnimation(float delay)
        {
            yield return new WaitForSeconds(delay);

            _animator.SetTrigger(START_ANIMATION);
        }
    }
}