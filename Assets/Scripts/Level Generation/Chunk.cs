using UnityEngine;
using Youregone.SL;
using System;
using Youregone.YPlayerController;

namespace Youregone.LevelGeneration
{
    public class Chunk : MovingObject
    {
        public Action<Chunk> OnDestruction;
        public Action<Chunk> OnPlayerInRange;

        [CustomHeader("Config")]
        [SerializeField] private Transform _endTransform;

        [CustomHeader("Don't change after initialization")]
        [SerializeField] private EChunkType _chunkType;
        [SerializeField] private int _id;

        private CircleCollider2D _playerTrigger;

        public EChunkType ChunkType => _chunkType;
        public Transform EndTransform => _endTransform;
        public int ChunkID => _id;

        protected override void Awake()
        {
            base.Awake();
            _playerTrigger = GetComponent<CircleCollider2D>();
        }

        private void OnEnable()
        {
            ServiceLocator.Get<MovingObjectHandler>().AddObject(this);
        }

        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.GetComponent<MovingObjectDestroyer>())
                OnDestruction?.Invoke(this);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<PlayerController>())
                OnPlayerInRange?.Invoke(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Get<MovingObjectHandler>().RemoveObject(this);
        }

        public void SetPlayerTriggerRange(float playerTriggerSpawnRange)
        {
            _playerTrigger.radius = playerTriggerSpawnRange;
        }
    }
}