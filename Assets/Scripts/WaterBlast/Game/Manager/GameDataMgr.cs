using System;

using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Game.Manager
{
    public class GameDataMgr : MonoDontDestroySingleton<GameDataMgr>
    {
        [Tooltip("마지막으로 끝난 레벨 번호")]
        public int endLevelNumber = 1;

        [Tooltip("아이템 사용 유무 index : 0 hammer, 1 horizon, 2 vertical, 3 mix")]
        [NonSerialized]
        public bool[] isUseItem = { false, false, false, false };

        [Tooltip("아이템 사용 유무 index : 0 hammer, 1 horizon, 2 vertical, 3 mix")]
        //[NonSerialized]
        public int[] availableItemCount = new int[4];

        public bool IsUseItem()
        {
            foreach(bool isValue in isUseItem)
            {
                if (isValue) return true;
            }

            return false;
        }
    }
}