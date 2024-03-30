using System;
using System.Collections.Generic;
using System.Linq;
using HoangHH.DesignPattern;
using HoangHH.UI;
using UnityEngine;

namespace HoangHH.Manager
{
    public class UIManager : HSingleton<UIManager>
    {
        #region Handle Panel Type

        private const string SCREEN_PREFIX = "Screen";
        private const string POPUP_PREFIX = "Popup";
        private const string NOTIF_PREFIX = "Notif";
        private const string LOADING_PREFIX = "Loading";

        private enum CanvasType
        {
            None = -1,
            Screen = 0,
            Popup = 1,
            Notification = 2,
            Loading = 3
        }

        // <summary>
        // Get the type of the canvas, using the prefix of the UI type (converted to string from enum)
        // </summary>
        private static CanvasType GetCanvasType(string key)
        {
            if (key.Contains(SCREEN_PREFIX))
                return CanvasType.Screen;
            if (key.Contains(POPUP_PREFIX))
                return CanvasType.Popup;
            if (key.Contains(NOTIF_PREFIX))
                return CanvasType.Notification;
            return key.Contains(LOADING_PREFIX) ? CanvasType.Loading : CanvasType.None;
        }

        // <summary>
        // Get the root of the canvas by the type of the canvas
        // </summary>
        private Transform GetRootByType(CanvasType type)
        {
            return type switch
            {
                CanvasType.Screen => screenContainer,
                CanvasType.Popup => popupContainer,
                CanvasType.Notification => notifContainer,
                CanvasType.Loading => loadingContainer,
                _ => null
            };
        }

        #endregion

        #region Variables and Properties

        [SerializeField] private Transform screenContainer;
        [SerializeField] private Transform popupContainer;
        [SerializeField] private Transform notifContainer;
        [SerializeField] private Transform loadingContainer;

        private readonly Dictionary<UIType, UICanvas> _initialedCanvas = new();

        private readonly List<UICanvas> _showingNotifications = new();

        private readonly List<UICanvas> _showingPopups = new();

        private Stack<UICanvas> _screenStack = new();

        #endregion

        #region Loading

        public LoadingUI loadingUI;

        public void StartLoading()
        {
            loadingContainer.gameObject.SetActive(true);
        }

        public void FinishLoading()
        {
            loadingContainer.gameObject.SetActive(false);
        }

        #endregion

        #region Fuctionalities

        private readonly Dictionary<UIType, string> _cachedUIType = new();
        
        private string ToStringUIType(UIType type)
        {
            if (_cachedUIType.TryGetValue(type, out string value)) return value;
            value = type.ToString();
            _cachedUIType.Add(type, value);
            return value;
        }
        
        public T LoadCanvas<T>(UIType type) where T : UICanvas
        {
            CanvasType canvasType = GetCanvasType(ToStringUIType(type));
            if (!_initialedCanvas.TryGetValue(type, out UICanvas canvas)) canvas = AddNewCanvas(type, canvasType);
            SetCanvasToContainer(canvas, type, canvasType);
            return (T)canvas;

            void SetCanvasToContainer(UICanvas canvasI, UIType typeI, CanvasType canvasTypeI)
            {
                switch (canvasTypeI)
                {
                    case CanvasType.Screen:
                        UICanvas currentScreen = GetCurrentScreen();
                        if (currentScreen is not null && currentScreen.GetTypeUI() != typeI &&
                            currentScreen.gameObject.activeSelf)
                            currentScreen.Close();
                        if (_screenStack.Contains(canvasI))
                            _screenStack = MakeElementToTopStack(canvasI, _screenStack);
                        else
                            _screenStack.Push(canvasI);
                        break;
                    case CanvasType.Popup:
                        if (_showingPopups.Contains(canvasI))
                            _showingPopups.Remove(canvasI);
                        _showingPopups.Add(canvasI);
                        break;
                    case CanvasType.Notification:
                        if (!_showingNotifications.Contains(canvasI))
                            _showingNotifications.Add(canvasI);
                        break;
                    case CanvasType.Loading:
                    case CanvasType.None:
                    default:
                        break;
                }

                canvasI.Tf.SetAsLastSibling();
            }
        }

        private UICanvas GetCurrentScreen()
        {
            return _screenStack.Count == 0 ? null : _screenStack.Peek();
        }

        public void GoBackLastScreen(object param = null)
        {
            _screenStack.Pop().Close();
            UICanvas currentScreen = GetCurrentScreen();
            if (currentScreen is null || GetCurrentScreen().GetTypeUI() is UIType.ScreenMainMenu)
            {
                ScreenMainMenu.Show();
            }
            else
            {
                currentScreen.Setup(param);
                currentScreen.Open();
            }
        }

        public void CloseAllGUI()
        {
            foreach (KeyValuePair<UIType, UICanvas> item in _initialedCanvas.Where(item =>
                         item.Value != null && item.Value.gameObject.activeInHierarchy))
                item.Value.Close();
        }

        public void DestroyCanvas(UICanvas canvas)
            // NOTE: Dont use this method directly, it is used in UICanvas.Close() with isDestroyOnClose = true
        {
            // If is screen
            if (_screenStack.Contains(canvas))
                _screenStack = new Stack<UICanvas>(_screenStack.Where(item => item != canvas));
            else Dismiss(canvas);
            _initialedCanvas.Remove(canvas.GetTypeUI());
            Destroy(canvas.gameObject);
        }

        public void Dismiss(UICanvas panel)
        {
            _showingPopups.Remove(panel);
            _showingNotifications.Remove(panel);
        }

        public void DismissTopPopup()
        {
            UICanvas topPanel = GetTopPopup();
            if (topPanel == null)
                return;
            topPanel.Close();
            return;

            UICanvas GetTopPopup()
            {
                return _showingPopups.Count == 0 ? null : _showingPopups[^1];
            }
        }

        public void DismissUICanvasByType(UIType id)
        {
            UICanvas panel = _initialedCanvas[id];
            if (panel == null)
                return;
            panel.Close();
        }

        #endregion


        #region Add new canvas from DataManager

        private Dictionary<UIType, UICanvas> _resourceCanvas;
        private Dictionary<UIType, UICanvas> ResourceCanvas => _resourceCanvas ??= ConfigManager.Ins.CanvasInstances;

        private UICanvas AddNewCanvas(UIType uiType, CanvasType canvasType)
        {
            UICanvas newCanvas = Instantiate(GetPrefab(uiType), GetRootByType(canvasType));
            _initialedCanvas.Add(uiType, newCanvas);
            return newCanvas;
        }

        private UICanvas GetPrefab(UIType id)
        {
            return ResourceCanvas[id];
        }

        private static Stack<UICanvas> MakeElementToTopStack(UICanvas objectTop, Stack<UICanvas> stack)
        {
            UICanvas[] extraPanel = stack.ToArray();
            for (int i = 0; i < extraPanel.Length; i++)
                if (extraPanel[i] == objectTop)
                {
                    for (int ii = i; ii > 0; ii--)
                        extraPanel[ii] = extraPanel[ii - 1];

                    extraPanel[0] = objectTop;
                }

            Array.Reverse(extraPanel);
            return new Stack<UICanvas>(extraPanel);
        }

        #endregion
    }
}

/// <summary>
///     Type of the UI, must start with prefix of the UI type
///     Ex: Screen -> ScreenMainMenu, Popup -> PopupDailyReward, Notif -> NotifAds, Loading -> LoadingPanel ...
/// </summary>
public enum UIType
{
    None = -1,

    // Screen
    ScreenMainMenu,
    ScreenInGame,
    ScreenGameOver,
    ScreenWin,
    ScreenShop,

    // Popup
    PopupDailyReward,
    PopupSetting
    // Notification

    // Loading
}
