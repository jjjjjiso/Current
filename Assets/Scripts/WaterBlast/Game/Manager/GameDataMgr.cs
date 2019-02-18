using System;

using UnityEngine;

namespace WaterBlast.Game.Manager
{
    [Serializable]
    public class GAMEDATA_SAVE
    {
        public int endLevel  = 1;
        public bool isBGM    = true;
        public bool isEffect = true;
    }

    public class GameDataMgr : MonoDontDestroySingleton<GameDataMgr>
    {
        static private readonly string SAVEDATA_FILE = @"/GameData.json";

        [Tooltip("마지막으로 끝난 레벨 단계")]
        public int endLevel = 1;

        [Tooltip("아이템 사용 중인지 index : 0 hammer, 1 horizon, 2 vertical, 3 mix")]
        [NonSerialized]
        public bool[] isUseItem = { false, false, false, false };

        public bool isBGM    = true;
        public bool isEffect = true;

        public bool IsUseItem()
        {
            foreach(bool isValue in isUseItem)
            {
                if (isValue) return true;
            }

            return false;
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void Save()
        {
            GAMEDATA_SAVE data = new GAMEDATA_SAVE();
            data.endLevel = endLevel;
            data.isBGM    = isBGM;
            data.isEffect = isEffect;

            SaveMgr.G.Save<GAMEDATA_SAVE>(data, SAVEDATA_FILE);
        }

        private void Load()
        {
            GAMEDATA_SAVE data = SaveMgr.G.Load<GAMEDATA_SAVE>(SAVEDATA_FILE);
            endLevel = data.endLevel;
            isBGM    = data.isBGM;
            isEffect = data.isEffect;
        }
    }
}