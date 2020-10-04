using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class PopupAD : Popup
    {
        [SerializeField] UILabel rewardCoin;
        [SerializeField] UILabel rewardCount;

        public void SetInfo()
        {
            rewardCoin.text = GameDataMgr.G.adsRewardCost.ToString();
            int count = UserDataMgr.G.coinRewardedMaxCount - UserDataMgr.G.coinRewardedCount;
            rewardCount.text = string.Format("{0}/{1}", count, UserDataMgr.G.coinRewardedMaxCount);
        }
    }
}