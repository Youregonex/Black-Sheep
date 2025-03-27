using UnityEngine;
using Youregone.YPlayerController;

namespace Youregone.LevelGeneration
{
    public class FallZone : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.GetComponent<PlayerController>())
            {
                return;
            }

            Destroy(collision.gameObject);
        }
    }
}