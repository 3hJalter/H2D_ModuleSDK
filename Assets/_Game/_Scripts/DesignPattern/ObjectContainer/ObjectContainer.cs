using System;
using System.Collections.Generic;
using UnityEngine;

namespace HoangHH.DesignPattern
{
    public abstract class ObjectContainer<T> : MonoBehaviour where T : Component
    {
        public enum ResetAction
        {
            None = -1,
            SetActiveFalse = 0,
            SetActiveTrue = 1,
        }

        private readonly List<List<ObjContainerData<T>>> _data = new ();
        public GameObject[] mainObjects;
        public int[] amounts;

        private readonly List<List<bool>> _availableState = new();
        private readonly List<int> _startFindIndex = new();

        public void OnInit(GameObject mainObject, int amount, bool isPopMain = false)
        {
            mainObjects = new[] {mainObject};
            amounts = new[] {amount};
            OnInit(isPopMain);
        }

        public void OnInit(bool isPopMain = false)
        {
            for (int i = 0; i < mainObjects.Length; i++)
            {
                _data.Add(new List<ObjContainerData<T>>());
                _availableState.Add(new List<bool>());
                _startFindIndex.Add(0);

                Transform parent = mainObjects[i].transform.parent;
                for (int j = 0; j < amounts[i]; j++)
                {
                    ObjContainerData<T> dataContain = new(j);
                    GameObject main = j == 0 ? mainObjects[i] : Instantiate(mainObjects[i], parent);

                    Component[] component = main.GetComponents<Component>();
                    foreach (Component comp in component)
                    {
                        switch (comp)
                        {
                            case Transform tf:
                                dataContain.Tf = tf;
                                break;
                            case T t:
                                dataContain.data = t;
                                break;
                        }
                    }

                    _data[i].Add(dataContain);
                    if (j == 0)
                    {
                        _availableState[i].Add(!isPopMain);
                    }
                    else
                    {
                        _availableState[i].Add(true);
                    }
                }

            }
        }

        public ObjContainerData<T> Pop(int id)
        {
            int index = _availableState[id].FindIndex(_startFindIndex[id], x => x);

            #region FIND_START_INDEX

            if (index < 0)
            {
                _startFindIndex[id] = 0;
                index = _availableState[id].FindIndex(_startFindIndex[id], x => x);
            }
            else if (index == _availableState[id].Count - 1)
            {
                _startFindIndex[id] = 0;
            }
            else
            {
                _startFindIndex[id] = index + 1;
            }

            #endregion

            _availableState[id][index] = false;
            ObjContainerData<T> dataCon = _data[id][index];
            return dataCon;
        }

        public void Push(int id, ObjContainerData<T> dataCon)
        {
            _availableState[id][dataCon.Id] = true;
        }

        public void Push(int id, Transform tf)
        {
            int index = _data[id].Find(x => x.Tf == tf).Id;
            _availableState[id][index] = true;
        }

        public void Push(int id, int objId)
        {
            _availableState[id][objId] = true;
        }

        public void ResetContainer(ResetAction action = ResetAction.None)
        {

            #region ADD_RESET_ACTION

            Action<int, int> resetAction = null;
            switch (action)
            {
                case ResetAction.SetActiveFalse:
                case ResetAction.SetActiveTrue:
                    resetAction = (i, j) =>
                        _data[i][j].Tf.gameObject.SetActive(action != ResetAction.SetActiveFalse);
                    break;
            }

            #endregion

            for (int i = 0; i < _availableState.Count; i++)
            {
                for (int j = 0; j < _availableState[i].Count; j++)
                {
                    _availableState[i][j] = true;
                    resetAction?.Invoke(i, j);
                }
            }

            for (int i = 0; i < _startFindIndex.Count; i++)
            {
                _startFindIndex[i] = 0;
            }
        }
    }

    public class ObjContainerData<T>
    {
        public int Id { get; private set; }
        public Transform Tf;
        public T data;

        public ObjContainerData(int id)
        {
            Id = id;
        }
    }
}