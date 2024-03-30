using HoangHH.Manager;
using UnityEngine;

namespace HoangHH.UI
{
    public class UICanvas : HMonoBehaviour
    {
        public bool isDestroyOnClose;
        protected bool isInit;
        protected UIManager gui;
        
        private RectTransform _mRectTransform;
        
        protected RectTransform MRectTransform
        {
            get
            {
                _mRectTransform = _mRectTransform ? _mRectTransform : gameObject.transform as RectTransform;
                return _mRectTransform;
            }
        }

        private void Awake()
        {
            gui = UIManager.Ins;
            isInit = true;
        }

        // <summary>
        // Get the type of UI
        // </summary>
        public virtual UIType GetTypeUI()
        {
            return UIType.None;
        }
        
        // <summary>
        // Setup and do logic for the UI before open it (Set active = true), using param to pass data
        // </summary>
        public virtual void Setup(object param)
        {
            
        }

        // <summary>
        // Open and do logic for the UI
        // </summary>
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }
        
        // <summary>
        // Do logic for the UI before close it (Set active = false)
        // Destroy the UI if isDestroyOnClose = true
        // </summary>
        public virtual void Close()
        {
            if (!isInit) return;
            gameObject.SetActive(false);
            gui.Dismiss(this);
            if (!isDestroyOnClose) return;
            isInit = false;
            gui.DestroyCanvas(this);
        }
    }
}
