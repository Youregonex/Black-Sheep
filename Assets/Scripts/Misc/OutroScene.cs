using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

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

        private void Awake()
        {
            foreach (TextMeshProUGUI text in _textObjectsList)
            {
                text.alpha = 0f;
            }

            _pressAnyKeyText.alpha = 0f;
        }

        public IEnumerator ShowTextCoroutine()
        {
            foreach(TextMeshProUGUI text in _textObjectsList)
            {
                text.DOFade(1f, _textFadeDuration).SetEase(Ease.Linear);
                yield return new WaitForSeconds(_textFadeDuration);
            }

            _pressAnyKeyText.DOFade(1f, _pressAnyKeyFadeDuration).SetEase(Ease.Linear);
            yield return new WaitForSeconds(_pressAnyKeyFadeDuration);
        }
    }
}