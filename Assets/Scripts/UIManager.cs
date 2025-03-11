using UnityEngine;

namespace Youregone.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        [Header("Comfig")]
        [SerializeField] private ScoreCounter _scoreCounter;
        [SerializeField] private HealthbarUI _healthbar;

        public ScoreCounter ScoreCounter => _scoreCounter;
        public HealthbarUI HealthbarUI => _healthbar;


        private void Awake()
        {
            instance = this;
        }
    }
}