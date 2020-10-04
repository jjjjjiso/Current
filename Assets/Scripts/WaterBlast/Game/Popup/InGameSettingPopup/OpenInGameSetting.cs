﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class OpenInGameSetting : MonoBehaviour
    {
        [SerializeField] private UIPanel uiSettingPanel = null;
        [SerializeField] private GameObject uiBlack = null;

        private int oldDepth = 0;
        private PopupInGameSetting popup = null;

        public void OnPressed()
        {
            if(popup == null)
            {
                oldDepth = uiSettingPanel.depth;
                uiSettingPanel.depth = 5;
                uiBlack.SetActive(true);

                popup = PopupInGameSetting.Open("In Game Setting");
                popup.onQuit += () =>
                {
                    PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/ExitGamePopup", "Exit Popup", null, null, "QUIT");
                    temp.onExit += () =>
                    {
                        temp.Close();
                        GameMgr.G.Failed();
                    };

                    uiSettingPanel.depth = oldDepth;
                    uiBlack.SetActive(false);
                };

                popup.onBGM += () =>
                {

                };

                popup.onEffect += () =>
                {

                };
            }
            else
            {
                popup.OnExit();
                uiSettingPanel.depth = oldDepth;
                uiBlack.SetActive(false);
            }
        }
    }
}