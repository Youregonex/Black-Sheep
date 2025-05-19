using UnityEngine;
using UnityEngine.EventSystems;
using Youregone.GameSystems;
using Youregone.SL;

namespace Youregone.UI
{
    public class UISound : MonoBehaviour, IPointerClickHandler
    {
        private SoundManager _soundManager;

        private void Start()
        {
            if (_soundManager == null)
                _soundManager = ServiceLocator.Get<SoundManager>();
        }

        private void OnEnable()
        {
            if(_soundManager == null)
                _soundManager = ServiceLocator.Get<SoundManager>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _soundManager.PlayUIClick();
        }
    }
}
