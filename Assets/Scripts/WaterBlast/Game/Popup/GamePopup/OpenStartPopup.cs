using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class OpenStartPopup : MonoBehaviour
    {
        public void OnPressed()
        {
            if (UserDataMgr.G.life <= 0)
            {
                int count = 1;
                if (UserDataMgr.G.IsCoins(count))
                {
                    PopupMgr.G.ShowItemPopup("Life Item Popup", "LIFE", "You can play the game with life!", "BUY", "life_icon",
                                             count, GameDataMgr.G.itemCost, () =>
                                             {
                                                 UserDataMgr.G.CoinsUsed(GameDataMgr.G.itemCost * count);
                                                 UserDataMgr.G.life += count;
                                                 UserDataMgr.G.SetTimeToNextLife();
                                                 if (LobbyMgr.G != null && LobbyMgr.G.levelNumber != null)
                                                 {
                                                     LobbyMgr.G.levelNumber.UpdateLifeLabel();
                                                     LobbyMgr.G.levelNumber.UpdateCoin();
                                                 }
                                             });
                }
                else
                {
                    PopupMgr.G.ShowAdsPopup(null, "Not enough coins and life." + "\n" + "Watch ads and get rewarded.", "OK");
                }
                return;
            }

            string level = string.Format("LEVEL {0}", GameDataMgr.G.endLevel.ToString());
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/GamePopup", "StartPopup", level, null, "PLAY");

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