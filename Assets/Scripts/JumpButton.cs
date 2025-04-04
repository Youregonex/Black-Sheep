using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace Youregone.UI
{
    public class JumpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action OnJumpButtonPressed;
        public event Action OnJumpButtonReleased;

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
            OnJumpButtonPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnJumpButtonReleased?.Invoke();
        }
    }
}