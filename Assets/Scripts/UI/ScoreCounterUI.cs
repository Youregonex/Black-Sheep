using UnityEngine;
using TMPro;
using Youregone.GameSystems;
using Youregone.SL;
using DG.Tweening;

namespace Youregone.UI
{
    public class ScoreCounterUI : MonoBehaviour
    {
        [CustomHeader("Config")]
        [SerializeField] private TextMeshProUGUI _scoreText;

        [CustomHeader("DOTween Settings")]
        [SerializeField] private float _animationDuration;
        [SerializeField] private float _scaleTo;
        [SerializeField] private Color _colorTo;
        [SerializeField] private Color _baseColor;

        private Sequence _currentSequence;

        private void Start()
        {
            ServiceLocator.Get<ScoreCounter>().OnScoreChanged += ScoreCounter_OnScoreChanged;
            ServiceLocator.Get<ScoreCounter>().OnComboPointsAwarded += PlayAnimation;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<ScoreCounter>().OnScoreChanged -= ScoreCounter_OnScoreChanged;
            ServiceLocator.Get<ScoreCounter>().OnComboPointsAwarded -= PlayAnimation;
        }

        private void PlayAnimation()
        {
            if (_currentSequence != null)
                _currentSequence.Kill();

            _currentSequence = DOTween.Sequence();

            _currentSequence
                .Append(_scoreText.transform.DOScale(_scaleTo, _animationDuration / 2))
                .Join(_scoreText.DOColor(_colorTo, _animationDuration / 2))
                .Append(_scoreText.transform.DOScale(Vector2.one, _animationDuration / 2))
                .Join(_scoreText.DOColor(_baseColor, _animationDuration / 2))
                .OnComplete(() => _currentSequence = null);
            _currentSequence.Play();
        }

        private void ScoreCounter_OnScoreChanged(int updatedScore)
        {
            _scoreText.text = $"Score: {updatedScore}";
        }
    }
}