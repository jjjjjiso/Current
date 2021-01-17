using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class PopupInGameSetting : Popup
    {
        public delegate void _callback();
        public _callback onQuit   = null;
        public _callback onBGM    = null;
        public _callback onEffect = null;

        [SerializeField] private UIButton uiBGM    = null;
        [SerializeField] private UIButton uiEffect = null;

        private Animator anim = null;

        private const string path = "Prefabs/Popup/InGameSettingsPopup";
        private const string blue = "button_circle_blue";
        private const string gray = "button_circle_grey";

        static public PopupInGameSetting Open(string ID)
        {
            return Open(path, ID);
        }

        static public PopupInGameSetting Open(string path, string ID)
        {
            PopupInGameSetting temp = Popup.Create<PopupInGameSetting>(path, ID);

            temp.uiBGM.normalSprite    = (GameDataMgr.G.isBGM)    ? blue : gray;
            temp.uiEffect.normalSprite = (GameDataMgr.G.isEffect) ? blue : gray;

            temp.anim = temp.gameObject.GetComponent<Animator>();
            temp.anim.SetTrigger("OnPressed");

            return temp;
        }

        public void OnQuit()
        {
            SoundMgr.G.EffectPlay(System.EffectSound.btn_ok);

            if (onQuit != null)
            {
                onQuit();
            }
            
            anim.SetTrigger("OffPressed");

            Delay(Close, .2f);
        }

        public void OnExit()
        {
            SoundMgr.G.EffectPlay(System.EffectSound.btn_ok);

            anim.SetTrigger("OffPressed");

            Delay(Close, .2f);
        }

        public void OnBGM()
        {
            SoundMgr.G.EffectPlay(System.EffectSound.btn_ok);

            if (onBGM != null)
            {
                onBGM();
            }
            
            if(GameDataMgr.G.isBGM)
            {
                uiBGM.normalSprite = gray;
                GameDataMgr.G.isBGM = false;
            }
            else
            {
                uiBGM.normalSprite = blue;
                GameDataMgr.G.isBGM = true;
            }

            SoundMgr.G.SetBGM(GameDataMgr.G.isBGM);
        }

        public void OnEffect()
        {
            if (onEffect != null)
            {
                onEffect();
            }
            
            if (GameDataMgr.G.isEffect)
            {
                uiEffect.normalSprite  = gray;
                GameDataMgr.G.isEffect = false;
            }
            else
            {
                uiEffect.normalSprite  = blue;
                GameDataMgr.G.isEffect = true;
            }

            SoundMgr.G.EffectPlay(System.EffectSound.btn_ok);
        }
    }
}