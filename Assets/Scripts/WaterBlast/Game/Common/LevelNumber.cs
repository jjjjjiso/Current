using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class LevelNumber : MonoBehaviour
    {
        [SerializeField] private UILabel uiLevel = null;

        private void Awake()
        {
            string num = string.Format("Level {0}", GameDataMgr.G.endLevel.ToString());
            if (uiLevel != null) uiLevel.text = num;
        }
    }
}