using WaterBlast.System;
using WaterBlast.Game.Manager;

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
            if (!IsWhetherOrNotToUse()) return;

            int index = (type == MittType.horizon) ? (int)ItemType.horizon : (int)ItemType.vertical;
            if (UserDataMgr.G.availableInGameItemCount[index] > 0)
            {
                GameDataMgr.G.isUseItem[index] = !GameDataMgr.G.isUseItem[index];
                bool isUseItem = GameDataMgr.G.isUseItem[index];
                itemClicked.SetInfo(isUseItem, (ItemType)index);
                if (isUseItem)
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
                //샵 팝업.
            }
        }
    }
}