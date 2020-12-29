using System.Globalization;

using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class BoosterItem : MonoBehaviour
    {
        [SerializeField] private ButtonObject btn;
        [SerializeField] private GameObject lockImg = null;
        [SerializeField] private GameObject CheckImg = null;
        [SerializeField] private UIButton uiBtnImg = null;
        [SerializeField] private UILabel uiItemCnt = null;

        [SerializeField] private BoosterType type = BoosterType.none; 

        private bool isLock = false;
        private int itemCount = 0;

        private void Awake()
        {
            CheckImg.SetActive(false);

            btn.fncClick = OnPressed;
        }

        public void OnPressed()
        {
            if (isLock) return;

            SoundMgr.G.EffectPlay(EffectSound.btn_ok);

            int idx = (int)(type - 1);
            if (itemCount > 0)
            {
                if (!GameDataMgr.G.isUseStartItem[idx])
                {
                    GameDataMgr.G.isUseStartItem[idx] = true;
                    ++UserDataMgr.G.userStartItemCount[idx];

                    uiBtnImg.normalSprite = "blue_square_button";
                    CheckImg.SetActive(true);
                    uiItemCnt.gameObject.SetActive(false);
                    
                }
                else
                {
                    GameDataMgr.G.isUseStartItem[idx] = false;
                    --UserDataMgr.G.userStartItemCount[idx];

                    uiBtnImg.normalSprite = "green_square_button";
                    CheckImg.SetActive(false);
                    uiItemCnt.gameObject.SetActive(true);
                }
            }
            else
            {
                int count = 3;
                if (UserDataMgr.G.IsItemCoins(count))
                {
                    TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                    string boosterName = myTI.ToTitleCase(type.ToString()).ToUpper();
                    string msg = string.Format("Start the level with a {0}!", boosterName);
                    PopupMgr.G.ShowItemPopup("Booster Item Popup", boosterName, msg, "BUY", type, count, GameDataMgr.G.itemCost, () =>
                    {
                        UserDataMgr.G.CoinsUsed(GameDataMgr.G.itemCost * count);
                        UserDataMgr.G.availableStartItemCount[idx] += count;
                        SetItemCount(UserDataMgr.G.availableStartItemCount[idx]);
                        if (LobbyMgr.G != null && LobbyMgr.G.levelNumber != null) LobbyMgr.G.levelNumber.UpdateCoin();
                    });
                }
                else
                {
                    PopupMgr.G.ShowAdsPopup(null, "Not enough coins." + "\n" + "Watch ads and get rewarded.", "OK");
                }
            }
        }

        public void BoosterItemUnLock(bool isValue)
        {
            isLock = !isValue;
            lockImg.SetActive(!isValue);
        }

        public void SetItemCount(int itemCount)
        {
            this.itemCount = itemCount;
            string text = (itemCount != 0) ? itemCount.ToString() : "+";
            uiItemCnt.text = text;
        }
    }
}