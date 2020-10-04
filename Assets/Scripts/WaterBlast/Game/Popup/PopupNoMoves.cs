using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class PopupNoMoves : MonoBehaviour
    {
        [SerializeField] private UILabel uiCoin = null;

        public void SetCoin()
        {
            if (uiCoin != null) uiCoin.text = UserDataMgr.G.coin.ToString();
        }
    }
}