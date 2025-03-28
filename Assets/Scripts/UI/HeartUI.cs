using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Youregone.UI
{
    public class HeartUI : MonoBehaviour
    {
        private Image _image;
        private float _targetScale;
        private float _animationDuration;

        public void Initialize(float targetScale, float animationDuration)
        {
            _image = GetComponent<Image>();
            _targetScale = targetScale;
            _animationDuration = animationDuration;
        }

        public void PlayDestroyAnimation()
        {

            Sequence sequence = DOTween.Sequence();

            sequence
                .Join(transform.DOScale(_targetScale, _animationDuration))
                .Join(_image.DOFade(0f, _animationDuration))
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });

            sequence.Play();
        }
    }
}