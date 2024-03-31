using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HoangHH.Config;
using HoangHH.Manager;
using HoangHH.UI;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace HoangHH.Editor.EditorWindow
{
    public class UIGenerator : OdinMenuEditorWindow
    {
        #region Saved Data Path

        #region UI Config

        private string _uiConfigPath = "Assets/_Game/_Resources/UIConfig.asset";
        
        [FoldoutGroup("Saved Data Path"), PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
        [InlineButton("SelectUIConfig", "Select")]
        [SerializeField] private UIConfig uiConfig;
        
        // ReSharper disable once UnusedMember.Local
        private void SelectUIConfig()
        {
            // Open a file panel to select a UIConfig asset
            string path = EditorUtility.OpenFilePanel("Select UIConfig", "Assets", "asset");
            if (string.IsNullOrEmpty(path)) return;
            // Get the relative path of the selected asset
            string relativePath = path.Substring(Application.dataPath.Length - 6);
            // Load the asset at the relative path
            uiConfig = AssetDatabase.LoadAssetAtPath<UIConfig>(relativePath);
            // save the path of the selected asset if it is a UIConfig
            if (uiConfig == null) return;
            _uiConfigPath = relativePath;
            // Save to PlayerPref
            PlayerPrefs.SetString("UIConfigPath", _uiConfigPath);
        }

        #endregion
        
        #region Enum path
        
        [FoldoutGroup("Saved Data Path"), PropertySpace(SpaceAfter = 10)]
        [InlineButton("SelectEnumPath", "Select")]
        [SerializeField] private string uiEnumPath = "Assets/_Game/_Scripts/Manager/UIManager.cs";
        
        // ReSharper disable once UnusedMember.Local
        private void SelectEnumPath()
        {
            // Open a file panel to select a UIConfig asset
            string path = EditorUtility.OpenFilePanel("Select UIType Enum Path", "Assets", "cs");
            if (string.IsNullOrEmpty(path)) return;
            // Get the relative path of the selected asset
            string relativePath = path.Substring(Application.dataPath.Length - 6);
            // check if the selected file is a cs file
            if (!relativePath.EndsWith(".cs"))
            {
                Debug.LogError("Please select a .cs file");
                return;
            }
            // check if the selected file has line contains "public enum UIType"
            string[] lines = System.IO.File.ReadAllLines(path);
            bool hasEnum = lines.Any(line => line.Contains("public enum UIType"));
            if (!hasEnum)
            {
                Debug.LogError("Please select a file contains 'enum UIType' inside");
                return;
            }
            uiEnumPath = relativePath;
            // Save to PlayerPref
            PlayerPrefs.SetString("UIEnumPath", uiEnumPath);
        }
        
        #endregion

        #region Saved UI Script Folder
       
        [FoldoutGroup("Saved Data Path"), PropertySpace(SpaceAfter = 10)]
        [InlineButton("SelectUIScriptFolder", "Select")]
        [SerializeField] private string uiScriptFolder = "Assets/_Game/_Scripts/UI";
        
        // ReSharper disable once UnusedMember.Local
        private void SelectUIScriptFolder()
        {
            // Open a folder panel to select a folder to save the generated UI script
            string path = EditorUtility.OpenFolderPanel("Select UI Script Folder", "Assets", "");
            if (string.IsNullOrEmpty(path)) return;
            // Get the relative path of the selected folder
            string relativePath = path.Substring(Application.dataPath.Length - 6);
            uiScriptFolder = relativePath;
        }
        
        #endregion

        #region Saved UI prefab Folder
        
        [FoldoutGroup("Saved Data Path"), PropertySpace(SpaceAfter = 10)]
        [InlineButton("SelectUIPrefabFolder", "Select")]
        [SerializeField] private string uiPrefabFolder = "Assets/_Game/_Prefabs/UI";
        
        // ReSharper disable once UnusedMember.Local
        private void SelectUIPrefabFolder()
        {
            // Open a folder panel to select a folder to save the generated UI prefab
            string path = EditorUtility.OpenFolderPanel("Select UI Prefab Folder", "Assets", "");
            if (string.IsNullOrEmpty(path)) return;
            // Get the relative path of the selected folder
            string relativePath = path.Substring(Application.dataPath.Length - 6);
            uiPrefabFolder = relativePath;
        }

        #endregion
        
        #endregion
        
        [Space]
        
        [SerializeField] private CanvasType canvasType;
        [SerializeField] private string uiName;
        
        private const string SCREEN_PREFIX = "Screen";
        private const string POPUP_PREFIX = "Popup";
        private const string NOTIF_PREFIX = "Notif";
        
        private readonly Dictionary<CanvasType, string> _canvasTypePrefix = new()
        {
            {CanvasType.Screen, SCREEN_PREFIX},
            {CanvasType.Popup, POPUP_PREFIX},
            {CanvasType.Notification, NOTIF_PREFIX}
        };

        [Button]
        private void CheckExisted()
        {
            uiName = char.ToUpper(uiName[0]) + uiName[1..];
            string scriptName = _canvasTypePrefix[canvasType] + uiName;
            
            Assembly assembly = Assembly.Load("Assembly-CSharp");
            bool hasType = assembly.GetType("HoangHH.UI." + scriptName) != null;
            
            Debug.Log($"Has {scriptName}: {hasType}");
            bool hasEnum = Enum.TryParse(typeof(UIType), scriptName, true, out _);
            Debug.Log($"Has {scriptName} in enum: {hasEnum}");
        }
        
        [Button]
        private void Generate()
        {
            if (!IsSavedDataPathValid()) return;
            // Get the base UICanvas from UIConfig based on type
            UICanvas baseCanvas = uiConfig.GetBaseCanvas(canvasType);
            if (baseCanvas == null)
            {
                Debug.LogError($"Base Canvas of {canvasType} is null");
                return;
            }
            // Create a new cs Script with name is _canvasTypePrefix+ uiName,
            // make sure the first letter of uiName is uppercase
            uiName = char.ToUpper(uiName[0]) + uiName[1..];
            string scriptName = _canvasTypePrefix[canvasType] + uiName;
            // Check if already has this enum that same with name
            if (!IsCanvasAlreadyExisted(scriptName)) return;
            Debug.Log($"Generate UI: {canvasType} - {uiName}");
            UIGeneratorCompiled.OpenWindow(
                uiPrefabFolder, uiScriptFolder, uiEnumPath, uiConfig, canvasType,
                scriptName, baseCanvas);
        }

        

        protected override OdinMenuTree BuildMenuTree()
        {
            // check if PlayerPref has the path of the selected UIConfig
            if (PlayerPrefs.HasKey("UIConfigPath"))
            {
                _uiConfigPath = PlayerPrefs.GetString("UIConfigPath");
                uiConfig = AssetDatabase.LoadAssetAtPath<UIConfig>(_uiConfigPath);
            }
            
            // check if PlayerPref has the path of the selected Enum
            if (PlayerPrefs.HasKey("UIEnumPath"))
            {
                uiEnumPath = PlayerPrefs.GetString("UIEnumPath");
            }
            
            OdinMenuTree tree = new(supportsMultiSelect: true)
            {
                { "Home", this, EditorIcons.House }, // Draws the this.someData field in this case.
                { "Player Settings", Resources.FindObjectsOfTypeAll<PlayerSettings>().FirstOrDefault(), SdfIconType.GearFill },
            };
            
            return tree;
        }
        
        #region Validate

        private bool IsSavedDataPathValid()
        {
            if (uiConfig == null)
            {
                Debug.LogError("Please select a UIConfig asset");
                return false;
            }

            if (!IsEnumPathValid()) return false;

            return true;
        }
        
        private bool IsEnumPathValid()
        {
            if (string.IsNullOrEmpty(uiEnumPath))
            {
                Debug.LogError("Please select a UIType enum path");
                return false;
            }
            if (!uiEnumPath.EndsWith(".cs"))
            {
                Debug.LogError("Please select a .cs file");
                return false;
            }
            string[] lines = System.IO.File.ReadAllLines(uiEnumPath);
            bool hasEnum = lines.Any(line => line.Contains("public enum UIType"));
            if (hasEnum) return true;
            Debug.LogError("Please select a file contains 'enum UIType' inside");
            return false;
        }

        private bool IsCanvasAlreadyExisted(string scriptName)
        {
            // Check if already has this enum that same with name
            string[] lines = System.IO.File.ReadAllLines(uiEnumPath);
            // read the line from last
            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (lines[i].Contains(scriptName))
                {
                    Debug.LogError($"This class {scriptName} is existed. Please choose other name.");
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
    
    public class UIGeneratorCompiled : UnityEditor.EditorWindow
    {
        #region Playpref Data Save
        
        private static string ScriptName
        {
            get => PlayerPrefs.GetString("UIGeneratorScriptName");
            set => PlayerPrefs.SetString("UIGeneratorScriptName", value);
        }
        
        private static string UIPrefabFolder
        {
            get => PlayerPrefs.GetString("UIGeneratorUIPrefabFolder");
            set => PlayerPrefs.SetString("UIGeneratorUIPrefabFolder", value);
        }
        
        private static string UIScriptFolder
        {
            get => PlayerPrefs.GetString("UIGeneratorUIScriptFolder");
            set => PlayerPrefs.SetString("UIGeneratorUIScriptFolder", value);
        }
        
        private static string UIEnumPath
        {
            get => PlayerPrefs.GetString("UIGeneratorUIEnumPath");
            set => PlayerPrefs.SetString("UIGeneratorUIEnumPath", value);
        }
        
        private static CanvasType CanvasType
        {
            get => (CanvasType) PlayerPrefs.GetInt("UIGeneratorCanvasType");
            set => PlayerPrefs.SetInt("UIGeneratorCanvasType", (int) value);
        }
        
        private static UIConfig UIConfig
        {
            get => AssetDatabase.LoadAssetAtPath<UIConfig>(PlayerPrefs.GetString("UIGeneratorUIConfigPath"));
            set => PlayerPrefs.SetString("UIGeneratorUIConfigPath", AssetDatabase.GetAssetPath(value));
        }
        
        #endregion
        
        private const string SCREEN_PREFIX = "Screen";
        private const string POPUP_PREFIX = "Popup";
        private const string NOTIF_PREFIX = "Notif";
        
        private static readonly Dictionary<CanvasType, string> CanvasTypePrefix = new()
        {
            {CanvasType.Screen, SCREEN_PREFIX},
            {CanvasType.Popup, POPUP_PREFIX},
            {CanvasType.Notification, NOTIF_PREFIX}
        };
        
        // KEY
        private static bool IsPrepared
        {
            get => PlayerPrefs.GetInt("UIGeneratorIsPrepared") == 1;
            set => PlayerPrefs.SetInt("UIGeneratorIsPrepared", value ? 1 : 0);
        }
        
        private static bool IsStartedCreatePrefab
        {
            get => PlayerPrefs.GetInt("UIGeneratorIsStartedCreatePrefab") == 1;
            set => PlayerPrefs.SetInt("UIGeneratorIsStartedCreatePrefab", value ? 1 : 0);
        }
        
        private static bool IsWaitingForCreateScript
        {
            get => PlayerPrefs.GetInt("UIGeneratorIsWaitingForCreateScript") == 1;
            set => PlayerPrefs.SetInt("UIGeneratorIsWaitingForCreateScript", value ? 1 : 0);
        }
        
        private static bool IsDone
        {
            get => PlayerPrefs.GetInt("UIGeneratorIsDone") == 1;
            set => PlayerPrefs.SetInt("UIGeneratorIsDone", value ? 1 : 0);
        }

        private static bool IsClose
        {
            get => PlayerPrefs.GetInt("UIGeneratorIsClose") == 1;
            set => PlayerPrefs.SetInt("UIGeneratorIsClose", value ? 1 : 0);
        }
        
        public static void OpenWindow(
            string uiPrefabFolder, string uiScriptFolder, string uiEnumPath, UIConfig uiConfig, CanvasType canvasType,
            string scriptName, UICanvas baseCanvas)
        {
            ScriptName = scriptName;
            UIPrefabFolder = uiPrefabFolder;
            UIScriptFolder = uiScriptFolder;
            UIEnumPath = uiEnumPath;
            UIConfig = uiConfig;
            CanvasType = canvasType;
            
            IsPrepared = false;
            IsStartedCreatePrefab = false;
            IsWaitingForCreateScript = true;
            IsDone = false;
            
            Debug.Log("Open UIGeneratorCompiled");
            GetWindow<UIGeneratorCompiled>().Show();
        }

        private void OnDestroy()
        {
            // Remove all the player prefs that used in this window
            PlayerPrefs.DeleteKey("UIGeneratorScriptName");
            PlayerPrefs.DeleteKey("UIGeneratorUIPrefabFolder");
            PlayerPrefs.DeleteKey("UIGeneratorUIScriptFolder");
            PlayerPrefs.DeleteKey("UIGeneratorUIEnumPath");
            PlayerPrefs.DeleteKey("UIGeneratorCanvasType");
            PlayerPrefs.DeleteKey("UIGeneratorUIConfigPath");
            PlayerPrefs.DeleteKey("UIGeneratorIsPrepared");
            PlayerPrefs.DeleteKey("UIGeneratorIsStartedCreatePrefab");
            PlayerPrefs.DeleteKey("UIGeneratorIsWaitingForCreateScript");
            PlayerPrefs.DeleteKey("UIGeneratorIsDone");
            PlayerPrefs.DeleteKey("UIGeneratorIsClose");
        }

        private void OnGUI()
        {
            if (!IsPrepared)
            {
                GenerateEnum(ScriptName);
                GenerateScriptFolder(CanvasType);
                GenerateScript(ScriptName);
                IsPrepared = true;
                Repaint();
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            }
            else
            {
                if (!IsStartedCreatePrefab)
                {
                    EditorGUILayout.LabelField("Creating Script...");
                }

                if (IsWaitingForCreateScript)
                {
                    // TODO: _scriptName is null here
                    bool hasEnum = Enum.TryParse(typeof(UIType), ScriptName, true, out _);
                    bool hasType = Assembly.Load("Assembly-CSharp").GetType("HoangHH.UI." + ScriptName) != null;
                    if (hasEnum && hasType)
                    {
                        IsWaitingForCreateScript = false;
                        Repaint();
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    }
                }
                else
                {
                    if (!IsStartedCreatePrefab)
                    {
                        GeneratePrefabFolder(CanvasType);
                        IsStartedCreatePrefab = true;
                        Repaint();
                        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                    }
                    else switch (IsDone)
                    {
                        case false:
                            GeneratePrefab(ScriptName, UIConfig.GetBaseCanvas(CanvasType));
                            IsDone = true;
                            Repaint();
                            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                            break;
                        case true when !IsClose:
                        {
                            IsClose = true;
                            if (EditorUtility.DisplayDialog("Notification", "Canvas created successfully!", "Nice!"))
                            {
                                AddPrefabToConfig();
                                Repaint();
                                Close();
                            }
                            break;
                        }
                    }
                }
            }
        }
        
         #region Generator

        #region Psuedo Code
        private const string ScriptTemplate = @"
using HoangHH.Manager;

namespace HoangHH.UI
{{
    public class {0} : UICanvas
    {{
        private const UIType UI_TYPE = UIType.{0};
        public static {0} Instance {{ get; private set; }}

        public static {0} Show(object param = null)
        {{
            {0} ins = UIManager.Ins.LoadCanvas<{0}>(UI_TYPE);
            Instance = ins;
            ins.Setup(param);
            ins.Open();
            return ins;
        }}

        public override UIType GetTypeUI()
        {{
            return UI_TYPE;
        }}

        public override void Setup(object param)
        {{
            base.Setup(param);
        }}

        public override void Open()
        {{
            base.Open();
        }}

        public override void Close()
        {{
            base.Close();
        }}
    }}
}}
";
        

        #endregion
        
        private static void GenerateEnum(string scriptName)
        {
            // find the line SdkPlaceHolder to insert the new enum
            string[] lines = System.IO.File.ReadAllLines(UIEnumPath);
            int sdkPlaceHolderIndex = Array.FindIndex(lines, line => line.Contains("SdkPlaceHolder"));

            if (sdkPlaceHolderIndex != -1)
            {
                List<string> newLines = new(lines);
                newLines.Insert(sdkPlaceHolderIndex, $"\t{scriptName},");
                System.IO.File.WriteAllLines(UIEnumPath, newLines.ToArray());
                return;
            }
            Debug.LogError("SdkPlaceHolder not found in the enum file.");
        }
        
        private static void GenerateScript(string scriptName)
        {
            string scriptPath = $"{UIScriptFolder}/{CanvasTypePrefix[CanvasType]}/{scriptName}.cs";

            string scriptContent = string.Format(ScriptTemplate, scriptName);

            // Write the script to the file
            System.IO.File.WriteAllText(scriptPath, scriptContent);
        }

        private static void GeneratePrefab(string scriptName, UICanvas baseCanvas)
        {
            // Create a new prefab with the name is _canvasTypePrefix + uiName
            GameObject prefab = PrefabUtility.InstantiatePrefab(baseCanvas.gameObject) as GameObject;
            if (prefab == null)
            {
                Debug.LogError("Cannot instantiate the base canvas");
                return;
            }
            // unpack the prefab
            PrefabUtility.UnpackPrefabInstance(prefab, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            // remove UICanvas Component 
            DestroyImmediate(prefab.GetComponent<UICanvas>());
            // Add the new script to the prefab
            prefab.AddComponent(Assembly.Load("Assembly-CSharp").GetType("HoangHH.UI." + ScriptName));
            // Change the name of the prefab
            prefab.name = scriptName;
            // Save the prefab to the prefab folder
            string prefabPath = $"{UIPrefabFolder}/{CanvasTypePrefix[CanvasType]}/{scriptName}.prefab";
            PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            // Destroy the instantiated prefab
            DestroyImmediate(prefab);
        }
        
        private static void AddPrefabToConfig()
        {
            string path = $"{UIPrefabFolder}/{CanvasTypePrefix[CanvasType]}/{ScriptName}.prefab";
            UICanvas prefab = AssetDatabase.LoadAssetAtPath<UICanvas>(path);
            // Add prefab to the UIConfig
            UIConfig.canvasInstances.Add((UIType) Enum.Parse(typeof(UIType), ScriptName), prefab.GetComponent<UICanvas>());
        }
        
        
        private static void GeneratePrefabFolder(CanvasType type)
        {
            if (!System.IO.Directory.Exists($"{UIPrefabFolder}/{CanvasTypePrefix[type]}"))
            {
                System.IO.Directory.CreateDirectory($"{UIPrefabFolder}/{CanvasTypePrefix[type]}");
            }
        }
        
        private static void GenerateScriptFolder(CanvasType type)
        {
            if (!System.IO.Directory.Exists($"{UIScriptFolder}/{CanvasTypePrefix[type]}"))
            {
                System.IO.Directory.CreateDirectory($"{UIScriptFolder}/{CanvasTypePrefix[type]}");
            }
        }
        
        #endregion
    }
}
