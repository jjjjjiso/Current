using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.Manager
{
    public class LobbyMgr : MonoSingleton<LobbyMgr>
    {
        public UILobbyBg uiLobbyBg;
        //public UILobbyButtonGrid uiLobbyBtnGrid;

        private void Start()
        {
            uiLobbyBg.UpdateBg();
            //uiLobbyBtnGrid.Init();
        }
    }
}