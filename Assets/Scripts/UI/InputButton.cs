using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Youregone.UI
{
    public class InputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnButtonPressed;
        public event Action OnButtonReleased;

        public Button Button { get; private set; }

        private void Awake()
        {
            Button = GetComponent<Button>();

            if (!(SystemInfo.deviceType == DeviceType.Handheld))
            {
                gameObject.SetActive(false);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnButtonPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnButtonReleased?.Invoke();
        }
    }
}