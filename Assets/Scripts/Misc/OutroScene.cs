using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using Youregone.SL;
using Youregone.YPlayerController;

namespace Youregone.GameSystems
{
    public class OutroScene : MonoBehaviour
    {
        [CustomHeader("Config")]
        [SerializeField] private List<TextMeshProUGUI> _textObjectsList;
        [SerializeField] private TextMeshProUGUI _pressAnyKeyText;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [CustomHeader("DOTween Config")]
        [SerializeField] private float _textFadeDuration;
        [SerializeField] private float _pressAnyKeyFadeDuration;

        public List<TextMeshProUGUI> TextObjectList => _textObjectsList;
        public SpriteRenderer SpriteRenderer => _spriteRenderer;

        private Tween _currentTween;

        private void Awake()
        {
            foreach (TextMeshProUGUI text in _textObjectsList)
            {
                text.alpha = 0f;
            }

            _pressAnyKeyText.alpha = 0f;
        }

        private void Start()
        {
            ServiceLocator.Get<PlayerController>().PlayerCharacterInput.OnScreenTap += ForceCompleteCurrentTween;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().PlayerCharacterInput.OnScreenTap -= ForceCompleteCurrentTween;
        }

        public IEnumerator ShowTextCoroutine()
        {
            foreach(TextMeshProUGUI text in _textObjectsList)
            {
                _currentTween = text.DOFade(1f, _textFadeDuration).SetEase(Ease.Linear);
                yield return _currentTween.WaitForCompletion();
                _currentTween = null;
            }

            Tween pressAnyKeyTween = _pressAnyKeyText.DOFade(1f, _pressAnyKeyFadeDuration).SetEase(Ease.Linear);
            yield return pressAnyKeyTween.WaitForCompletion();
        }

        private void ForceCompleteCurrentTween()
        {
            if (_currentTween == null)
                return;

            _currentTween.Complete();
            _currentTween = null;
        }
    }
}