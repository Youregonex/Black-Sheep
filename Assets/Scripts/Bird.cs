using UnityEngine;
using Youregone.PlayerControls;
using System.Collections;

namespace Youregone.LevelGeneration
{
    public class Bird : MovingObject
    {
        [Header("Bird Config")]
        [SerializeField] private Animator _animator;
        [SerializeField] private float _birdFlyVelocityX;
        [SerializeField] private float _birdFlyVelocityY;

        private void Start()
        {
            MovingObjectHandler.instance.AddObject(this);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerController>())
                Fly();
        }

        private void Fly()
        {
            transform.parent = null;
            _animator.SetTrigger("FLY");
            _rigidBody2D.velocity += new Vector2(-PlayerController.instance.CurrentSpeed + _birdFlyVelocityX, _birdFlyVelocityY);
            MovingObjectHandler.instance.RemoveObject(this);
            StartCoroutine(DelayedDestroy());
        }

        private IEnumerator DelayedDestroy()
        {
            yield return new WaitForSeconds(5f);

            Destroy(gameObject);
        }
    }
}
