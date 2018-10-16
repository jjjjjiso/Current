using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.Manager
{
    public class GameDataMgr : MonoDontDestroySingleton<GameDataMgr>
    {
        [Tooltip("마지막으로 끝난 레벨 번호")]
        public int endLevelNumber = 1;
    }
}