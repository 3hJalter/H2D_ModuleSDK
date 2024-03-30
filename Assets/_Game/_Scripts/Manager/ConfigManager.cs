using System.Collections.Generic;
using HoangHH.Config;
using HoangHH.DesignPattern;
using HoangHH.UI;
using UnityEngine;

namespace HoangHH.Manager
{
    public class ConfigManager : HSingleton<ConfigManager>
    {
        #region UI Config

        [SerializeField] private UIConfig uiConfig;

        public Dictionary<UIType, UICanvas> CanvasInstances => uiConfig.canvasInstances;

        #endregion

        #region Audio Config

        [SerializeField] private AudioConfig audioConfig;
        
        public AudioConfig AudioConfig => audioConfig;

        #endregion
    }
}
