using System;
using System.Collections.Generic;
using UnityEngine;

namespace Youregone.SL
{
    public class ServiceLocator
    {
        private static System.Collections.Generic.Dictionary<Type, IService> _services = new();

        public static void Register<T>(T service) where T : IService
        {
            Type type = typeof(T);

            if (_services.ContainsKey(type))
            {
                //Debug.LogWarning($"Service {type.Name} is already registered and will be replaced!");
            }

            _services[type] = service;
        }

        public static T Get<T>() where T : IService
        {
            Type type = typeof(T);

            if (!_services.ContainsKey(type))
            {
                Debug.LogWarning($"Service {type.Name} is not registered!");
                return default;
            }

            return (T)_services[type];
        }

        public static void Unregister<T>() where T : IService
        {
            var type = typeof(T);

            if (_services.ContainsKey(type))
            {
                _services.Remove(type);
            }
            else
            {
                Debug.LogWarning($"Service {type.Name} was not registered");
            }
        }
    }
}