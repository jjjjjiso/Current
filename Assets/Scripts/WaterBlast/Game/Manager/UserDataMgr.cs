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
        public int coinRewardedCount = 0;
        public int[] availableStartItemsCount  = null;
        public int[] availableInGameItemsCount = null;

        //public DateTime quitTime;
        public int timeToNextLife = 0;
        public long quitTime;
    }

    public class UserDataMgr : MonoDontDestroySingleton<UserDataMgr>
    {
        static private readonly string SAVEDATA_FILE = @"/UserData.json";

        //[NonSerialized]
        public int life = 0;
        [NonSerialized] public int timeToNextLife = 0;
        //[NonSerialized]
        public int coin = 0;
        [NonSerialized] public int coinRewardedCount = 0;

        [NonSerialized] public int maxLife = 5;
        [NonSerialized] public int coinRewardedMaxCount = 10;
        [NonSerialized] public int lifeAddTime = 1800; // 초단위

        private DateTime loginTime;
        private DateTime quitTime;

        [Tooltip("시작 아이템 개수 index : 0 arrow, 1 bomb, 2 rainbow")]
        //[NonSerialized]
        public int[] availableStartItemCount = new int[3];
        [NonSerialized]
        public int[] userStartItemCount = new int[3];

        [Tooltip("인 게임 아이템 개수 index : 0 hammer, 1 horizon, 2 vertical, 3 mix")]
        //[NonSerialized]
        public int[] availableInGameItemCount = new int[4];

        private Coroutine co_timeToNextLife;

        protected override void OnAwake()
        {
            base.OnAwake();
            Load();

            StartTimeToNextLife();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) Save();
            else
            {
                Load();
                StartTimeToNextLife();
            }
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

        public void AddCoins(int cost)
        {
            coin += cost;
        }

        public void CoinsUsed(int cost)
        {
            coin -= cost;
            if (coin < 0) coin = 0;
        }

        public bool IsCoins(int useCoin)
        {
            return coin >= useCoin;
        }

        public bool IsItemCoins(int itemCnt)
        {
            return coin >= (GameDataMgr.G.itemCost * itemCnt);
        }

        public void SetTimeToNextLife()
        {
            if (!IsMaxLife() && timeToNextLife > 0) return;
            timeToNextLife = IsMaxLife() ? 0 : lifeAddTime;
        }

        public bool IsMaxLife()
        {
            return life >= maxLife;
        }

        public void UpdateLife()
        {
            --life;
            if (life <= 0) life = 0;
            SetTimeToNextLife();
            StartTimeToNextLife();
        }

        public void StartTimeToNextLife()
        {
            if (co_timeToNextLife != null)
            {
                StopCoroutine(co_timeToNextLife);
                co_timeToNextLife = null;
            }
            co_timeToNextLife = StartCoroutine(Co_TimeToNextLife());
        }

        IEnumerator Co_TimeToNextLife()
        {
            while (!IsMaxLife())
            {
                yield return new WaitForSeconds(1);
                --timeToNextLife;
                //Debug.Log("Co_TimeToNextLife = " + timeToNextLife);
                if (timeToNextLife <= 0)
                {
                    ++life;
                    SetTimeToNextLife();
                }

                if (LobbyMgr.G != null && LobbyMgr.G.levelNumber != null) LobbyMgr.G.levelNumber.UpdateLifeLabel();
            }
            yield break;
        }

        private void Save()
        {
            USER_SAVE data = new USER_SAVE();
            data.life = life;
            data.coin = coin;
            data.coinRewardedCount = coinRewardedCount;
            data.availableStartItemsCount  = availableStartItemCount;
            data.availableInGameItemsCount = availableInGameItemCount;
            data.timeToNextLife = timeToNextLife;
            data.quitTime = DateTime.Now.Ticks;

            SaveMgr.Save<USER_SAVE>(data, SAVEDATA_FILE);
        }

        private void Load()
        {
            USER_SAVE data = SaveMgr.Load<USER_SAVE>(SAVEDATA_FILE);
            if (data == null)
            {
                life = maxLife;
                coin = 500;
                return;
            }
            availableStartItemCount  = data.availableStartItemsCount;
            availableInGameItemCount = data.availableInGameItemsCount;
            life = data.life;
            coin = data.coin;
            coinRewardedCount = data.coinRewardedCount;
            quitTime = new DateTime(data.quitTime);
            loginTime = DateTime.Now;
            TimeSpan ts = loginTime - quitTime;
            if (quitTime.Day != loginTime.Day)
            {
                coinRewardedCount = 0;
            }

            if (ts.TotalSeconds > data.timeToNextLife)
            {   
                life = maxLife;
            }
            else
            {
                if (life < maxLife)
                {
                    timeToNextLife = 0;
                    int plusLife = Mathf.FloorToInt((data.timeToNextLife - (float)ts.TotalSeconds) / lifeAddTime);
                    life += plusLife;
                    timeToNextLife = Mathf.FloorToInt((data.timeToNextLife - (float)ts.TotalSeconds) - (lifeAddTime * plusLife));
                }
            }
        }
    }
}