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
        [SerializeField] private HeartUI _heartPrefab;
        [SerializeField] private Transform _heartParent;
        [SerializeField] private RectTransform _tempParent;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _heartAnimationDuration;
        [SerializeField] private float _rotateMin;
        [SerializeField] private float _rotateMax;
        [SerializeField] private float _targetScale;

        private void Start()
        {
            ServiceLocator.Get<PlayerController>().OnDamageTaken += RemoveHeart;
            ServiceLocator.Get<PlayerController>().OnDeath += RemoveAllHearts;
            ServiceLocator.Get<PlayerController>().OnHealthAdded += PlayerController_OnHealthAdded; ;

            foreach (HeartUI heartUI in _heartsUIList)
                heartUI.Initialize(_targetScale, _rotateMin, _rotateMax, _heartAnimationDuration);
        }

        private void PlayerController_OnHealthAdded()
        {
            HeartUI heartUI = Instantiate(_heartPrefab);
            heartUI.transform.SetParent(_heartParent);
            heartUI.Initialize(_targetScale, _rotateMin, _rotateMax, _heartAnimationDuration);
            _heartsUIList.Add(heartUI);
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
            ServiceLocator.Get<PlayerController>().OnDamageTaken -= RemoveHeart;
            ServiceLocator.Get<PlayerController>().OnDeath -= RemoveAllHearts;
            ServiceLocator.Get<PlayerController>().OnHealthAdded -= PlayerController_OnHealthAdded;
        }

        private void RemoveHeart()
        {
            HeartUI lastChild = _heartsUIList[^1];

            lastChild.PlayDestroyAnimation();
            lastChild.transform.SetParent(_tempParent);
            _heartsUIList.Remove(lastChild);
        }

        private void RemoveAllHearts()
        {
            foreach(HeartUI heart in _heartsUIList)
                Destroy(heart.gameObject);
            
            _heartsUIList.Clear();
        }
    }
}