using UnityEngine;

namespace Youregone.LevelGeneration
{
    public class Obstacle : MovingObject
    {
        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(gameObject);
        }
    }
}
