using UnityEngine;
using System;
using Youregone.LevelGeneration;

namespace Youregone.PlayerControls
{
    public class GroundCheck : MonoBehaviour
    {
        public Action Landed;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.GetComponent<Chunk>() && !PlayerController.instance.IsGrounded)
            {
                Landed?.Invoke();
            }
        }
    }
}
