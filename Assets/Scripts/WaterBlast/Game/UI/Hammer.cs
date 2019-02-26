using WaterBlast.System;
using WaterBlast.Game.Manager;
using WaterBlast.Game.Popup;

namespace WaterBlast.Game.UI
{
    public class Hammer : Item
    {
        public void OnPressed()
        {
            if (isLock) return;
            if (!IsWhetherOrNotToUse()) return;

            int index = (int)ItemType.hammer;
            if (UserDataMgr.G.availableInGameItemCount[index] > 0)
            {
                GameDataMgr.G.isUseInGameItem[index] = !GameDataMgr.G.isUseInGameItem[index];
                bool isUseItem = GameDataMgr.G.isUseInGameItem[index];
                itemClicked.SetInfo(isUseItem, (ItemType)index);
                if(isUseItem)
                {
                    DepthSetting(6);
                }
                else
                {
                    ResetInfo();
                }
            }
            else
            {
                //아이템 샵 팝업.
                string msg = string.Format("Removes any cube or obstacle!");
                PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/ItemPopup", "Item Popup", "Hammer", msg, "Buy");
                temp.GetComponent<PopupItem>().ItemSetting("item_hammer", 200);

                temp.onConfirm += () =>
                {

                };
            }
        }
    }
}