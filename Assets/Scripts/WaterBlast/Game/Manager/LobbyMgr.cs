using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Game.Manager
{
    public class LobbyMgr : MonoSingleton<LobbyMgr>
    {
        public UILobbyBg uiLobbyBg;
        //public UILobbyButtonGrid uiLobbyBtnGrid;
        public LevelScene levelNumber;

        private void Start()
        {
            SoundMgr.G.BGMChangePlay(System.BGMSound.lobby);

            uiLobbyBg.UpdateBg();
            //uiLobbyBtnGrid.Init();
            levelNumber.Init();
        }
    }
}