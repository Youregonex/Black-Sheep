using System.Collections.Generic;
using UnityEngine;
using Youregone.PlayerControls;

namespace Youregone.GameSystems
{
    public class PauseManager : MonoBehaviour
    {
        public static PauseManager instance;

        [Header("Config")]
        [SerializeField] private GameObject _pauseBackground;

        [Header("Debug")]
        [SerializeField] private bool _gamePaused;

        private List<PausableMonoBehaviour> _pausableObjectList = new();

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            PlayerController.instance.OnPauseTriggered += PlayerController_OnPauseTriggered;
        }

        private void OnDestroy()
        {
            PlayerController.instance.OnPauseTriggered -= PlayerController_OnPauseTriggered;
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
                pausable.UnPause();
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