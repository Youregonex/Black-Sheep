using UnityEngine;
using UnityEngine.UI;
using Youregone.Camera;
using Youregone.UI;

namespace Youregone.GameState
{
    public class GameState : MonoBehaviour
    {
        public static GameState instance;


        [Header("Config")]
        [SerializeField] private Button _playButton;
        [SerializeField] private CameraGameStartSequence _camera;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            _playButton.onClick.AddListener(() =>
            {
                _camera.StartGame();
                ScoreCounter.instance.ScoreTextGO.SetActive(true);
                ScoreCounter.instance.HealthbarGO.SetActive(true);
            });
        }
    }
}