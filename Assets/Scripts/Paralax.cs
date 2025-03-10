using UnityEngine;
using Youregone.PlayerControls;

namespace Youregone.LevelGeneration
{
    public class Paralax : MovingObject
    {
        [Header("Paralax Config")]
        [SerializeField] private float _paralaxSpawnDistance;
        [SerializeField] private float _paralaxSpawnXOffset;
        [SerializeField] private float _paralaxFactor;
        [SerializeField] private Paralax _paralaxLayerPrefab;
        [SerializeField] private Transform _paralaxEndPoint;

        [Header("Test")]
        [SerializeField] private PlayerController _player;
        [SerializeField] private bool _paralaxCreated = false;

        private void Start()
        {
            _player = PlayerController.instance;

            MovingObjectHandler.instance.AddObject(this);
            ChangeVelocity(new Vector2(_player.CurrentSpeed, 0f));
        }

        private void Update()
        {
            if (_player == null)
                return;

            if(_paralaxEndPoint.transform.position.x - _player.transform.position.x <= _paralaxSpawnDistance && !_paralaxCreated)
                CreateParalax();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<MovingObjectDestroyer>())
                Destroy(gameObject);
        }

        public override void ChangeVelocity(Vector2 newVelocity)
        {
            _rigidBody2D.velocity = -newVelocity * _paralaxFactor;
        }

        private void CreateParalax()
        {
            Vector2 paralaxSpawnPosition = new(_paralaxEndPoint.position.x + _paralaxSpawnXOffset, transform.position.y);
            Paralax paralax = Instantiate(_paralaxLayerPrefab, paralaxSpawnPosition, Quaternion.identity);
            paralax.gameObject.name = gameObject.name;
            _paralaxCreated = true;
        }
    }
}