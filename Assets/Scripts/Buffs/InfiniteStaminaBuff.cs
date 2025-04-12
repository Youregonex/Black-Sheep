using UnityEngine;
using Youregone.YPlayerController;

namespace Youregone.LevelGeneration
{
    public class InfiniteStaminaBuff : Buff
    {
        [CustomHeader("Infinite Stamina Settings")]
        [SerializeField] private float _duration;

        protected override void Apply(PlayerController player)
        {
            player.TriggerInfiniteStaminaBuff(_duration);
            Destroy(gameObject);
        }
    }
}