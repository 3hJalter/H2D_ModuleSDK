using HoangHH.DesignPattern;
using HoangHH.UI;
using UnityEngine;

namespace HoangHH.Manager
{
    public enum GameState
    {
        MainMenu,
        InGame,
        Pause,
        GameOver
    }
    
    [DefaultExecutionOrder(-100)]
    public class GameManager : HDispatcher<GameManager>
    {
        #region Game Manager Awakening

        protected override void OnSingletonAwaken()
        {
            ApplyAppSetting();
            ReduceScreenResolution();
            VerifyData();
        }
        
        /// <summary>
        /// Verify the data
        /// </summary>
        private static void VerifyData()
        {
            HDebug.Log(LogType.Todo, "Make a Verify from GameData");
            HDebug.Log(LogType.Test, "Check mute Bgm " + DataManager.Ins.IsBgmMute);
            HDebug.Log(LogType.Test, "Check mute Sfx " + DataManager.Ins.IsSfxMute);
        }

        /// <summary>
        ///   Set the base app setting
        /// </summary>
        private static void ApplyAppSetting()
        {
            Input.multiTouchEnabled = false;
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
        
        /// <summary>
        ///  Reduce the screen resolution if the screen height is greater than 1280
        /// </summary>
        private static void ReduceScreenResolution()
        {
            const int maxScreenHeight = 1280;
            float ratio = Screen.currentResolution.width / (float)Screen.currentResolution.height;
            if (Screen.currentResolution.height > maxScreenHeight)
                Screen.SetResolution(Mathf.RoundToInt(ratio * maxScreenHeight), maxScreenHeight, true);
        }

        #endregion

        #region Game Manager Starting

        private void Start()
        {
            HDebug.Log(LogType.Todo, "Start the Game Manager");
            ScreenMainMenu.Show();
        }

        #endregion

        #region Regular Methods

        public GameState ChangeState(GameState newState)
        {
            return newState;
        }

        #endregion
    }
}
