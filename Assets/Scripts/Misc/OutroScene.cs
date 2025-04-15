using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using Youregone.SL;
using Youregone.YPlayerController;
using UnityEngine.UI;

namespace Youregone.UI
{
    public class OutroScene : MonoBehaviour
    {
        [CustomHeader("Config")]
        [SerializeField] private List<TextMeshProUGUI> _textObjectsList;
        [SerializeField] private TextMeshProUGUI _pressAnyKeyText;

        [CustomHeader("DOTween Config")]
        [SerializeField] private float _textFadeDuration;
        [SerializeField] private float _textFadeTarget;
        [SerializeField] private float _pressAnyKeyFadeDuration;

        public List<TextMeshProUGUI> TextObjectList => _textObjectsList;

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
                _currentTween = text.DOFade(_textFadeTarget, _textFadeDuration).From(0f).SetEase(Ease.Linear);
                yield return _currentTween.WaitForCompletion();
                _currentTween = null;
            }

            Tween pressAnyKeyTween = _pressAnyKeyText.DOFade(_textFadeTarget, _pressAnyKeyFadeDuration).From(0f).SetEase(Ease.Linear);
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