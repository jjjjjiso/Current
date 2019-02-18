using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.UI
{
    public class Hammer : Item
    {
        public void OnPressed()
        {
            if (!IsWhetherOrNotToUse()) return;

            int index = (int)ItemType.hammer;
            if (UserDataMgr.G.availableInGameItemCount[index] > 0)
            {
                GameDataMgr.G.isUseItem[index] = !GameDataMgr.G.isUseItem[index];
                bool isUseItem = GameDataMgr.G.isUseItem[index];
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
            }
        }
    }
}