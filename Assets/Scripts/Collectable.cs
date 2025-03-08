using UnityEngine;
using Youregone.PlayerControls;
using Youregone.UI;

namespace Youregone.LevelGeneration
{
    public class Collectable : MovingObject
    {
        [Header("Collectable Config")]
        [SerializeField] private int _pointsBonus;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<PlayerController>())
            {
                ScoreCounter.instance.AddPoints(_pointsBonus);
                Destroy(gameObject);
            }
        }
    }
}