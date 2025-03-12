using UnityEngine;
using System;
using Youregone.LevelGeneration;
using System.Collections;
using UnityEngine.SceneManagement;
using Youregone.EnemyAI;
using Youregone.State;
using UnityEngine.UI; 

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
        [SerializeField] private float _baseMoveSpeed;
        [SerializeField] private float _ramMoveSpeed;
        [SerializeField] private float _staminaMax;
        [SerializeField] private float _staminaDrain;
        [SerializeField] private float _staminaRechargeRate;
        [SerializeField] private float _staminaRechargeDelay;
        [SerializeField] private int _maxHealth;
        [SerializeField] private Image _staminaBar;

        [Header("Sprite Flash Config")]
        [SerializeField] private Material _flashMaterial;
        [SerializeField] private float _flashDuration;

        [Header("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private GroundCheck _groundCheck;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Test")]
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _staminaCurrent;
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private bool _isGrounded = true;
        [SerializeField] private bool _isRaming = false;
        [SerializeField] private int _currentHealth;
        [SerializeField] private bool _canRechargeStamina;

        private Coroutine _flashCoroutine;
        private Coroutine _staminaCoroutine;
        private bool _runStarted = false;
        private Rigidbody2D rb;

        public bool IsGrounded => _isGrounded;
        public bool IsRaming => _isRaming;
        public float CurrentSpeed => _currentSpeed;
        public int CurrentHealth => _currentHealth;


        private void Awake()
        {
            instance = this;

            _baseMaterial = _spriteRenderer.material;
            rb = GetComponent<Rigidbody2D>();
            _groundCheck.Landed += Land;
            _currentHealth = _maxHealth;

            _staminaCurrent = _staminaMax;
            _currentSpeed = 0f;
        }

        private void Update()
        {
            if (GameState.instance.CurrentGameState != EGameState.Gameplay)
                return;

            UpdateStaminaBar();

            if (Input.GetKeyDown(KeyCode.Space) && !_isRaming && _currentHealth > 0 && _runStarted)
                Jump();

            if (_staminaCurrent <= 0 && _isRaming)
                StopRam();

            if (Input.GetKeyUp(KeyCode.F) && _isRaming)
                StopRam();

            if ((Input.GetKeyDown(KeyCode.F) || Input.GetKey(KeyCode.F)) && _currentHealth > 0 && _isGrounded && !_isRaming && _staminaCurrent > 0)
                StartRam();

            if (_isRaming)
                _staminaCurrent -= _staminaDrain * Time.deltaTime;
            else if(_canRechargeStamina)
                _staminaCurrent += _staminaRechargeRate * Time.deltaTime;
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

        private void UpdateStaminaBar()
        {
            _staminaBar.fillAmount = _staminaCurrent / _staminaMax;
        }

        private void StartRam()
        {
            if (!_runStarted)
                _runStarted = true;

            _staminaCurrent -= _staminaCurrent * .05f;
            _isRaming = true;
            _canRechargeStamina = false;
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

            if (_staminaCoroutine != null)
                StopCoroutine(_staminaCoroutine);

            _staminaCoroutine = StartCoroutine(StaminaRechargeDelayCoroutine());
        }

        private IEnumerator StaminaRechargeDelayCoroutine()
        {
            yield return new WaitForSeconds(_staminaRechargeDelay);

            _canRechargeStamina = true;
            _staminaCoroutine = null;
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

            _isGrounded = false;
            rb.velocity = new Vector2(_currentSpeed, 0f);
            rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _animator.SetTrigger(ANIMATION_JUMP_TRIGGER);
        }
    }
}