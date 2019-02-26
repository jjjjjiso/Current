using System.Globalization;

using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class BoosterItem : MonoBehaviour
    {
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
        }

        public void OnPressed()
        {
            if (isLock) return;
            if(itemCount > 0)
            {
                int idx = (int)(type - 1);
                if (!GameDataMgr.G.isUseStartItem[idx])
                {
                    GameDataMgr.G.isUseStartItem[idx] = true;
                    --UserDataMgr.G.availableStartItemCount[idx];

                    uiBtnImg.normalSprite = "BlueSquareButton";
                    CheckImg.SetActive(true);
                    uiItemCnt.gameObject.SetActive(false);
                    
                }
                else
                {
                    GameDataMgr.G.isUseStartItem[idx] = false;
                    ++UserDataMgr.G.availableStartItemCount[idx];

                    uiBtnImg.normalSprite = "GreenSquareButton";
                    CheckImg.SetActive(false);
                    uiItemCnt.gameObject.SetActive(true);
                }
            }
            else
            {
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                string boosterName = myTI.ToTitleCase(type.ToString());
                string msg = string.Format("Start the level with a {0}!", boosterName);
                PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/ItemPopup", "Booster Item Popup", boosterName, msg, "Buy");
                temp.GetComponent<PopupItem>().ItemSetting(type, 150);

                temp.onConfirm += () =>
                {

                };
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