using UnityEngine;

namespace HoangHH.DesignPattern
{
    /// <summary>
    /// Singleton pattern for MonoBehaviour
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class HSingleton<T> : HMonoBehaviour where T : HMonoBehaviour
    {
        private static T _instance;

        private void Awake()
        {
            if (!_instance) _instance = this as T;
            OnSingletonAwaken();
        }

        /// <summary>
        /// Override this method to add more logic when singleton is awake instead of using Awake method
        /// </summary>
        protected virtual void OnSingletonAwaken()
        { }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static T Ins
        {
            get
            {
                if (_instance) return _instance;
                _instance = FindObjectOfType<T>() ?? new GameObject().AddComponent<T>();
                return _instance;
            }
        }
    }
}
