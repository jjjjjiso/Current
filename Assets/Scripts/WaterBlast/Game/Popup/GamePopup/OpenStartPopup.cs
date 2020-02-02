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
            GamePopupItemGroup item = temp.GetComponentInChildren<GamePopupItemGroup>();
            if(item != null)
            {
                item.BoosterItemSetting();
                item.gameObject.GetComponent<UIWidget>().bottomAnchor.absolute = 50;
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