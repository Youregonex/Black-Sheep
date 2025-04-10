using System.Collections.Generic;
using UnityEngine;
using Youregone.Factories;
using Youregone.LevelGeneration;

namespace Youregone.ObjectPooling
{
    public class ChunkPool
    {
        private Dictionary<EChunkType, Dictionary<int, Queue<Chunk>>> _mainPoolDictionary;
        private Dictionary<EChunkType, List<Chunk>> _chunkPrefabsDictionary;
        private Factory<Chunk> _chunkFactory;
        private Transform _mainParent;
        private Dictionary<EChunkType, Transform> _parentChunkTypeDictionary = new();
        private float _playerTriggerSpawnRange; 

        public ChunkPool(Dictionary<EChunkType, List<Chunk>> chunkPrefabsDictionary, Factory<Chunk> chunkFactory, float playerTriggerSpawnRange, Transform parent)
        {
            _mainParent = parent;
            _chunkPrefabsDictionary = chunkPrefabsDictionary;
            _chunkFactory = chunkFactory;
            _playerTriggerSpawnRange = playerTriggerSpawnRange;

            CreatePools();
        }

        public Chunk Dequeue(EChunkType chunkType, int chunkID)
        {
            if (!_mainPoolDictionary.TryGetValue(chunkType, out Dictionary<int, Queue<Chunk>> innerDictionary))
            {
                Debug.Log($"Couldn't find {chunkType} chunkType pool");
                return null;
            }

            if (!innerDictionary.TryGetValue(chunkID, out Queue<Chunk> chunkPool))
            {
                Debug.Log($"Couldn't find {chunkID} id pool");
                return null;
            }

            Chunk pooledChunk;
            if (chunkPool.Count == 0)
            {
                pooledChunk = _chunkFactory.Create(_chunkPrefabsDictionary[chunkType][chunkID]);
                pooledChunk.transform.SetParent(_parentChunkTypeDictionary[chunkType]);
                pooledChunk.SetPlayerTriggerRange(_playerTriggerSpawnRange);
            }
            else
                pooledChunk = chunkPool.Dequeue();

            pooledChunk.gameObject.SetActive(true);
            return pooledChunk;
        }

        public void Enqueue(Chunk chunk)
        {
            chunk.gameObject.SetActive(false);
            _mainPoolDictionary[chunk.ChunkType][chunk.ChunkID].Enqueue(chunk);
        }

        private void CreatePools()
        {
            _mainPoolDictionary = new();

            foreach (KeyValuePair<EChunkType, List<Chunk>> chunkPrefabDictionary in _chunkPrefabsDictionary)
            {
                EChunkType currentChunkType = chunkPrefabDictionary.Key;

                GameObject typeParent = new($"Chunk {currentChunkType} Pool");
                typeParent.transform.SetParent(_mainParent);
                _parentChunkTypeDictionary.Add(chunkPrefabDictionary.Key, typeParent.transform);

                _mainPoolDictionary.Add(currentChunkType, new());

                for (int i = 0; i < chunkPrefabDictionary.Value.Count; i++)
                    _mainPoolDictionary[currentChunkType].Add(chunkPrefabDictionary.Value[i].ChunkID, new Queue<Chunk>());                
            }
        }
    }
}
