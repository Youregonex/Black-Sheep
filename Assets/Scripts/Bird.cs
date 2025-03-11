using UnityEngine;
using Youregone.PlayerControls;
using System.Collections;

namespace Youregone.LevelGeneration
{
    public class Bird : MovingObject
    {
        [Header("Bird Config")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Vector2 _birdFlyVelocity;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            MovingObjectHandler.instance.AddObject(this);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerController>())
                FlyAway();
        }

        private void FlyAway()
        {
            MovingObjectHandler.instance.RemoveObject(this);

            transform.parent = null;
            _animator.SetTrigger("FLY");

            Vector2 birdVelocity = new(UnityEngine.Random.Range(-_birdFlyVelocity.x, _birdFlyVelocity.x), _birdFlyVelocity.y);

            if (birdVelocity.x < 0)
                birdVelocity.x -= PlayerController.instance.CurrentSpeed;
            else
                _spriteRenderer.flipX = false;

            _rigidBody2D.velocity = birdVelocity;

            StartCoroutine(DelayedDestroy());
        }

        private IEnumerator DelayedDestroy()
        {
            yield return new WaitForSeconds(5f);

            Destroy(gameObject);
        }
    }
}
