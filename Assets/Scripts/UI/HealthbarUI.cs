using UnityEngine;
using System.Collections.Generic;
using Youregone.YPlayerController;
using Youregone.SL;
using DG.Tweening;

namespace Youregone.UI
{
    public class HealthbarUI : MonoBehaviour, IService
    {
        [CustomHeader("Settings")]
        [SerializeField] private List<HeartUI> _heartsUIList;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _heartTargetScale;
        [SerializeField] private float _heartAnimationDuration;

        private void Start()
        {
            ServiceLocator.Get<PlayerController>().OnDamageTaken += RemoveHeart;
            ServiceLocator.Get<PlayerController>().OnDeath += RemoveAllHearts;

            foreach(HeartUI heartUI in _heartsUIList)
                heartUI.Initialize(_heartTargetScale, _heartAnimationDuration);
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
            ServiceLocator.Get<PlayerController>().OnDamageTaken -= RemoveHeart;
            ServiceLocator.Get<PlayerController>().OnDeath -= RemoveAllHearts;
        }

        private void RemoveHeart()
        {
            int heartIndex = ServiceLocator.Get<PlayerController>().CurrentHealth;

            _heartsUIList[heartIndex].PlayDestroyAnimation();
            _heartsUIList.RemoveAt(heartIndex);
        }

        private void RemoveAllHearts()
        {
            foreach(HeartUI heart in _heartsUIList)
                Destroy(heart.gameObject);
            
            _heartsUIList.Clear();
        }
    }
}