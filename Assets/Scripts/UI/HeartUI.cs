using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Youregone.UI
{
    public class HeartUI : MonoBehaviour
    {
        private Image _image;
        private float _animationDuration;
        private float _rotateMin;
        private float _rotateMax;
        private float _targetScale;
        private Sequence _currentSequence;

        public void Initialize(float targetScale, float rotateMin, float rotateMax, float animationDuration)
        {
            _rotateMin = rotateMin;
            _rotateMax = rotateMax;
            _targetScale = targetScale;

            _image = GetComponent<Image>();
            _animationDuration = animationDuration;
        }

        private void OnDestroy()
        {
            if (_currentSequence != null)
                _currentSequence.Kill();
        }

        public void PlayDestroyAnimation()
        {
            float randomRotation = UnityEngine.Random.Range(_rotateMin, _rotateMax);
            
            _currentSequence = DOTween.Sequence();

            _currentSequence
                .Join(transform.DOMoveY(Screen.height / 2, _animationDuration))
                .Join(transform.DORotate(new Vector3(0f, 0f, randomRotation), _animationDuration))
                .Join(_image.DOFade(0f, _animationDuration))
                .Join(transform.DOScale(new Vector2(_targetScale, _targetScale), _animationDuration))
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
    }
}