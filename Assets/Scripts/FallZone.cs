using UnityEngine;
using Youregone.PlayerControls;
using UnityEngine.SceneManagement;

namespace Youregone.LevelGeneration
{
    public class FallZone : MonoBehaviour
    {
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.transform.GetComponent<PlayerController>())
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            Destroy(collision.gameObject);
        }
    }
}