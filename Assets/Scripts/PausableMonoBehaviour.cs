using UnityEngine;
using Youregone.SL;

namespace Youregone.GameSystems
{
    public abstract class PausableMonoBehaviour : MonoBehaviour
    {
        protected virtual void Start()
        {
            AddPausableObject();
        }

        public abstract void Pause();
        public abstract void UnPause();

        protected void AddPausableObject()
        {
            ServiceLocator.Get<PauseManager>().AddPausableObject(this);
        }
    }
}