using HoangHH.DesignPattern;

namespace HoangHH.Manager
{
    public class DataManager : HNonMonoSingleton<DataManager>
    {
        private HGameData _gameData;
        
        private HGameData GameData => _gameData ??= Database.LoadData();

        public void Save()
        {
            Database.SaveData(_gameData);
        }
        
        #region Sound

        public bool IsBgmMute
        {
            get => GameData.setting.isBgmMute;
            set => GameData.setting.isBgmMute = value;
        }
        
        public bool IsSfxMute
        {
            get => GameData.setting.isSfxMute;
            set => GameData.setting.isSfxMute = value;
        }

        #endregion
    }
}
