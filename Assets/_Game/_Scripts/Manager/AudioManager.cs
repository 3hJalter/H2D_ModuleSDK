using System;
using System.Collections.Generic;
using AudioEnum;
using DG.Tweening;
using HoangHH.Config;
using HoangHH.DesignPattern;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HoangHH.Manager
{
    [Serializable]
    public struct AudioSourceData
    {
        [SerializeField]
        private AudioSource audioSource;
        [HideInInspector]
        public ObjectContainer<AudioSource> audioSourcePool;
        [ReadOnly]
        public Audio currentAudio;
        [ReadOnly]
        [SerializeField] private float baseVolume;

        private List<AudioSource> audioSources;
        public bool IsPlaying => audioSource.isPlaying;

        public GameObject GameObject => audioSource.gameObject;
        
        public void SetLoop(bool loop)
        {
            audioSource.loop = loop;
        }
        
        public void SetAudio(Audio audio)
        {
            if (audio is null) return;
            if (audioSourcePool && audioSource.isPlaying)
            {
                AudioSource newAudio = audioSourcePool.Pop(0).data;
                audioSourcePool.Push(0, audioSource.transform);
                audioSource = newAudio;
            }

            currentAudio = audio;
            audioSource!.clip = audio.clip;
            Volume = audio.multiplier;
        }
        
        public void Play()
        {
            audioSource.Play();
        }
        
        public void Pause()
        {
            audioSource.Pause();
        }
        
        public void Stop()
        {
            audioSource.Stop();
        }
        
        public float BaseVolume
        {
            get => baseVolume;
            set
            {
                baseVolume = value;
                Volume = currentAudio?.multiplier ?? 1;
            }
        }

        public float Volume
        {
            get => audioSource.volume;
            set => audioSource.volume = baseVolume * value;
        }

        public bool IsMute => audioSource.volume < 0.01 || audioSource.mute;
        
    }
    
    public class AudioManager : HSingleton<AudioManager>
    {
        private AudioConfig audioConfig;
        [SerializeField] private AudioContainer audioSourcePool;

        [SerializeField] private AudioSourceData bgm;
        [SerializeField] private AudioSourceData sfx;

        private void Awake()
        {
            audioSourcePool.OnInit(true);
            sfx.audioSourcePool = audioSourcePool;
            
            bgm.BaseVolume = DataManager.Ins.IsBgmMute ? 0 : 1;
            sfx.BaseVolume = DataManager.Ins.IsSfxMute ? 0 : 1;
            
            audioConfig = ConfigManager.Ins.AudioConfig;
            
        }

        public float BgmVolume => bgm.BaseVolume;
        public float SfxVolume => sfx.BaseVolume;
        
        // <summary>
        // Get a background music from AudioData
        // </summary>
        // <param name="type">Type of BGM</param>
        // <returns>Audio</returns>
        private Audio GetBgmAudio(BgmType type)
        {
            return GetAudio(audioConfig.BgmAudioDict, type);
        }

        // <summary>
        // Get a sound effect from AudioData
        // </summary>
        // <param name="type">Type of SFX</param>
        // <returns>Audio</returns>
        private Audio GetSfxAudio(SfxType type)
        {
            return GetAudio(audioConfig.SfxAudioDict, type);
        }

        // <summary>
        // Get an audio from a certain dictionary
        // </summary>
        // <param name="audioDictionary">Dictionary of audio</param>
        // <param name="type">Type of audio</param>
        // <returns>Audio</returns>
        private static Audio GetAudio<T>(IReadOnlyDictionary<T, Audio> audioDictionary, T type)
        {
            return audioDictionary.GetValueOrDefault(type);
        }

        // <summary>
        // Play a background music
        // </summary>
        // <param name="type">Type of BGM</param>
        // <param name="fadeFloat">Fade out time</param>
        public void PlayBgm(BgmType type, float fadeOut = 0.3f)
        {
            Audio audioIn = GetBgmAudio(type);
            if (audioIn is null) return;
            if (audioIn == bgm.currentAudio && bgm.IsPlaying) return;
            bgm.SetLoop(true);
            if (fadeOut == 0f || bgm.IsMute)
            {
                bgm.SetAudio(audioIn);
                bgm.Play();
            } else
            {
                DOVirtual.Float(bgm.currentAudio.multiplier, 0, fadeOut, value => bgm.Volume = value)
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        bgm.SetAudio(audioIn);
                        bgm.Play();
                    });
            }
        }
        
        // <summary>
        // Play a sound effect
        // </summary>
        // <param name="type">Type of SFX</param>
        public Audio PlaySfx(SfxType type)
        {
            Audio audioIn = GetSfxAudio(type);
            if (audioIn is null) return null;
            sfx.SetAudio(audioIn);
            sfx.Play();
            return audioIn;
        }
        
        // <summary>
        // Play a random sound effect from a list
        // </summary>
        // <param name="sfxTypes">List of SFX</param>
        public void PlayRandomSfx(List<SfxType> sfxTypes)
        {
            PlaySfx(sfxTypes[Random.Range(0, sfxTypes.Count)]);
        }
        
        public void PauseBgm()
        {
            bgm.Pause();
        }

        public void UnPauseBgm()
        {
            bgm.Play();
        }
        
        public void StopBgm()
        {
            bgm.Stop();
        }
        
        public void StopSfx(SfxType type = SfxType.None)
        {
            if (type == SfxType.None)
            {
                sfx.Stop();
                return;
            }
            if (sfx.currentAudio != GetSfxAudio(type)) return;
            sfx.Stop();
        }

        public bool IsBgmMute()
        {
            return bgm.IsMute;
        }
        
        public bool IsSfxMute()
        {
            return sfx.IsMute;
        }

        public void ToggleBgmVolume(bool isMute)
        {
            bgm.BaseVolume = isMute ? 0 : 1;
            DataManager.Ins.IsBgmMute = isMute;
        }
        
        public void ToggleSfxVolume(bool isMute)
        {
            sfx.BaseVolume = isMute ? 0 : 1;
            DataManager.Ins.IsSfxMute = isMute;
        }
    }
}
