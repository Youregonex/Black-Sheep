using UnityEngine;

namespace Youregone.Factories
{
    public class Factory<T> where T : Object
    {
        public T Create(T prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("Prefab is null");
                return null;
            }

            return Object.Instantiate(prefab);
        }
    }
}