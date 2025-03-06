using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Youregone.PlayerControls;

namespace Youregone.UI
{
    public class HealthbarUI : MonoBehaviour
    {
        [SerializeField] private List<Image> _heartsUIList;

        private void Start()
        {
            PlayerController.instance.OnDamageTaken += DeleteHeart;
        }

        private void OnDestroy()
        {
            PlayerController.instance.OnDamageTaken -= DeleteHeart;
        }

        private void DeleteHeart()
        {
            int heartIndex = PlayerController.instance.CurrentHealth;

            Destroy(_heartsUIList[heartIndex]);
        }
    }
}