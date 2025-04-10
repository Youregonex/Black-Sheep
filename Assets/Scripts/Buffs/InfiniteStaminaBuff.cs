using UnityEngine;
using Youregone.YPlayerController;
using System.Collections;

namespace Youregone.LevelGeneration
{
    public class InfiniteStaminaBuff : Buff
    {
        [CustomHeader("Infinite Stamina Settings")]
        [SerializeField] private float _duration;
        [SerializeField] private GameObject _visual;

        private float _currentStaminaDrain;

        protected override void Apply(PlayerController player)
        {
            _visual.SetActive(false);

            StartCoroutine(ApplyInfiniteStaminaBuff(player));
        }

        private IEnumerator ApplyInfiniteStaminaBuff(PlayerController player)
        {
            _currentStaminaDrain = player.StaminaDrain;

            if (_currentStaminaDrain == 0)
            {
                Destroy(gameObject);
                yield break;
            }

            Debug.Log("Start");

            player.SetStaminaDrain(0f);
            yield return new WaitForSeconds(_duration);
            player.SetStaminaDrain(_currentStaminaDrain);
            Debug.Log("End");
            Destroy(gameObject);
        }
    }
}