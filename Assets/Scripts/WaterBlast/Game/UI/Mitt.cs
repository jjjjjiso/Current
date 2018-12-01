using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;
using WaterBlast.Game.Common;

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
            if (GameDataMgr.Get().availableItemCount[index] > 0)
            {
                GameDataMgr.Get().isUseItem[index] = !GameDataMgr.Get().isUseItem[index];
                bool isUseItem = GameDataMgr.Get().isUseItem[index];
                itemClicked.SetInfo(isUseItem, (ItemType)index);
                itemUI.GetComponent<UISprite>().depth = (isUseItem == true) ? 6 : 2;
            }
            else
            {
                //샵 팝업.
            }
        }
    }
}