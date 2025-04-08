using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace Youregone.UI
{
    public class ScoreHolderUI : MonoBehaviour
    {
        [CustomHeader("Settings")]
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private Image _background;
        [SerializeField] private Color _defaultBackgroundColor;
        [SerializeField] private Color _personalRecordBackgroundColor;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _animationDuration;

        private void Awake()
        {
            _background.color = _defaultBackgroundColor;
            GetComponent<CanvasGroup>().DOFade(1f, _animationDuration).From(0f);
        }

        public void SetData(string name, int score, bool isPersonalRecord)
        {
            if (isPersonalRecord)
                _background.color = _personalRecordBackgroundColor;

            _name.text = name;
            _score.text = score.ToString();
        }
    }
}