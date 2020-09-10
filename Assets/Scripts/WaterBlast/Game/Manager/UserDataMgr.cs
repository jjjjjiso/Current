using System.Collections;
using System;
using UnityEngine;

namespace WaterBlast.Game.Manager
{
    [Serializable]
    public class USER_SAVE
    {
        public int life = 0;
        public int coin = 0;
        public int[] availableStartItemsCount  = null;
        public int[] availableInGameItemsCount = null;
    }

    public class UserDataMgr : MonoDontDestroySingleton<UserDataMgr>
    {
        static private readonly string SAVEDATA_FILE = @"/UserData.json";

        [NonSerialized]
        public int life = 0;
        [NonSerialized]
        public int coin = 0;

        [Tooltip("시작 아이템 개수 index : 0 arrow, 1 bomb, 2 rainbow")]
        //[NonSerialized]
        public int[] availableStartItemCount = new int[3];
        [HideInInspector]
        public int[] userStartItemCount = new int[3];

        [Tooltip("인 게임 아이템 개수 index : 0 hammer, 1 horizon, 2 vertical, 3 mix")]
        //[NonSerialized]
        public int[] availableInGameItemCount = new int[4];

        protected override void OnAwake()
        {
            base.OnAwake();
            //Load();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void SetUseStartItem()
        {
            for (int i = 0; i < availableStartItemCount.Length; ++i)
            {
                availableStartItemCount[i] -= userStartItemCount[i];
                if (availableStartItemCount[i] < 0) availableStartItemCount[i] = 0;
                userStartItemCount[i] = 0;
            }
        }

        private void Save()
        {
            USER_SAVE data = new USER_SAVE();
            data.life = life;
            data.coin = coin;
            data.availableStartItemsCount  = availableStartItemCount;
            data.availableInGameItemsCount = availableInGameItemCount;

            SaveMgr.Save<USER_SAVE>(data, SAVEDATA_FILE);
        }

        private void Load()
        {
            USER_SAVE data = SaveMgr.Load<USER_SAVE>(SAVEDATA_FILE);
            availableStartItemCount  = data.availableStartItemsCount;
            availableInGameItemCount = data.availableInGameItemsCount;
            life = data.life;
            coin = data.coin;
        }
    }
}