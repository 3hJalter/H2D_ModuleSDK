using System.Collections.Generic;
using HoangHH.Manager;
using HoangHH.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HoangHH.Config
{
    [CreateAssetMenu(fileName = "UI", menuName = "GameConfig/UI", order = 1)]
    public class UIConfig: SerializedScriptableObject
    {
        [Header("All UICanvas")]
        // ReSharper disable once CollectionNeverUpdated.Global
        public readonly Dictionary<UIType, UICanvas> canvasInstances = new();

        [FoldoutGroup("Base Canvas")] 
        [SerializeField] private UICanvas baseScreen;
        [FoldoutGroup("Base Canvas")] 
        [SerializeField] private UICanvas basePopup;
        [FoldoutGroup("Base Canvas")] 
        [SerializeField] private UICanvas baseNotif;
        
        public UICanvas GetBaseCanvas(CanvasType type)
        {
            return type switch
            {
                CanvasType.Screen => baseScreen,
                CanvasType.Popup => basePopup,
                CanvasType.Notification => baseNotif,
                _ => null
            };
        }
    }
}
