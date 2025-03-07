using UnityEngine;

namespace Youregone.LevelGeneration
{
    public class MovingObject : MonoBehaviour
    {
        protected Rigidbody2D _rigidBody2D;

        private void Awake()
        {
            _rigidBody2D = transform.GetComponent<Rigidbody2D>();
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.GetComponent<MovingObjectDestroyer>())
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            MovingObjectHandler.instance.RemoveObject(this);
        }

        public void StartMovement(float moveSpeed)
        {
            Vector2 chunkVelocity = new(-moveSpeed, 0);
            _rigidBody2D.velocity = chunkVelocity;
        }

        public void ChangeVelocity(Vector2 newVelocity)
        {
            _rigidBody2D.velocity = -newVelocity;
        }

        public void StopMovement()
        {
            _rigidBody2D.velocity = Vector2.zero;
        }
    }
}
