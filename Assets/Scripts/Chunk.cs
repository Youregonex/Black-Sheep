using UnityEngine;

namespace Youregone.LevelGeneration
{
    public class Chunk : MovingObject
    {
        [SerializeField] private Transform _endTransform;

        public Transform EndTransform => _endTransform;
    }
}