using WaterBlast.System;
using WaterBlast.Game.Manager;
using WaterBlast.Game.Popup;

namespace WaterBlast.Game.UI
{
    public enum MittType
    {
        horizon,
        vertical,
    }

    public class Mitt : Item
    {
        public MittType type = MittType.horizon;

        public void OnPressed()
        {
            if (isLock) return;
            if (!IsWhetherOrNotToUse()) return;

            SoundMgr.G.EffectPlay(EffectSound.btn_ok);

            int index = (type == MittType.horizon) ? (int)ItemType.horizon : (int)ItemType.vertical;
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
                //샵 팝업.
                int count = 3;
                if (UserDataMgr.G.IsItemCoins(count))
                {
                    string title = null;
                    string msg = null;
                    bool isTemp = true;
                    if (type == MittType.horizon)
                    {
                        title = "ROW GLOVE";
                        msg = "Removes everything in a row!";
                    }
                    else
                    {
                        isTemp = false;
                        title = "COLUMN GLOVE";
                        msg = "Removes everything in a column!";
                    }
                    
                    PopupMgr.G.ShowItemPopup("Item Popup", title, msg, "BUY", "item_arrow", count, GameDataMgr.G.itemCost, 
                        () =>
                        {
                            UserDataMgr.G.CoinsUsed(GameDataMgr.G.itemCost * count);
                            UserDataMgr.G.availableInGameItemCount[index] += count;
                            UpdateInGameItemCount(index);
                        }, false, isTemp);
                }
                else
                {
                    PopupMgr.G.ShowAdsPopup(null, "Not enough coins." + "\n" + "Watch ads and get rewarded.", "OK");
                }
            }
        }
    }
}