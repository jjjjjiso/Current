using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class OpenStartPopup : MonoBehaviour
    {
        public void OnPressed()
        {
            string level = string.Format("Level {0}", GameDataMgr.G.endLevel.ToString());
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/GamePopup", "StartPopup", level, null, "Play");

            temp.GetComponent<GamePopup>().OnPopup(GamePopupState.start);
            GamePopupItemCount item = temp.GetComponentInChildren<GamePopupItemCount>();
            if(item != null)
            {
                item.SetItemCount(UserDataMgr.G.availableStartItemCount);
                item.gameObject.GetComponent<UIWidget>().topAnchor.absolute = -65;
            }

            temp.onConfirm += () =>
            {
                SceneFadeInOut fade = gameObject.GetComponentInParent<SceneFadeInOut>();
                fade.delayTime = 0.15f;
                fade.fadeTime = 0.3f;
                fade.OnPressed();
            };
        }
    }
}