using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;
using Youregone.SL;
using Youregone.YPlayerController;
using Youregone.GameSystems;

namespace Youregone.LevelGeneration
{
    public class RockBreakPiece : MovingObject
    {
        public event Action<RockBreakPiece> OnDestruction;

        [CustomHeader("Rock Break Piece Config")]
        [SerializeField] private Vector2 _direction;
        [SerializeField] private float _forceMultiplier;
        [SerializeField] private List<Sprite> _rockPiecesSpriteList;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private WaterSplash _waterSplashSmallPrefab;

        [CustomHeader("DOTween Config")]
        [SerializeField] private float _fadeDuration;
        [SerializeField] private float _fadeDelay;
        [SerializeField] private float _slowdownDuration;

        private SoundManager _soundManager;

        private Vector2 _prePauseVelocity;
        private float _prePauseGravityScale;
        private float _prePauseAngularVelocity;
        private Tween _fadeTween;
        private Tween _slowDownTween;

        private void OnEnable()
        {
            ServiceLocator.Get<MovingObjectHandler>().AddObject(this);

            if (_soundManager == null)
                _soundManager = ServiceLocator.Get<SoundManager>();

            PickRandomSprite();
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            ApplyForce();
            Vector2 newVelocity = new(ServiceLocator.Get<PlayerController>().CurrentSpeed, _rigidBody.velocity.y);
            ChangeVelocity(newVelocity);

            _fadeTween = _spriteRenderer
                .DOFade(0f, _fadeDuration)
                .SetDelay(_fadeDelay)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    OnDestruction?.Invoke(this);
                    _fadeTween = null;
                });
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);

            if (collision.transform.GetComponent<FallZone>())
            {
                _spriteRenderer.color = new Color(1f, 1f, 1f, 0f);

                KillActiveTweens();

                _soundManager.PlayWaterSplashClip(transform.position);
                WaterSplash waterSplash = Instantiate(_waterSplashSmallPrefab, transform.position, Quaternion.identity);
                waterSplash.Initialize(false);
                OnDestruction?.Invoke(this);
            }
        }

        private void OnDisable()
        {
            ServiceLocator.Get<MovingObjectHandler>().AddObject(this);

            KillActiveTweens();
        }

        protected override void OnDestroy()
        {
            KillActiveTweens();
        }

        public override void Pause()
        {
            _prePauseVelocity = _rigidBody.velocity;
            _prePauseGravityScale = _rigidBody.gravityScale;
            _prePauseAngularVelocity = _rigidBody.angularVelocity;

            if (_slowDownTween != null)
                _slowDownTween.Pause();

            if (_fadeTween != null)
                _fadeTween.Pause();

            _rigidBody.velocity = Vector2.zero;
            _rigidBody.gravityScale = 0f;
            _rigidBody.angularVelocity = 0f;
        }

        public override void Unpause()
        {
            if (_slowDownTween != null)
                _slowDownTween.Play();

            if (_fadeTween != null)
                _fadeTween.Play();

            _rigidBody.velocity = _prePauseVelocity;
            _rigidBody.gravityScale = _prePauseGravityScale;
            _rigidBody.angularVelocity = _prePauseAngularVelocity;
        }

        private void KillActiveTweens()
        {
            if (_fadeTween != null)
            {
                _fadeTween.Kill();
                _fadeTween = null;
            }

            if (_slowDownTween != null)
            {
                _slowDownTween.Kill();
                _slowDownTween = null;
            }
        }

        public override void ChangeVelocity(Vector2 newVelocity)
        {
            if (_slowDownTween != null)
            {
                _slowDownTween.Kill();
                _slowDownTween = null;
            }

            float currentVelocityX = 0f;

            if (_rigidBody != null)
                currentVelocityX = _rigidBody.velocity.x;
            else
                return;

            _slowDownTween = DOTween
                .To(
                    () => currentVelocityX,
                    x => currentVelocityX = x,
                    -newVelocity.x,
                    _slowdownDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    _slowDownTween = null;
                })
                .OnUpdate(() =>
                {
                    if (_rigidBody != null)
                        _rigidBody.velocity = new Vector2(currentVelocityX, _rigidBody.velocity.y);
                });
        }

        private void ApplyForce()
        {
            float randomXForceModifier;
            float randomYForceModifier;

            float randomForceMidifierRamSpeedMin = 1.25f;
            float randomForceMidifierRamSpeedMax = 2f;

            float randomForceMidifierNormalSpeedMin = 1f;
            float randomForceeModifierNormalSpeedMax = 1.75f;

            if (ServiceLocator.Get<PlayerController>().IsRaming)
            {
                randomXForceModifier = UnityEngine.Random.Range(randomForceMidifierRamSpeedMin, randomForceMidifierRamSpeedMax);
                randomYForceModifier = UnityEngine.Random.Range(randomForceMidifierRamSpeedMin, randomForceMidifierRamSpeedMax);
            }
            else
            {
                randomXForceModifier = UnityEngine.Random.Range(randomForceMidifierNormalSpeedMin, randomForceeModifierNormalSpeedMax);
                randomYForceModifier = UnityEngine.Random.Range(randomForceMidifierNormalSpeedMin, randomForceeModifierNormalSpeedMax);
            }

            _rigidBody.AddForce(new Vector2(
                _direction.x * _forceMultiplier * randomXForceModifier,
                _direction.y * _forceMultiplier * randomYForceModifier),
                ForceMode2D.Impulse);
        }

        private void PickRandomSprite()
        {
            int randomSpriteIndex = UnityEngine.Random.Range(0, _rockPiecesSpriteList.Count);
            _spriteRenderer.sprite = _rockPiecesSpriteList[randomSpriteIndex];
        }
    }
}