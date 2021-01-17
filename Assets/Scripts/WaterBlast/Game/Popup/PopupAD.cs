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
            int count = UserDataMgr.G.coinRewardedMaxCount - UserDataMgr.G.coinRewardedCount;
            rewardCount.text = string.Format("{0}/{1}", count, UserDataMgr.G.coinRewardedMaxCount);
            rewardCount.color = (count > 0) ? Color.white : Color.red;

            int cost = (count > 0) ? GameDataMgr.G.adsRewardCost : GameDataMgr.G.itemCost;
            rewardCoin.text = cost.ToString();
        }
    }
}