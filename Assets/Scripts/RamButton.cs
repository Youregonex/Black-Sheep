using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace Youregone.UI
{
    public class RamButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnRamButtonPressed;
        public event Action OnRamButtonReleased;

        public Button Button { get; private set; }

        private void Awake()
        {
            Button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnRamButtonPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnRamButtonReleased?.Invoke();
        }
    }
}