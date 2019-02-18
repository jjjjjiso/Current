using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.Popup
{
    public class GamePopupItemCount : MonoBehaviour
    {
        [SerializeField] private UILabel[]  uiItemCnt = null;

        public void SetItemCount(int[] itemCount)
        {
            for(int i=0; i<itemCount.Length; ++i)
            {
                string text = (itemCount[i] != 0) ? itemCount[i].ToString() : "+";
                uiItemCnt[i].text = text;
            }
        }
    }
}