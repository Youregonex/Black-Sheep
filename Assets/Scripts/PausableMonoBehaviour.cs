using UnityEngine;

namespace Youregone.GameSystems
{
    public abstract class PausableMonoBehaviour : MonoBehaviour
    {
        protected virtual void Start()
        {
            AddPausableObject();
        }

        protected void AddPausableObject()
        {
            PauseManager.instance.AddPausableObject(this);
        }

        public abstract void Pause();

        public abstract void UnPause();
    }
}