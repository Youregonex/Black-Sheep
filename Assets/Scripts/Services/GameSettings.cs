using System.Collections.Generic;
using UnityEngine;
using Youregone.LevelGeneration;
using Youregone.SaveSystem;
using Youregone.SL;
using Youregone.UI;

namespace Youregone.GameSystems
{
    public class GameSettings : MonoBehaviour, IService
    {
        [CustomHeader("Progressive Difficulty")]
        [SerializeField] private bool _progressiveDifficultyEnabled;
        [SerializeField] private int _maxDifficultyScore;

        [CustomHeader("Level Generation")]
        [SerializeField, Range(0f, 1f)] private float _obstacleSpawnChance;
        [SerializeField, Range(0f, 1f)] private float _pitSpawnChanceStart;
        [SerializeField, Range(0f, 1f)] private float _pitSpawnChanceMidGame;
        [SerializeField, Range(0f, 1f)] private float _pitSpawnChanceMax;
        [Space(10f)]
        [SerializeField, Range(0f, 100f)] private float _bridgeSpawnCooldownStart;
        [SerializeField, Range(0f, 100f)] private float _bridgeSpawnCooldownMidGame;
        [SerializeField, Range(0f, 100f)] private float _bridgeSpawnCooldownMax;

        [CustomHeader("Collectables")]
        [SerializeField, Range(0, 1f)] private float _collectableSpawnChance;
        [SerializeField, Range(0f, 1f)] private float _rareCollectableSpawnChance;

        [CustomHeader("Buff Drop Settings")]
        [SerializeField, Range(0f, 1f)] private float _buffDropChance;
        [SerializeField] private List<Buff> _buffList;

        [CustomHeader("Props")]
        [SerializeField, Range(0f, 1f)] private float _birdSpawnChance;


        private PlayerPrefsSaverLoader _playerPrefsSaverLoader;

        public bool ProgressiveDifficultyEnabled => _progressiveDifficultyEnabled;
        public float ObstacleSpawnChance => _obstacleSpawnChance;
        public float BirdSpawnChance => _birdSpawnChance;

        public float BridgeSpawnCooldownStart => _bridgeSpawnCooldownStart;
        public float PitSpawnChanceStart => _pitSpawnChanceStart;
        public float BridgeSpawnCooldownMidGame => _bridgeSpawnCooldownMidGame;
        public float PitSpawnChanceMidGame => _pitSpawnChanceMidGame;
        public float BridgeSpawnCooldownMax => _bridgeSpawnCooldownMax;
        public float PitSpawnChanceMax => _pitSpawnChanceMax;
        public float BuffDropChance => _buffDropChance;
        public int MaxDifficultyScore => _maxDifficultyScore;
        public float CollectableSpawnChance => _collectableSpawnChance;
        public float RareCollectableSpawnChance => _rareCollectableSpawnChance;

        public List<Buff> BuffList => _buffList;

        public bool OutroEnabled
        {
            get
            {
                if (_playerPrefsSaverLoader == null)
                    _playerPrefsSaverLoader = ServiceLocator.Get<PlayerPrefsSaverLoader>();

                return _playerPrefsSaverLoader.OutroEnabled;
            }
        }

        private void Start()
        {
            if(_playerPrefsSaverLoader == null)
                _playerPrefsSaverLoader = ServiceLocator.Get<PlayerPrefsSaverLoader>();

            ServiceLocator.Get<GameScreenUI>().OnGameOutroToggleRequest += GameScreenUI_OnGameOutroToggleRequest;
        }

        private void OnDestroy()
        {
            ServiceLocator.Get<GameScreenUI>().OnGameOutroToggleRequest -= GameScreenUI_OnGameOutroToggleRequest;
        }

        private void GameScreenUI_OnGameOutroToggleRequest()
        {
            _playerPrefsSaverLoader.ToggleOutroEnable();
        }
    }
}