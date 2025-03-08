using UnityEngine;

namespace Youregone.LevelGeneration
{
    public class MovingObject : MonoBehaviour
    {
        [Header("Test")]
        [SerializeField] private float _currentVelocityX;
        [SerializeField] protected Rigidbody2D _rigidBody2D;

        private void Awake()
        {
            _rigidBody2D = transform.GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _currentVelocityX = _rigidBody2D.velocity.x;
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
            _rigidBody2D.velocity = chunkVelocity;
        }

        public virtual void ChangeVelocity(Vector2 newVelocity)
        {
            _rigidBody2D.velocity = -newVelocity;
        }

        public void StopMovement()
        {
            _rigidBody2D.velocity = Vector2.zero;
        }
    }
}
