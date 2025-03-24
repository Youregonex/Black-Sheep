using UnityEngine;

namespace Youregone.LevelGeneration
{

    public enum ChunkType
    {
        Default,
        Bridge,
        Pit
    }

    public class Chunk : MovingObject
    { 
        [CustomHeader("Config")]
        [SerializeField] private Transform _endTransform;
        [SerializeField] private ChunkType _chunkType;

        public ChunkType ChunkType => _chunkType;
        public Transform EndTransform => _endTransform;
    }
}