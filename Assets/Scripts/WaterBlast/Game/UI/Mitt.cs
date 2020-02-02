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
                string title = null;
                string msg = null;
                bool isTemp = true;
                if (type == MittType.horizon)
                {
                    title = "Row Glove";
                    msg = string.Format("Removes everything in a row!");
                }
                else
                {
                    isTemp = false;
                    title = "Column Glove";
                    msg = string.Format("Removes everything in a column!");
                }
                PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/ItemPopup", "Item Popup", title, msg, "Buy");
                temp.GetComponent<PopupItem>().ItemSetting("item_arrow", 400, isTemp);

                temp.onConfirm += () =>
                {

                };
            }
        }
    }
}