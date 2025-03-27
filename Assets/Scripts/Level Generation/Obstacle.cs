using UnityEngine;
using Youregone.SO;
using System;
using Youregone.YPlayerController;

namespace Youregone.LevelGeneration
{
    public class Obstacle : MovingObject
    {
        public event Action<Obstacle> OnDestruction;

        [CustomHeader("Obstacle Config")]
        [SerializeField] private ObstacleSO _obstacleSO;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Transform _birdSpawnPointsParent;
        [SerializeField] private int _breakPiecesCountMin;
        [SerializeField] private int _breakPiecesCountMax;

        public Transform BirdSpawnPointsParent => _birdSpawnPointsParent;
        public ObstacleSO ObstacleSO => _obstacleSO;

        private void OnEnable()
        {
            _spriteRenderer.sprite = _obstacleSO.sprites[UnityEngine.Random.Range(0, _obstacleSO.sprites.Count)];
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if(collision.transform.GetComponent<PlayerController>() || collision.transform.GetComponent<MovingObjectDestroyer>())
                OnDestruction?.Invoke(this);
        }

        public int GetBreakPiecesAmount()
        {
            return UnityEngine.Random.Range(_breakPiecesCountMin, _breakPiecesCountMax + 1);
        }
    }
}