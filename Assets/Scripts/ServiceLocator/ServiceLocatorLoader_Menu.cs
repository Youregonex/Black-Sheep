using UnityEngine;
using Youregone.GameSystems;
using Youregone.SaveSystem;
using Youregone.SoundFX;
using Youregone.UI;

namespace Youregone.SL
{
    public class ServiceLocatorLoader_Menu : MonoBehaviour
    {
        [Header("Services to register")]
        [SerializeField] private PlayerPrefsSaverLoader _playerPrefsSaverLoader;
        [SerializeField] private Transition _transition;
        [SerializeField] private LocalDatabase _localDatabase;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private Music _mainMusic; 

        private void Awake()
        {
            ServiceLocator.Register(_playerPrefsSaverLoader);
            ServiceLocator.Register(_transition);
            ServiceLocator.Register(_localDatabase);
            ServiceLocator.Register(_soundManager);
            ServiceLocator.Register(_mainMusic);
        }
    }
}