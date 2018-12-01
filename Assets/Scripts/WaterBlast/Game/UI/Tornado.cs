using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;
using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class Tornado : Item
    {
        public void OnPressed()
        {
            if (!IsWhetherOrNotToUse()) return;

            int index = (int)ItemType.mix;
            if (GameDataMgr.Get().availableItemCount[index] > 0)
            {
                GameDataMgr.Get().isUseItem[index] = !GameDataMgr.Get().isUseItem[index];
                bool isUseItem = GameDataMgr.Get().isUseItem[index];
                itemClicked.SetInfo(isUseItem, (ItemType)index);
                itemUI.GetComponent<UISprite>().depth = (isUseItem == true) ? 6 : 2;
            }
            else
            {
                //아이템 샵 팝업.
            }
        }
    }
}