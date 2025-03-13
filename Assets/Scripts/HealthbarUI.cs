using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Youregone.PlayerControls;

namespace Youregone.UI
{
    public class HealthbarUI : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private List<Image> _heartsUIList;

        private void Start()
        {
            PlayerController.instance.OnDamageTaken += RemoveHeart;
            PlayerController.instance.OnDeath += RemoveAllHearts;
        }

        private void OnDestroy()
        {
            PlayerController.instance.OnDamageTaken -= RemoveHeart;
            PlayerController.instance.OnDeath -= RemoveAllHearts;
        }

        private void RemoveHeart()
        {
            int heartIndex = PlayerController.instance.CurrentHealth;

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