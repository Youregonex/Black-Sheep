using UnityEngine;
using System;
using Youregone.LevelGeneration;

namespace Youregone.PlayerControls
{
    public class GroundCheck : MonoBehaviour
    {
        public event Action Landed;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((collision.transform.GetComponent<Chunk>() || collision.transform.root.GetComponent<Chunk>())  && !PlayerController.instance.IsGrounded)
            {
                Landed?.Invoke();
            }
        }
    }
}
