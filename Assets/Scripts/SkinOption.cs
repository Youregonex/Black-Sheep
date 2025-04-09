using UnityEngine;
using UnityEngine.UI;

namespace Youregone.UI
{
    public class SkinOption : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private Image _image;

        public RectTransform SelfRectTransform { get; private set; }

        private void Awake()
        {
            SelfRectTransform = GetComponent<RectTransform>(); 
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }
    }
}