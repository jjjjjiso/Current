using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class GamePopupItemGroup : MonoBehaviour
    {
        [SerializeField] private BoosterItem[] items = null;

        public void BoosterItemSetting()
        {
            foreach (KeyValuePair<BoosterType, bool> keyValue in GameDataMgr.G.level.availableStartItem)
            {
                if (keyValue.Key == BoosterType.none) continue;
                int index = (int)(keyValue.Key-1);
                items[index].BoosterItemUnLock(keyValue.Value);
            }

            for (int i=0; i<items.Length; ++i)
            {
                items[i].SetItemCount(UserDataMgr.G.availableStartItemCount[i]);
            }
        }
    }
}