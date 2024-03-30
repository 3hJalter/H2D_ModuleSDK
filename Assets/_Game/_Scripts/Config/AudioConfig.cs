using System;
using System.Collections.Generic;
using AudioEnum;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HoangHH.Config
{
    [CreateAssetMenu(fileName = "Audio", menuName = "GameConfig/Audio", order = 1)]
    public class AudioConfig : SerializedScriptableObject
    {
        [Title("BGM")]
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<BgmType, Audio> _bgmAudioDict = new();
        
        [Title("SFX")]
        // ReSharper disable once Unity.RedundantSerializeFieldAttribute
        [SerializeField] private readonly Dictionary<SfxType, Audio> _sfxAudioDict = new();
        
        public Dictionary<BgmType, Audio> BgmAudioDict => _bgmAudioDict;
        
        public Dictionary<SfxType, Audio> SfxAudioDict => _sfxAudioDict;
    }

    [Serializable]
    public class Audio
    {
        public AudioClip clip;
        [Range(0,1)]
        public float multiplier = 1;
    }
}

namespace AudioEnum
{
    public enum SfxType
    {
        None = -1,
        
    }

    public enum BgmType
    {
        None = -1,
        
    }
}
