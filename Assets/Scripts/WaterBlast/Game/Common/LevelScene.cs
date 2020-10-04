using UnityEngine;

using WaterBlast.Game.Manager;
using WaterBlast.Game.Popup;

namespace WaterBlast.Game.Common
{
    public class LevelScene : MonoBehaviour
    {
        [SerializeField] private GameObject uiLifePlusBtn = null;
        [SerializeField] private UILabel uiLife = null;
        [SerializeField] private GameObject objTimeToNextLife = null;
        [SerializeField] private UILabel uiTimeToNextLife = null;
        [SerializeField] private UILabel uiCoin = null;
        [SerializeField] private UILabel uiLevel = null;

        private int minutes = 0;
        private int seconds = 0;

        public void Init()
        {
            UpdateLifeLabel();
            UpdateCoin();

            string num = string.Format("LEVEL {0}", GameDataMgr.G.endLevel.ToString());
            if (uiLevel != null) uiLevel.text = num;
        }

        public void UpdateLifeLabel()
        {
            if (uiLifePlusBtn != null) uiLifePlusBtn.SetActive(!UserDataMgr.G.IsMaxLife());
            if (uiLife != null) uiLife.text = UserDataMgr.G.IsMaxLife() ? "Full" : UserDataMgr.G.life.ToString();
            if (uiTimeToNextLife != null)
            {
                objTimeToNextLife.SetActive(false);
                if (UserDataMgr.G.timeToNextLife > 0)
                {
                    objTimeToNextLife.SetActive(true);
                    minutes = UserDataMgr.G.timeToNextLife / 60;
                    seconds = UserDataMgr.G.timeToNextLife % 60;
                    uiTimeToNextLife.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
                }
            }
        }

        public void UpdateCoin()
        {
            if (uiCoin != null) uiCoin.text = UserDataMgr.G.coin.ToString();
        }

        public void ShowAds()
        {
            PopupMgr.G.ShowAdsPopup(null, "Watch ads and get rewarded.", "OK");
        }

        public void ShowPurchaseLife()
        {
            if (UserDataMgr.G.IsMaxLife()) return;
            int count = 1;
            if (UserDataMgr.G.IsCoins(count))
            {
                PopupMgr.G.ShowItemPopup("Life Item Popup", "LIFE", "You can play the game with life!", "BUY", 
                                         "life_icon", count, GameDataMgr.G.itemCost, () =>
                                         {
                                             UserDataMgr.G.CoinsUsed(GameDataMgr.G.itemCost * count);
                                             UserDataMgr.G.life += count;
                                             UserDataMgr.G.SetTimeToNextLife();
                                             UpdateLifeLabel();
                                             UpdateCoin();
                                         });
            }
            else
            {
                ShowAds();
            }
        }
    }
}

