﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Popup
{
    public class OpenInGameSetting : MonoBehaviour
    {
        public UIButton btn;
        [SerializeField] private UIPanel uiSettingPanel = null;
        [SerializeField] private GameObject uiBlack = null;

        private int oldDepth = 0;
        private PopupInGameSetting popup = null;

        public void OnPressed()
        {
            SoundMgr.G.EffectPlay(System.EffectSound.btn_ok);

            if (popup == null)
            {
                oldDepth = uiSettingPanel.depth;
                uiSettingPanel.depth = 5;
                ActiveBlack(true);

                popup = PopupInGameSetting.Open("In Game Setting");
                popup.onQuit += () =>
                {
                    PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/ExitGamePopup", "Exit Popup", null, null, "QUIT");
                    temp.onConfirm += () =>
                    {
                        temp.Close();
                        GameMgr.G.Failed();
                    };

                    uiSettingPanel.depth = oldDepth;
                    ActiveBlack(false);
                };
            }
            else
            {
                popup.OnExit();
                uiSettingPanel.depth = oldDepth;
                ActiveBlack(false);
            }
        }

        public void ActiveBlack(bool isValue)
        {
            uiBlack.SetActive(isValue);
        }
    }
}