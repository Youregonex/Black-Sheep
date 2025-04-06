using UnityEngine;
using Youregone.GameSystems;
using Youregone.SL;

namespace Youregone.LevelGeneration
{
    public class MovingObject : PausableMonoBehaviour
    {
        [CustomHeader("Debug")]
        [SerializeField] private float _currentXVelocity;

        protected Rigidbody2D _rigidBody;

        protected virtual void Awake() => _rigidBody = transform.GetComponent<Rigidbody2D>();

        protected override void Start()
        {
            base.Start();
            ServiceLocator.Get<MovingObjectHandler>().AddObject(this);
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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ServiceLocator.Get<MovingObjectHandler>().RemoveObject(this);
        }

        public virtual void ChangeVelocity(Vector2 newVelocity)
        {
            _rigidBody.velocity = -newVelocity;
        }

        public virtual void StopMovement()
        {
            _rigidBody.velocity = Vector2.zero;
        }

        public override void Pause() {}
        public override void Unpause() {}
    }
}
