using System.Collections.Generic;
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
    }
}
