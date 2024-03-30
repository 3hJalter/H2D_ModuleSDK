using System;
using Newtonsoft.Json;
using UnityEngine;

namespace HoangHH
{
    public class HGameData
    {
        public readonly SettingData setting = new();
        public readonly UserData user = new();

        [Serializable]
        public class UserData
        {
            
        }
        
        [Serializable]
        public class SettingData
        {
            public bool haptic = true;
            public bool isBgmMute;
            public bool isSfxMute;
            
            public int highPerformance = 1;
            public bool iOsTrackingRequested;
        }
    }
    
    public static class Database
    {
        private const string DATA_KEY = "GameData";

        public static void SaveData(HGameData data)
        {
            string dataString = JsonConvert.SerializeObject(data);
            PlayerPrefs.SetString(DATA_KEY, dataString);
            PlayerPrefs.Save();
        }

        public static HGameData LoadData()
        {

            if (PlayerPrefs.HasKey(DATA_KEY))
            {
                return JsonConvert.DeserializeObject<HGameData>(PlayerPrefs.GetString(DATA_KEY));
            }
            // If no game data can be loaded, create new one
            HGameData gameData = new();
            SaveData(gameData);
            return gameData;
        }
    }
}
