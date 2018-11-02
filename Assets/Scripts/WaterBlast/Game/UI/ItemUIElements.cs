using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class ItemUIElements : MonoBehaviour
    {
        [SerializeField]
        private Item[] items = null;

        public void ItemSetting(ItemType type, bool isEnable)
        {
            items[(int)type].LockUI(isEnable);
        }

        public void ItemReset(ItemType type)
        {
            items[(int)type].ResetInfo();
        }
    }
}