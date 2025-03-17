using UnityEngine;
using System.Collections.Generic;
using System;
using DG.Tweening;
using Youregone.SL;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class RockBreakPiece : MovingObject
    {
        public event Action<RockBreakPiece> OnDestruction;

        [Header("Config")]
        [SerializeField] private Vector2 _direction;
        [SerializeField] private float _forceMultiplier;
        [SerializeField] private List<Sprite> _rockPiecesSpriteList;
        [SerializeField] private float _fadeDuration;
        [SerializeField] private float _slowdownDuration;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        protected override void Start()
        {
            AddPausableObject();
        }

        private void OnEnable()
        {
            PickRandomSprite();
            _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);

            float randomXForceModifier = UnityEngine.Random.Range(.5f, 1f);
            float randomYForceModifier = UnityEngine.Random.Range(1f, 2f);

            _rigidBody.velocity = Vector2.zero;
            _rigidBody.AddForce(new Vector2(_direction.x * _forceMultiplier * randomXForceModifier, _direction.y * _forceMultiplier * randomYForceModifier), ForceMode2D.Impulse);

            DOTween.To(
                      () => _rigidBody.velocity.x,
                      x => _rigidBody.velocity = new Vector2(x, _rigidBody.velocity.y),
                      -ServiceLocator.Get<PlayerController>().CurrentSpeed,
                      _slowdownDuration
                  ).SetEase(Ease.OutQuad);

            _spriteRenderer.DOFade(0f, _fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                OnDestruction?.Invoke(this);
            });
        }

        private void PickRandomSprite()
        {
            int randomSpriteIndex = UnityEngine.Random.Range(0, _rockPiecesSpriteList.Count);
            _spriteRenderer.sprite = _rockPiecesSpriteList[randomSpriteIndex];
        }
    }
}