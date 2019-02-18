using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class ItemUIElements : MonoBehaviour
    {
        [SerializeField]
        private Item[] items = null;

        public void ItemSetting(ItemType type, bool isEnable)
        {
            int index = (int)type;
            items[index].UnLockUI(index, isEnable);
        }

        public void UpdateInGameItem(ItemType type)
        {
            int index = (int)type;
            items[index].UpdateInGameItemCount(index);
            items[index].ResetInfo();
        }
    }
}