using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;

namespace WaterBlast.Game.Popup
{
    public class GamePopup : MonoBehaviour
    {
        [SerializeField] GameObject[] contents = null;
        [SerializeField] GameObject itemGroup = null;

        private void Awake()
        {
            foreach(GameObject obj in contents)
            {
                obj.SetActive(false);
            }
        }
        
        public void OnPopup(GamePopupState state)
        {
            contents[(int)state].SetActive(true);
            if (state == GamePopupState.success) itemGroup.SetActive(false);
            else itemGroup.SetActive(true);
        }
    }
}