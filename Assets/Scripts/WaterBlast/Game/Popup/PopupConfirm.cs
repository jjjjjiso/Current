using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.Popup
{
    public class PopupConfirm : Popup
    {
        public delegate void _callback();
        public _callback onConfirm = null;
        public _callback onEixt = null;

        [SerializeField] private UILabel uiTitle   = null;
        [SerializeField] private UILabel uiMessage = null;
        [SerializeField] private UILabel uiBtn = null;

        private const string path = "Prefabs/Popup/ConfirmPopup";

        static public PopupConfirm Open(string ID, string title, string message, string btn)
        {
            return Open(path, ID, title, message, btn);
        }

        static public PopupConfirm Open(string path, string ID, string title, string message, string btn)
        {
            PopupConfirm temp = Popup.Create<PopupConfirm>(path, ID);

            if (title   != null && temp.uiTitle   != null) { temp.uiTitle.text   = title/*DataMgr.G.GetIndexUIText(204)*/; }
            if (message != null && temp.uiMessage != null) { temp.uiMessage.text = message/*DataMgr.G.GetIndexUIText(204)*/; }
            if (temp.uiBtn     != null) { temp.uiBtn.text     = btn/*DataMgr.G.GetIndexUIText(204)*/; }

            return temp;
        }

        public void OnConfirm()
        {
            if (onConfirm != null)
            {
                onConfirm();
            }

            Delay(Close, 0.3f);
            //Close();
        }

        public void OnExit()
        {
            if (onEixt != null)
            {
                onEixt();
            }

            Delay(Close, 0.3f);
        }
    }
}