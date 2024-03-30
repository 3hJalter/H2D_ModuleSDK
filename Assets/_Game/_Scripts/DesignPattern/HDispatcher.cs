using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoangHH.DesignPattern
{
    /// <summary>
    /// Simple dispatcher for sending event to other listener
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HDispatcher<T> : HSingleton<T> where T : HMonoBehaviour
    {
        private readonly Dictionary<EventID, Action> _listenerEventDictionary = new();
        private readonly Dictionary<EventID, Action<object>> _listenerEventDictionaryParam = new();
        
        public void RegisterListenerEvent(EventID eventID, Action callback)
        {
            if (!_listenerEventDictionary.TryAdd(eventID, callback))
                _listenerEventDictionary[eventID] += callback;
        }

        public void RegisterListenerEvent(EventID eventID, Action<object> callback)
        {
            if (!_listenerEventDictionaryParam.TryAdd(eventID, callback))
                _listenerEventDictionaryParam[eventID] += callback;
        }

        public void UnregisterListenerEvent(EventID eventID, Action callback)
        {
            if (_listenerEventDictionary.ContainsKey(eventID))
                _listenerEventDictionary[eventID] -= callback;
        }

        public void UnregisterListenerEvent(EventID eventID, Action<object> callback)
        {
            if (_listenerEventDictionaryParam.ContainsKey(eventID))
                _listenerEventDictionaryParam[eventID] -= callback;
        }

        public void PostEvent(EventID eventID)
        {
            if (_listenerEventDictionary.TryGetValue(eventID, out Action value))
                value.Invoke();
        }
        
        public void PostEvent(EventID eventID, object param)
        {
            if (_listenerEventDictionaryParam.TryGetValue(eventID, out Action<object> value))
                value.Invoke(param);
        }

        public void ClearAllListenerEvent()
        {
            _listenerEventDictionary.Clear();
        }
    }

    public enum EventID
    {
    }
}
