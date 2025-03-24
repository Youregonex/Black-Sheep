using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Youregone.PlayerControls;
using Youregone.SL;

namespace Youregone.UI
{
    public class HealthbarUI : MonoBehaviour, IService
    {
        [CustomHeader("Config")]
        [SerializeField] private List<Image> _heartsUIList;

        private void Start()
        {
            ServiceLocator.Get<PlayerController>().OnDamageTaken += RemoveHeart;
            ServiceLocator.Get<PlayerController>().OnDeath += RemoveAllHearts;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().OnDamageTaken -= RemoveHeart;
            ServiceLocator.Get<PlayerController>().OnDeath -= RemoveAllHearts;
        }

        private void RemoveHeart()
        {
            int heartIndex = ServiceLocator.Get<PlayerController>().CurrentHealth;

            Destroy(_heartsUIList[heartIndex]);
            _heartsUIList.RemoveAt(heartIndex);
        }

        private void RemoveAllHearts()
        {
            foreach(Image heart in _heartsUIList)
                Destroy(heart.gameObject);
            
            _heartsUIList.Clear();
        }
    }
}