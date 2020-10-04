using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class PopupItem : MonoBehaviour
    {
        [SerializeField] private UISprite uiItemImg = null;
        [SerializeField] private UILabel uiItemCount = null;
        [SerializeField] private UILabel uiPrice = null;
        [SerializeField] private GameObject black = null;

        [SerializeField] private GameObject objBuyBtn = null;
        [SerializeField] private GameObject objConfirmBtn = null;

        [SerializeField] private UILabel uiCoin = null;

        public void ItemSetting(BoosterType type, int count, int price)
        {
            SetCoin();
            SetBtn(true);
            black.SetActive(true);
            if (type == BoosterType.arrow)
                uiItemImg.spriteName = "horizon";
            else
                uiItemImg.spriteName = type.ToString();

            uiItemCount.text = string.Format("x{0}", count.ToString());
            uiPrice.text = (count * price).ToString();
        }

        public void ItemSetting(string name, int count, int price, bool isBlack = false, bool isTemp = true)
        {
            SetCoin();
            SetBtn(true);
            black.SetActive(isBlack);
            if (!isTemp) uiItemImg.transform.eulerAngles = new Vector3(0, 0, 90f);
            uiItemImg.spriteName = name;
            uiItemCount.text = string.Format("x{0}", count.ToString());
            uiPrice.text = (count * price).ToString();
        }

        public void UpdateData(bool isBuy)
        {
            SetBtn(isBuy);
            SetCoin();
        }

        private void SetBtn(bool isBuy)
        {
            objBuyBtn.SetActive(isBuy);
            objConfirmBtn.SetActive(!isBuy);
        }

        private void SetCoin()
        {
            if (uiCoin != null) uiCoin.text = UserDataMgr.G.coin.ToString();
        }
    }
}