using UnityEngine;
using System.Collections.Generic;

namespace Youregone.GameSystems
{
    public class UpdateManager : MonoBehaviour
    {
        private static List<IUpdateObserver> _updateObserversList = new();
        private static List<IUpdateObserver> _pendingUpdateObserversList = new();
        private static int _currentIndex;

        private void Update()
        {
            for (_currentIndex = _updateObserversList.Count - 1; _currentIndex >= 0; _currentIndex--)
            {
                _updateObserversList[_currentIndex].ObservedUpdate();
            }

            _updateObserversList.AddRange(_pendingUpdateObserversList);
            _pendingUpdateObserversList.Clear();
        }

        public static void RegisterUpdateObserver(IUpdateObserver updateObserver)
        {
            if (_updateObserversList.Contains(updateObserver))
                return;

            _updateObserversList.Add(updateObserver);
        }

        public static void UnregisterUpdateObserver(IUpdateObserver updateObserver)
        {
            if (!_updateObserversList.Contains(updateObserver))
                return;

            _updateObserversList.Remove(updateObserver);
            _currentIndex--;
        }
    }
}