using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;

namespace WaterBlast.Game.Manager
{
    public class SoundMgr : MonoDontDestroySingleton<SoundMgr>
    {
        public AudioSource sourceBgm = null;
        public AudioSource sourceEffect = null;
        public AudioSource sourceGameEffect = null;

        public Transform tranGame;
        private List<AudioSource> sourceGameEffects;

        [Header("BGM Clips"), Tooltip("오디오 클립들")]
        public AudioClip[] BGMClips = null;
        [Header("FX Clips"), Tooltip("오디오 클립들")]
        public AudioClip[] EffectClips = null;

        private float time;
        private float mLastTimestamp = 0f;

        private int gameEffectIndex = 0;

        protected override void OnAwake()
        {
            if (sourceGameEffects == null) sourceGameEffects = new List<AudioSource>();
            gameEffectIndex = 0;
        }

        public void RemoveGameEffect()
        {
            if (sourceGameEffects == null || sourceGameEffects.Count <= 0) return;
            foreach (var tmp in sourceGameEffects)
            {
                if (tmp == null) continue;
                Destroy(tmp.gameObject);
            }
            sourceGameEffects.Clear();
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
            if (!GameDataMgr.G.isBGM || BGMClips.Length <= 0 || BGMClips.Length <= (int)bgmSound) return;

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
            if (!GameDataMgr.G.isEffect || EffectClips.Length <= 0 || EffectClips.Length <= (int)effectSound) return;

            time = RealTime.time;
            if (sourceBgm.clip == EffectClips[(int)effectSound] && mLastTimestamp + 0.1f > time) return;
            mLastTimestamp = time;

            sourceEffect.clip = EffectClips[(int)effectSound];
            sourceEffect.loop = isLoop;
            sourceEffect.volume = volume;
            sourceEffect.Play();
        }

        public void GameEffectPlay(EffectSound effectSound, float volume = 1f, bool isLoop = false)
        {
            if (!GameDataMgr.G.isEffect || EffectClips.Length <= 0 || EffectClips.Length <= (int)effectSound) return;

            AudioSource tmpAudio = GetGameEffect();
            if (tmpAudio == null) return;

            tmpAudio.clip = EffectClips[(int)effectSound];
            tmpAudio.loop = isLoop;
            tmpAudio.volume = volume;
            tmpAudio.Play();
        }

        private AudioSource GetGameEffect()
        {
            for (int i = 0; i < sourceGameEffects.Count; ++i)
            {
                if (sourceGameEffects[i].isPlaying) continue;
                sourceGameEffects[i].clip = null;
                return sourceGameEffects[i];
            }

            AudioSource tmp = Instantiate(sourceGameEffect);
            tmp.transform.SetParent(tranGame);
            tmp.transform.localPosition = Vector3.zero;
            tmp.transform.localScale = Vector3.one;

            sourceGameEffects.Add(tmp);
            return tmp;
        }
    }
}