using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.Popup
{
    public class PopupRoot : MonoBehaviour
    {
        static PopupRoot popupRoot = null;

        static public void Add(Popup popup)
        {
            if (popupRoot == null)
            {
                popupRoot = GameObject.FindObjectOfType<PopupRoot>();
                
                if(popupRoot == null)
                {
                    popupRoot = Resources.Load<PopupRoot>("UI Root Popup");
                }
            }

            for(int i = 0; i < popupRoot.transform.childCount; ++i)
            {
                if (popupRoot.transform.GetChild(i) == null) continue;
                if (popup == popupRoot.transform.GetChild(i)) return;
            }

            popup.transform.parent = popupRoot.transform;
            popup.transform.Reset();
        }

        private void Awake()
        {
            popupRoot = this;
        }
    }
}