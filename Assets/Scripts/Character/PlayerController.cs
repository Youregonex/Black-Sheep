using UnityEngine;
using System;
using Youregone.LevelGeneration;
using System.Collections;
using Youregone.EnemyAI;
using UnityEngine.UI;
using Youregone.GameSystems;
using Youregone.SL;

namespace Youregone.PlayerControls
{
    public class PlayerController : PausableMonoBehaviour, IUpdateObserver, IService
    {
        public event Action OnRamStart;
        public event Action OnRamStop;
        public event Action OnDeath;
        public event Action OnDamageTaken;

        private const string ANIMATION_JUMP_TRIGGER = "JUMP";
        private const string ANIMATION_LAND_TRIGGER = "LAND";
        private const string ANIMATION_STARTRAM_TRIGGER = "START_RAM";
        private const string ANIMATION_STOPRAM_TRIGGER = "STOP_RAM";
        private const string ANIMATION_DEATH_TRIGGER = "DEATH";
        private const string ANIMATION_STAMINA_BAR_FULL_CHARGE_TRIGGER = "BARFULL";

        [CustomHeader("Sheep Config")]
        [SerializeField] private float _baseMoveSpeed;
        [SerializeField] private float _ramMoveSpeed;
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _staminaMax;
        [SerializeField] private float _staminaDrain;
        [SerializeField, Range(0f, 1f)] private float _minStaminaDrainPerUse;
        [SerializeField] private float _staminaRechargeRate;
        [SerializeField] private float _staminaRechargeDelay;
        [SerializeField] private int _maxHealth;
        [SerializeField] private Image _staminaBar;
        [SerializeField] private bool _immortal;

        [CustomHeader("Sprite Flash Config")]
        [SerializeField] private Material _flashMaterial;
        [SerializeField] private float _flashDuration;

        [CustomHeader("Components")]
        [SerializeField] private Animator _animator;
        [SerializeField] private Animator _staminaBarAnimator;
        [SerializeField] private GroundCheck _groundCheck;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Gradient _staminaGradient;

        [CustomHeader("Debug")]
        [SerializeField] private float _currentSpeed;
        [SerializeField] private float _staminaCurrent;
        [SerializeField] private Material _baseMaterial;
        [SerializeField] private bool _isGrounded = true;
        [SerializeField] private bool _isRaming = false;
        [SerializeField] private bool _canRechargeStamina = true;
        [SerializeField] private int _currentHealth;

        private PlayerCharacterInput _playerInput;
        private float _baseGravityScale;
        private Vector2 _prePauseVelocity;
        private Coroutine _flashCoroutine;
        private Coroutine _staminaCoroutine;
        private bool _runStarted = false;
        private Rigidbody2D _rigidBody;
        private GameState _gameState;

        public bool IsGrounded => _isGrounded;
        public bool IsRaming => _isRaming;
        public float CurrentSpeed => _currentSpeed;
        public int CurrentHealth => _currentHealth;
        public PlayerCharacterInput PlayerCharacterInput => _playerInput;

        private void Awake()
        {
            _playerInput = new();
            _playerInput.OnRamButtonReleased += StopRam;

            _baseMaterial = _spriteRenderer.material;
            _rigidBody = GetComponent<Rigidbody2D>();
            _groundCheck.Landed += Land;
            _currentHealth = _maxHealth;

            _baseGravityScale = _rigidBody.gravityScale;
            _staminaCurrent = _staminaMax;
            _currentSpeed = 0f;
        }

        private void OnEnable()
        {
            UpdateManager.RegisterUpdateObserver(this);
        }

        protected override void Start()
        {
            base.Start();

            _gameState = ServiceLocator.Get<GameState>();
        }

        public void ObservedUpdate()
        {
            if (_gameState.CurrentGameState != EGameState.Gameplay)
                return;

            UpdateStaminaBar();

            if (_playerInput.JumpPressed)
                Jump();

            if (_playerInput.RamPressed)
                StartRam();

            if (_staminaCurrent <= 0 && _isRaming)
                StopRam();

            ManageStamina();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if ((collision.transform.GetComponent<Obstacle>() && !_isRaming) || collision.transform.GetComponent<Enemy>())
            {
                TakeDamage();
                return;
            }

            if (collision.transform.GetComponent<FallZone>())
                Fall();
        }

        private void OnDisable()
        {
            UpdateManager.UnregisterUpdateObserver(this);
        }

        private void OnDestroy()
        {
            _groundCheck.Landed -= Land;
            _playerInput.OnRamButtonReleased -= StopRam;
        }

        public override void Pause()
        {
            _animator.speed = 0f;

            _prePauseVelocity = _rigidBody.velocity;
            _rigidBody.velocity = Vector2.zero;
            _rigidBody.gravityScale = 0f;

            _staminaBar.GetComponent<Animator>().speed = 0f;
        }

        public override void Unpause()
        {
            _animator.speed = 1f;

            _rigidBody.velocity = _prePauseVelocity;
            _rigidBody.gravityScale = _baseGravityScale;

            _staminaBar.GetComponent<Animator>().speed = 1f;
        }

        private void ManageStamina()
        {
            if (_isRaming)
                _staminaCurrent -= _staminaDrain * Time.deltaTime;
            else if (_canRechargeStamina && _staminaCurrent < _staminaMax)
            {
                _staminaCurrent += _staminaRechargeRate * Time.deltaTime;

                if (_staminaCurrent >= _staminaMax)
                {
                    _staminaCurrent = _staminaMax;
                    _staminaBarAnimator.SetTrigger(ANIMATION_STAMINA_BAR_FULL_CHARGE_TRIGGER);
                }
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

        private void TakeDamage()
        {
            Flash();

            if (_immortal)
                return;

            _currentHealth--;
            OnDamageTaken?.Invoke();

            if (_currentHealth == 0)
                CharacterDie();
        }

        private void Fall()
        {
            CharacterDie();
        }

        private void CharacterDie()
        {
            Debug.Log("Death");
            _animator.SetTrigger(ANIMATION_DEATH_TRIGGER);
            _currentSpeed = 0f;
            OnDeath?.Invoke();
        }

        private void UpdateStaminaBar()
        {
            float fillAmount = _staminaCurrent / _staminaMax;
            _staminaBar.fillAmount = fillAmount;
            _staminaBar.color = _staminaGradient.Evaluate(fillAmount);
        }

        private void StartRam()
        {
            if (!_runStarted)
                _runStarted = true;

            if (_currentHealth <= 0 || !_isGrounded || _isRaming || _staminaCurrent <= 0)
                return;

            _staminaCurrent -= _staminaCurrent * _minStaminaDrainPerUse;
            _isRaming = true;
            _canRechargeStamina = false;
            _currentSpeed = _ramMoveSpeed;
            OnRamStart?.Invoke();
            _animator.SetTrigger(ANIMATION_STARTRAM_TRIGGER);
        }

        private void StopRam()
        {
            if (!_isRaming)
                return;

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
            StartCoroutine(LandCoroutine());
        }

        private IEnumerator LandCoroutine()
        {
            _isGrounded = true;
            _animator.SetTrigger(ANIMATION_LAND_TRIGGER);

            //For some reason sometimes "LAND" trigger gets stuck and jump animation breaks
            float triggerResetDelay = .25f;
            yield return new WaitForSeconds(triggerResetDelay);
            _animator.ResetTrigger(ANIMATION_LAND_TRIGGER);
        }

        private void Jump()
        {
            if (_currentHealth <= 0 || !_runStarted || !_isGrounded)
                return;
            
            if (_isRaming)
                StopRam();

            _isGrounded = false;
            _rigidBody.velocity = new Vector2(_currentSpeed, 0f);
            _rigidBody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _animator.SetTrigger(ANIMATION_JUMP_TRIGGER);
        }
    }
}