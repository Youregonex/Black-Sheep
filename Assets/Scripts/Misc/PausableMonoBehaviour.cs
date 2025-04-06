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

        protected virtual void OnDestroy()
        {
            RemovePausableObject();
        }

        public abstract void Pause();
        public abstract void Unpause();

        private void AddPausableObject()
        {
            ServiceLocator.Get<PauseManager>().AddPausableObject(this);
        }

        private void RemovePausableObject()
        {
            ServiceLocator.Get<PauseManager>().RemovePausableObject(this);
        }
    }
}