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
                string msg = string.Format("Shuffles all of the cubes!");
                PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/ItemPopup", "Item Popup", "TORNADO", msg, "BUY");
                temp.GetComponent<PopupItem>().ItemSetting("item_shuffle", 100);

                temp.onConfirm += () =>
                {

                };
            }
        }
    }
}