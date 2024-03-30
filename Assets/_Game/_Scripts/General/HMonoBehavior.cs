using Sirenix.OdinInspector;
using UnityEngine;

namespace HoangHH
{
    public class HMonoBehaviour : SerializedMonoBehaviour
    {
        private Transform _tf;

        public Transform Tf
        {
            get
            {
                _tf = _tf ? _tf : transform;
                return _tf;
            }
        }
    }
}
