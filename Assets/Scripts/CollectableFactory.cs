using UnityEngine;
using Youregone.LevelGeneration;

namespace Youregone.Factories
{

    public class CollectableFactory
    {
        public Collectable CreateCollectable(Collectable collectablePrefab)
        {
            return GameObject.Instantiate(collectablePrefab);
        }
    }
}