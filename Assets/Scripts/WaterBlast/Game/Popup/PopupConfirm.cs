using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.Popup
{
    public class PopupConfirm : Popup
    {
        public delegate void _callback();
        public _callback onConfirm = null;
        public _callback onExit = null;

        [SerializeField] private UILabel uiTitle   = null;
        [SerializeField] private UILabel uiMessage = null;
        [SerializeField] private UILabel uiBtn = null;

        private bool IsDelayCloseConfirm;
        private bool IsDelayCloseExit;

        private const string path = "Prefabs/Popup/ConfirmPopup";

        static public PopupConfirm Open(string ID, string title, string message, string btn, bool isDelayCloseConfirm = true, bool isDelayCloseExit = true)
        {
            return Open(path, ID, title, message, btn, isDelayCloseConfirm, isDelayCloseExit);
        }

        static public PopupConfirm Open(string path, string ID, string title, string message, string btn, bool isDelayCloseConfirm = true, bool isDelayCloseExit = true)
        {
            PopupConfirm temp = Popup.Create<PopupConfirm>(path, ID);

            Manager.SoundMgr.G.GameEffectPlay(System.EffectSound.popup_open);

            if (!string.IsNullOrEmpty(title)   && temp.uiTitle   != null) { temp.uiTitle.text   = title/*DataMgr.G.GetIndexUIText(204)*/; }
            if (!string.IsNullOrEmpty(message) && temp.uiMessage != null) { temp.uiMessage.text = message/*DataMgr.G.GetIndexUIText(204)*/; }
            if (temp.uiBtn != null) { temp.uiBtn.text     = btn/*DataMgr.G.GetIndexUIText(204)*/; }

            temp.IsDelayCloseConfirm = isDelayCloseConfirm;
            temp.IsDelayCloseExit = isDelayCloseExit;

            return temp;
        }

        public void SetMessage(string message)
        {
            if (!string.IsNullOrEmpty(message) && uiMessage != null) { uiMessage.text = message/*DataMgr.G.GetIndexUIText(204)*/; }
        }

        public void OnConfirm()
        {
            Manager.SoundMgr.G.EffectPlay(System.EffectSound.btn_ok);

            if (onConfirm != null)
            {
                onConfirm();
            }

            if (IsDelayCloseConfirm) Delay(Close, 0.3f);
        }

        public void OnExit()
        {
            Manager.SoundMgr.G.EffectPlay(System.EffectSound.btn_cancel);

            if (onExit != null)
            {
                onExit();
            }

            if (IsDelayCloseExit) Delay(Close, 0.3f);
        }
    }
}