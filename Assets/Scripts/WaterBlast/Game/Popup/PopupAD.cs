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
        [SerializeField] UILabel uiCoin;

        public void SetInfo()
        {
            UserDataMgr.G.UpdateAdsRewardedCount();

            int count = UserDataMgr.G.coinRewardedMaxCount - UserDataMgr.G.coinRewardedCount;
            rewardCount.text = string.Format("{0}/{1}", count, UserDataMgr.G.coinRewardedMaxCount);
            rewardCount.color = (count > 0) ? Color.black : Color.red;

            int cost = (count > 0) ? GameDataMgr.G.adsRewardCost : GameDataMgr.G.adsRewardTenCost;
            rewardCoin.text = cost.ToString();

            if (uiCoin != null) uiCoin.text = UserDataMgr.G.coin.ToString();
        }
    }
}