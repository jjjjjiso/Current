using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.Popup
{
    public class PopupItem : Popup
    {
        public delegate void OnClick();
        public OnClick onClick;

        static public Popup Open(string path, string id)
        {
            PopupItem temp = Popup.Create<PopupItem>(path, id);

            return temp;
        }

        public void Click()
        {
            if(onClick != null)
            {
                onClick();
            }

            Close();
        }
    }
}