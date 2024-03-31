using HoangHH.Editor.EditorWindow;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEngine;

#if UNITY_EDITOR

namespace HoangHH.Editor
{
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    
    public class HoangSDK : OdinEditorWindow
    {
        [MenuItem("Tools/HoangSDK/UIGenerator")]
        private static void OpenWindow()
        {
            UIGenerator window = GetWindow<UIGenerator>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 400);
        }
        
        [MenuItem("Tools/HoangSDK/ClearGameData")]
        private static void ClearGameData()
        {
            PlayerPrefs.DeleteKey("GameData");
            Debug.Log("Delete GameData Complete");
        }
    }
}

#endif




