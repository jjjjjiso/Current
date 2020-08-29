﻿using System;

using UnityEngine;

using FullSerializer;

using WaterBlast.System;
using WaterBlast.Game.Common;

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

        [Tooltip("테스트 씬인지 여부 = true -> test, false -> real game")]
        public bool isTestScene = false;

        [Tooltip("테스트 씬에서만 사용 시작 레벨 단계")]
        public int startLevel = 1;

        [Tooltip("마지막으로 끝난 레벨 단계")]
        public int endLevel = 1;

        [Tooltip("시작 아이템 사용 했는지 index : 0 arrow, 1 bomb, 2 rainbow")]
        [NonSerialized]
        public bool[] isUseStartItem = { false, false, false };

        [Tooltip("아이템 사용 중인지 index : 0 hammer, 1 horizon, 2 vertical, 3 mix")]
        [NonSerialized]
        public bool[] isUseInGameItem = { false, false, false, false };

        public bool isBGM    = true;
        public bool isEffect = true;
        
        [NonSerialized]
        public Level level = null;

        protected override void OnAwake()
        {
            //Load();
            UpdateLevel();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Reset()
        {
            for(int i=0; i<isUseStartItem.Length; ++i)
            {
                isUseStartItem[i] = false;
            }
        }

        public void UpdateLevel()
        {
            var serializer = new fsSerializer();
            level = FileUtils.LoadJsonFile<Level>(serializer, "Levels/" + ((!isTestScene) ? 2 : startLevel)/*endLevel*/);
        }

        public bool IsUseInGameItem()
        {
            foreach(bool isValue in isUseInGameItem)
            {
                if (isValue) return true;
            }

            return false;
        }

        private void Save()
        {
            GAMEDATA_SAVE data = new GAMEDATA_SAVE();
            data.endLevel = endLevel;
            data.isBGM    = isBGM;
            data.isEffect = isEffect;

            SaveMgr.Save<GAMEDATA_SAVE>(data, SAVEDATA_FILE);
        }

        private void Load()
        {
            GAMEDATA_SAVE data = SaveMgr.Load<GAMEDATA_SAVE>(SAVEDATA_FILE);
            if(data != null)
            {
                endLevel = data.endLevel;
                isBGM = data.isBGM;
                isEffect = data.isEffect;
            }
        }
    }
}