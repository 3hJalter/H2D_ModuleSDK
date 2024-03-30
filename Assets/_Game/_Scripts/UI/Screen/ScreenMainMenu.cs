using HoangHH.Manager;

namespace HoangHH.UI
{
    public class ScreenMainMenu : UICanvas
    {
        private const UIType UI_TYPE = UIType.ScreenMainMenu;
        public static ScreenMainMenu Instance { get; private set; }
        
        public static ScreenMainMenu Show(object param = null)
        {
            ScreenMainMenu ins = UIManager.Ins.LoadCanvas<ScreenMainMenu>(UI_TYPE);
            Instance = ins;
            ins.Setup(param);
            ins.Open();
            return ins;
        }

        public override UIType GetTypeUI()
        {
            return UI_TYPE;
        }
        
        // TEST
        public void OpenInGame()
        {
            ScreenInGame.Show();
        }
        
        public override void Setup(object param)
        {
            base.Setup(param);
        }

        public override void Open()
        {
            base.Open();
        }

        public override void Close()
        {
            base.Close();
        }
    }
}
