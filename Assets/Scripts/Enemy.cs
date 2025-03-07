using UnityEngine;
using Youregone.PlayerControls;
using Youregone.LevelGeneration;

namespace Youregone.EnemyAI
{
    public class Enemy : MovingObject
    {
        private const string ATTACK_TRIGGER = "RAM";

        [Header("Enemy Config")]
        [SerializeField] private float _moveSpeed;

        [Header("Components")]
        [SerializeField] private Animator _animator;

        private void Start()
        {
            MovingObjectHandler.instance.AddObject(this);

            float platformMoveSpeed = PlayerController.instance.IsRaming ? PlayerController.instance.RamMoveSpeed : PlayerController.instance.BaseMoveSpeed;

            StartMovement(platformMoveSpeed);
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
                float platformMoveSpeed = PlayerController.instance.IsRaming ? PlayerController.instance.RamMoveSpeed : PlayerController.instance.BaseMoveSpeed;

                _movementLocked = true;
                _rigidBody2D.velocity = new Vector2(-(platformMoveSpeed + _moveSpeed), 0f);
                _animator.SetTrigger(ATTACK_TRIGGER);
            }
        }
    }
}