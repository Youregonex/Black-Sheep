using UnityEngine;
using TMPro;
using Youregone.SL;
using Youregone.GameSystems;

namespace Youregone.UI
{
    public class CloversCollectedUI : MonoBehaviour
    {
        [CustomHeader("Elements")]
        [SerializeField] private TextMeshProUGUI _baseCloverCountText;
        [SerializeField] private TextMeshProUGUI _rareCloverCountText;

        public void Initialize(int baseCloversAmount, int rareCloversAmount)
        {
            ServiceLocator.Get<PlayerCloversCollected>().OnBaseCloverCollected += UpdateBaseCloverTextAmount;
            ServiceLocator.Get<PlayerCloversCollected>().OnRareCloverCollected += UpdateRareCloverTextAmount;

            UpdateBaseCloverTextAmount(baseCloversAmount);
            UpdateRareCloverTextAmount(rareCloversAmount);
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerCloversCollected>().OnBaseCloverCollected -= UpdateBaseCloverTextAmount;
            ServiceLocator.Get<PlayerCloversCollected>().OnRareCloverCollected -= UpdateRareCloverTextAmount;
        }

        private void UpdateBaseCloverTextAmount(int amount)
        {
            _baseCloverCountText.text = amount.ToString();
        }

        private void UpdateRareCloverTextAmount(int amount)
        {
            _rareCloverCountText.text = amount.ToString();
        }
    }
}