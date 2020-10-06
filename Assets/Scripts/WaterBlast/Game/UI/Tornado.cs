using WaterBlast.System;
using WaterBlast.Game.Manager;
using WaterBlast.Game.Popup;

namespace WaterBlast.Game.UI
{
    public class Tornado : Item
    {
        public void OnPressed()
        {
            if (isLock) return;
            if (!IsWhetherOrNotToUse()) return;

            SoundMgr.G.EffectPlay(EffectSound.btn_ok);

            int index = (int)ItemType.mix;
            if (UserDataMgr.G.availableInGameItemCount[index] > 0)
            {
                GameDataMgr.G.isUseInGameItem[index] = !GameDataMgr.G.isUseInGameItem[index];
                bool isUseItem = GameDataMgr.G.isUseInGameItem[index];
                itemClicked.SetInfo(isUseItem, (ItemType)index);
                if (isUseItem)
                {
                    DepthSetting(12);
                }
                else
                {
                    ResetInfo();
                }
            }
            else
            {
                //아이템 샵 팝업.
                int count = 3;
                if (UserDataMgr.G.IsCoins(count))
                {
                    PopupMgr.G.ShowItemPopup("Item Popup", "TORNADO", "Shuffles all of the cubes!", "BUY",
                                             "item_shuffle", count, GameDataMgr.G.itemCost, () =>
                                             {
                                                 UserDataMgr.G.CoinsUsed(GameDataMgr.G.itemCost * count);
                                                 UserDataMgr.G.availableInGameItemCount[index] += count;
                                                 UpdateInGameItemCount(index);
                                             });
                }
                else
                {
                    PopupMgr.G.ShowAdsPopup(null, "Not enough coins." + "\n" + "Watch ads and get rewarded.", "OK");
                }
            }
        }
    }
}