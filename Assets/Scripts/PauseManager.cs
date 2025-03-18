using System.Collections.Generic;
using UnityEngine;
using Youregone.PlayerControls;
using Youregone.SL;

namespace Youregone.GameSystems
{
    public class PauseManager : MonoBehaviour, IService
    {
        [Header("Config")]
        [SerializeField] private GameObject _pauseBackground;

        [Header("Debug")]
        [SerializeField] private bool _gamePaused;

        private List<PausableMonoBehaviour> _pausableObjectList = new();


        private void Start()
        {
            ServiceLocator.Get<PlayerController>().OnPauseTriggered += PlayerController_OnPauseTriggered;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<PlayerController>().OnPauseTriggered -= PlayerController_OnPauseTriggered;
        }

        private void PlayerController_OnPauseTriggered()
        {
            if (_gamePaused)
            {
                UnPauseGame();
                return;
            }

            PauseGame();
        }

        public void PauseGame()
        {
            _gamePaused = true;
            _pauseBackground.gameObject.SetActive(true);

            foreach (PausableMonoBehaviour pausable in _pausableObjectList)
                pausable.Pause();
        }

        public void UnPauseGame()
        {
            _gamePaused = false;
            _pauseBackground.gameObject.SetActive(false);

            foreach (PausableMonoBehaviour pausable in _pausableObjectList)
                pausable.Unpause();
        }

        public void AddPausableObject(PausableMonoBehaviour pausable)
        {
            if (_pausableObjectList.Contains(pausable))
                return;

            _pausableObjectList.Add(pausable);
        }

        public void RemovePausableObject(PausableMonoBehaviour pausable)
        {
            if (!_pausableObjectList.Contains(pausable))
                return;

            _pausableObjectList.Remove(pausable);
        }
    }
}