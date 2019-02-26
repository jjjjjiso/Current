using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;

namespace WaterBlast.Game.Popup
{
    public class PopupItem : MonoBehaviour
    {
        [SerializeField] private UISprite uiItemImg = null;
        [SerializeField] private UILabel  uiPrice    = null;
        [SerializeField] private GameObject black = null;

        public void ItemSetting(BoosterType type, int price)
        {
            black.SetActive(true);
            if (type == BoosterType.arrow)
                uiItemImg.spriteName = "horizon";
            else
                uiItemImg.spriteName = type.ToString();
            uiPrice.text         = price.ToString();
        }

        public void ItemSetting(string name, int price, bool isTemp = true)
        {
            black.SetActive(false);
            if (!isTemp) //임시
            {
                uiItemImg.transform.eulerAngles = new Vector3(0, 0, 90f);
            }
            uiItemImg.spriteName = name;
            uiPrice.text = price.ToString();
        }
    }
}