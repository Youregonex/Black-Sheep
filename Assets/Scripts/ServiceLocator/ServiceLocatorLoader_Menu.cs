using UnityEngine;
using Youregone.SaveSystem;
using Youregone.UI;

namespace Youregone.SL
{
    public class ServiceLocatorLoader_Menu : MonoBehaviour
    {
        [Header("Services to register")]
        [SerializeField] private PlayerPrefsSaverLoader _playerPrefsSaverLoader;
        [SerializeField] private Transition _transition;
        [SerializeField] private LocalDatabase _localDatabase;

        private void Awake()
        {
            ServiceLocator.Register<PlayerPrefsSaverLoader>(_playerPrefsSaverLoader);
            ServiceLocator.Register<Transition>(_transition);
            ServiceLocator.Register<LocalDatabase>(_localDatabase);
        }
    }
}