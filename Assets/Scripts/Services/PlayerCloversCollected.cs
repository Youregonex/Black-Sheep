using UnityEngine;
using Youregone.SaveSystem;
using Youregone.SL;
using Youregone.YPlayerController;
using System;
using Youregone.UI;

namespace Youregone.GameSystems
{
    public class PlayerCloversCollected : MonoBehaviour, IService
    {
        public Action<int> OnBaseCloverCollected;
        public Action<int> OnRareCloverCollected;

        [CustomHeader("Debug")]
        [SerializeField] private int _baseCloverSaved;
        [SerializeField] private int _rareCloverSaved;
        [SerializeField] private int _baseCloversCollected;
        [SerializeField] private int _rareCloversCollected;

        private PlayerPrefsSaverLoader _playerPrefs;

        public int BaseCloversCollected => _baseCloversCollected;
        public int RareCloversCollected => _rareCloversCollected;

        private void Start()
        {
            ServiceLocator.Get<PlayerController>().OnDeath += PlayerController_OnDeath;
            _playerPrefs = ServiceLocator.Get<PlayerPrefsSaverLoader>();

            _baseCloverSaved = _playerPrefs.GetBaseCloverAmount();
            _rareCloverSaved = _playerPrefs.GetRareCloverAmount();

            ServiceLocator.Get<GameScreenUI>().CloversCollectedUI.Initialize(_baseCloverSaved, _rareCloverSaved);
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().OnDeath -= PlayerController_OnDeath;
        }

        public void CollectClover(bool rareClover)
        {
            if (rareClover)
            {
                _rareCloversCollected++;
                OnRareCloverCollected?.Invoke(_rareCloversCollected + _rareCloverSaved);
            }
            else
            {
                _baseCloversCollected++;
                OnBaseCloverCollected?.Invoke(_baseCloversCollected + _baseCloverSaved);
            }
        }

        private void PlayerController_OnDeath()
        {
            _playerPrefs.SavePlayerClovers(_baseCloversCollected, _rareCloversCollected);
        }
    }
}