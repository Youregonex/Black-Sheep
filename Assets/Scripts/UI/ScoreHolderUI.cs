using UnityEngine;
using TMPro;
using DG.Tweening;

namespace Youregone.UI
{
    public class ScoreHolderUI : MonoBehaviour
    {
        [CustomHeader("Serritngs")]
        [SerializeField] private TextMeshProUGUI _name;
        [SerializeField] private TextMeshProUGUI _score;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _animationDuration;

        private void Awake()
        {
            GetComponent<CanvasGroup>().DOFade(1f, _animationDuration).From(0f);
        }

        public void SetData(string name, int score)
        {
            _name.text = name;
            _score.text = score.ToString();
        }
    }
}