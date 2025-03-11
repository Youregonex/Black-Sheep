using UnityEngine;
using System;
using Youregone.LevelGeneration;
using System.Collections;
using UnityEngine.SceneManagement;
using Youregone.EnemyAI;

namespace Youregone.PlayerControls
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;

        public event Action OnRamStart;
        public event Action OnRamStop;
        public event Action OnDeath;
        public event Action OnDamageTaken;

        private const string ANIMATION_JUMP_TRIGGER = "JUMP";
        private const string ANIMATION_LAND_TRIGGER = "LAND";
        private const string ANIMATION_STARTRAM_TRIGGER = "START_RAM";
        private const string ANIMATION_STOPRAM_TRIGGER = "STOP_RAM";
        private const string ANIMATION_DEATH_TRIGGER = "DEATH";

        [Header("Sheep Config")]
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _ramTimeMax;
        [SerializeField] private float _baseMoveSpeed;
        [SerializeField] private float _ramMoveSpeed;
        [SerializeField] private int _maxHealth;
        [SerializeField] private float _jumpCooldown = .25f;

        [Header("Sprite Flash Config")]
        [SerializeField] private Material _flashMaterial;
        [SerializeField] private float _flashDuration;

        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private GroundCheck _groundCheck;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Test")]
        [SerializeField] private float _currentSpeed;
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private bool _isGrounded = true;
        [SerializeField] private bool _isRaming = false;
        [SerializeField] private int _currentHealth;
        [SerializeField] private float _jumpCooldownCurrent = .25f;

        private Coroutine _flashCoroutine;

        public bool IsGrounded => _isGrounded;
        public bool IsRaming => _isRaming;
        public float CurrentSpeed => _currentSpeed;
        public int CurrentHealth => _currentHealth;

        private Rigidbody2D rb;

        private void Awake()
        {
            instance = this;

            _baseMaterial = _spriteRenderer.material;
            rb = GetComponent<Rigidbody2D>();
            _groundCheck.Landed += Land;
            _currentHealth = _maxHealth;
            _currentSpeed = 0f;
        }

        private void Update()
        {                
            if (Input.GetKeyDown(KeyCode.Space) && !_isRaming && _currentHealth > 0)
                Jump();

            if ((Input.GetKeyDown(KeyCode.F) || Input.GetKey(KeyCode.F)) && _currentHealth > 0 && _isGrounded && !_isRaming)
                StartRam();

            if (Input.GetKeyUp(KeyCode.F) && _isRaming)
                StopRam();

            if (_jumpCooldownCurrent > 0)
                _jumpCooldownCurrent -= Time.deltaTime;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((collision.transform.GetComponent<Obstacle>() && !_isRaming) || collision.transform.GetComponent<Enemy>())
            {
                TakeDamage();
                return;
            }
        }

        private void Flash()
        {
            if (_flashCoroutine != null)
                return;

            _flashCoroutine = StartCoroutine(SpriteFlashCoroutine());
        }

        private IEnumerator SpriteFlashCoroutine()
        {
            _spriteRenderer.material = _flashMaterial;

            yield return new WaitForSeconds(_flashDuration);

            _spriteRenderer.material = _baseMaterial;
            _flashCoroutine = null;
        }

        private void OnDestroy()
        {
            _groundCheck.Landed -= Land;
        }

        private void TakeDamage()
        {
            _currentHealth--;
            OnDamageTaken?.Invoke();
            Flash();

            if (_currentHealth == 0)
            {
                Debug.Log("Death");
                OnDeath?.Invoke();
                _animator.SetTrigger(ANIMATION_DEATH_TRIGGER);
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                return;
            }
        }

        private void StartRam()
        {
            _isRaming = true;
            _currentSpeed = _ramMoveSpeed;
            OnRamStart?.Invoke();
            _animator.SetTrigger(ANIMATION_STARTRAM_TRIGGER);
        }

        private void StopRam()
        {
            _isRaming = false;
            _currentSpeed = _baseMoveSpeed;
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
            if (!_isGrounded || _jumpCooldownCurrent > 0)
                return;

            _isGrounded = false;
            _jumpCooldownCurrent = _jumpCooldown;
            rb.velocity = new Vector2(_currentSpeed, 0f);
            rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _animator.SetTrigger(ANIMATION_JUMP_TRIGGER);
        }
    }
}