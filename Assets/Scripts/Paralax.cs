using UnityEngine;
using Youregone.PlayerControls;
using Youregone.GameSystems;

namespace Youregone.LevelGeneration
{
    public class Paralax : MovingObject, IUpdateObserver
    {
        [Header("Paralax Config")]
        [SerializeField] private float _paralaxSpawnDistance;
        [SerializeField] private float _paralaxSpawnXOffset;
        [SerializeField] private float _paralaxFactor;
        [SerializeField] private Paralax _paralaxLayerPrefab;
        [SerializeField] private Transform _paralaxEndPoint;

        [Header("Debug")]
        [SerializeField] private bool _paralaxCreated = false;

        private PlayerController _player;

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateManager.RegisterUpdateObserver(this);
        }

        protected override void Start()
        {
            base.Start();

            _player = PlayerController.instance;
            ChangeVelocity(new Vector2(_player.CurrentSpeed, 0f));
        }

        public void ObservedUpdate()
        {
            if (_player == null)
                return;

            if (_paralaxEndPoint.transform.position.x - _player.transform.position.x <= _paralaxSpawnDistance && !_paralaxCreated)
                CreateParalax();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<MovingObjectDestroyer>())
                Destroy(gameObject);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UpdateManager.UnregisterUpdateObserver(this);
        }

        public override void ChangeVelocity(Vector2 newVelocity)
        {
            _rigidBody.velocity = -newVelocity * _paralaxFactor;
        }

        private void CreateParalax()
        {
            Vector2 paralaxSpawnPosition = new(_paralaxEndPoint.position.x + _paralaxSpawnXOffset, transform.position.y);
            Paralax paralax = Instantiate(_paralaxLayerPrefab, paralaxSpawnPosition, Quaternion.identity);
            paralax.transform.parent = transform.parent;
            paralax.gameObject.name = gameObject.name;
            _paralaxCreated = true;
        }
    }
}