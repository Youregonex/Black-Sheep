using UnityEngine;
using Youregone.GameSystems;

namespace Youregone.LevelGeneration
{
    public class MovingObject : PausableMonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private float _currentXVelocity;

        protected Rigidbody2D _rigidBody;

        private void Awake() => _rigidBody = transform.GetComponent<Rigidbody2D>();

        protected override void Start()
        {
            base.Start();
            MovingObjectHandler.instance.AddObject(this);
        }

        private void Update()
        {
            _currentXVelocity = _rigidBody.velocity.x;
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.GetComponent<MovingObjectDestroyer>())
                Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            MovingObjectHandler.instance.RemoveObject(this);
        }

        public void StartMovement(float moveSpeed)
        {
            Vector2 chunkVelocity = new(-moveSpeed, 0);
            _rigidBody.velocity = chunkVelocity;
        }

        public virtual void ChangeVelocity(Vector2 newVelocity)
        {
            _rigidBody.velocity = -newVelocity;
        }

        public void StopMovement()
        {
            _rigidBody.velocity = Vector2.zero;
        }

        public override void Pause() {}
        public override void UnPause() {}
    }
}
