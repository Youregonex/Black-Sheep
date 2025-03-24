using UnityEngine;
using System;

namespace Youregone.PlayerControls
{
    public class GroundCheck : MonoBehaviour
    {
        public event Action Landed;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Landed?.Invoke();
        }
    }
}
