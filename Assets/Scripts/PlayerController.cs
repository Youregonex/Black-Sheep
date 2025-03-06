using UnityEngine;
using System;
using Youregone.LevelGeneration;

namespace Youregone.PlayerControls
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;

        public Action OnRamStart;
        public Action OnRamStop;
        public Action OnDeath;

        private const string ANIMATION_JUMP_TRIGGER = "JUMP";
        private const string ANIMATION_LAND_TRIGGER = "LAND";
        private const string ANIMATION_STARTRAM_TRIGGER = "START_RAM";
        private const string ANIMATION_STOPRAM_TRIGGER = "STOP_RAM";

        [Header("Sheep Config")]
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _ramTimeMax;
        [SerializeField] private float _ramMoveSpeed;
        [SerializeField] private float _baseMoveSpeed;
        [SerializeField] private int _maxHealth;

        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private GroundCheck _groundCheck;

        [Header("Test")]
        [SerializeField] private float _ramTimeCurrent;
        [SerializeField] private bool _isGrounded = true;
        [SerializeField] private bool _isRaming = false;
        [SerializeField] private int _currentHealth;

        public bool IsGrounded => _isGrounded;
        public bool IsRaming => _isRaming;
        public float BaseMoveSpeed => _baseMoveSpeed;
        public float RamMoveSpeed => _ramMoveSpeed;

        private Rigidbody2D rb;

        private void Awake()
        {
            instance = this;

            rb = GetComponent<Rigidbody2D>();
            _groundCheck.Landed += Land;
            _currentHealth = _maxHealth;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !_isRaming)
                Jump();

            if (Input.GetKeyDown(KeyCode.F) && _isGrounded)
                StartRam();

            if (_isRaming)
            {
                if (_ramTimeCurrent > 0)
                    _ramTimeCurrent -= Time.deltaTime;
                else
                    StopRam();
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.GetComponent<Obstacle>() && !_isRaming)
                TakeDamage();
        }

        private void OnDestroy()
        {
            _groundCheck.Landed -= Land;
        }

        private void TakeDamage()
        {
            if (_currentHealth == 1)
            {
                Debug.Log("Death");
                OnDeath?.Invoke();
                return;
            }

            _currentHealth--;
        }

        private void StartRam()
        {
            _isRaming = true;
            _ramTimeCurrent = _ramTimeMax;
            OnRamStart?.Invoke();
            _animator.SetTrigger(ANIMATION_STARTRAM_TRIGGER);
        }

        private void StopRam()
        {
            _isRaming = false;
            OnRamStop?.Invoke();
            _animator.SetTrigger(ANIMATION_STOPRAM_TRIGGER);
        }

        private void Land()
        {
            _isGrounded = true;
            _animator.SetTrigger(ANIMATION_LAND_TRIGGER);
        }

        private void Jump()
        {
            if (!_isGrounded)
                return;

            rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _animator.SetTrigger(ANIMATION_JUMP_TRIGGER);
            _isGrounded = false;
        }
    }
}