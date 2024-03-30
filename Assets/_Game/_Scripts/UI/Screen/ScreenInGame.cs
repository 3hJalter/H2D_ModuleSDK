using HoangHH.Manager;

namespace HoangHH.UI
{
    public class ScreenInGame : UICanvas
    {
        private const UIType UI_TYPE = UIType.ScreenInGame;
        
        public static ScreenInGame Instance { get; private set; }
        
        public static ScreenInGame Show(object param = null)
        {
            ScreenInGame ins = UIManager.Ins.LoadCanvas<ScreenInGame>(UI_TYPE);
            Instance = ins;
            ins.Setup(param);
            ins.Open();
            return ins;
        }
        
        public override UIType GetTypeUI()
        {
            return UI_TYPE;
        }
        
        // TEST: Open Main Menu
        public void OpenMainMenu()
        {
            ScreenMainMenu.Show();
        }
    }
}
