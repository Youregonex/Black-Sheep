using UnityEngine;
using Youregone.EnemyAI;

namespace Youregone.LevelGeneration
{
    public class BridgeChunk : Chunk
    {
        [CustomHeader("Bridge Chunk Settings")]
        [SerializeField] private Transform _enemySpawnPoint;
        [SerializeField] private Enemy _enemyPrefab;

        protected override void OnEnable()
        {
            base.OnEnable();
            Enemy enemy = Instantiate(_enemyPrefab, _enemySpawnPoint.position, Quaternion.identity);
            enemy.transform.SetParent(transform);
        }
    }
}