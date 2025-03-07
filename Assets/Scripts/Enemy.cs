using UnityEngine;
using Youregone.PlayerControls;

namespace Youregone.EnemyAI
{
    public class Enemy : MonoBehaviour
    {
        private const string ATTACK_TRIGGER = "RAM";

        [Header("Enemy Config")]
        [SerializeField] private float _moveSpeed;

        [Header("Components")]
        [SerializeField] private Animator _animator;

        [Header("Test")]
        [SerializeField] private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.transform.GetComponent<PlayerController>())
            {
                _rb.velocity = new Vector2(-_moveSpeed, 0f);
                _animator.SetTrigger(ATTACK_TRIGGER);
            }
        }
    }
}