using System.Collections;
using UnityEngine;

using WaterBlast.System;

namespace WaterBlast.Game.Manager
{
    public class SoundMgr : MonoDontDestroySingleton<SoundMgr>
    {
        public AudioSource sourceBgm = null;
        public AudioSource sourceEffect = null;
        
        [Header("BGM Clips"), Tooltip("오디오 클립들")]
        public AudioClip[] BGMClips = null;
        [Header("FX Clips"), Tooltip("오디오 클립들")]
        public AudioClip[] EffectClips = null;

        private float time;
        private float mLastTimestamp = 0f;

        protected override void OnAwake()
        {
            if (BGMClips.Length > 0) BGMPlay(BGMClips[(int)BGMSound.lobby], 0.3f);
        }

        public void SetBGM(bool isValue)
        {
            if (isValue) BGMStop();
            else BGMPlay();
        }

        public void BGMPlay(AudioClip changeClip, float volume)
        {
            if (!GameDataMgr.G.isBGM) return;

            sourceBgm.clip = changeClip;
            sourceBgm.volume = volume;
            sourceBgm.Play();
        }

        public void BGMChangePlay(BGMSound bgmSound, float volume = 1f, float changeSpeed = 1f, float waitTime = 1f, float maxTime = 1f, bool isSmooth = false)
        {
            if (!GameDataMgr.G.isBGM || BGMClips.Length <= 0) return;

            time = RealTime.time;
            if (sourceBgm.clip == BGMClips[(int)bgmSound] && mLastTimestamp + 0.1f > time) return;
            mLastTimestamp = time;

            AudioClip changeClip = BGMClips[(int)bgmSound];

            if (!isSmooth)
            {
                BGMPlay(changeClip, volume);
                return;
            }

            StartCoroutine(Co_BGMChangePlay(changeClip, volume, changeSpeed, waitTime, maxTime));
        }

        IEnumerator Co_BGMChangePlay(AudioClip changeClip, float volume, float changeSpeed, float waitTime, float maxTime)
        {
            float startTime = Time.time;
            float progress = 0f;
            while (progress < maxTime)
            {
                progress = (Time.time - startTime) * changeSpeed;
                sourceBgm.volume = Mathf.Lerp(sourceBgm.volume, 0, progress);
                yield return new WaitForSeconds(waitTime);
            }

            BGMPlay(changeClip, volume);
        }

        public void BGMPlay()
        {
            if (sourceBgm.clip == null) return;
            sourceBgm.Play();
        }

        public void BGMStop()
        {
            sourceBgm.Stop();
        }

        public void EffectPlay(EffectSound effectSound, float volume = 1f, bool isLoop = false)
        {
            if (!GameDataMgr.G.isEffect || EffectClips.Length <= 0) return;

            time = RealTime.time;
            if (sourceBgm.clip == EffectClips[(int)effectSound] && mLastTimestamp + 0.1f > time) return;
            mLastTimestamp = time;

            sourceEffect.clip = EffectClips[(int)effectSound];
            sourceEffect.loop = isLoop;
            sourceEffect.volume = volume;
            sourceEffect.Play();
        }
    }
}