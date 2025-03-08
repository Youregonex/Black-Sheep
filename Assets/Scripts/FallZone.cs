using UnityEngine;

namespace Youregone.LevelGeneration
{
    public class FallZone : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(collision.gameObject);
        }
    }
}