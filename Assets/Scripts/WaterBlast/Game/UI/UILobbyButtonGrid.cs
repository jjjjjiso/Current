using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;

public class UILobbyButtonGrid : MonoBehaviour
{
    public LobbyButton[] btns;

    public void Init()
    {
        for (int i = 0; i < btns.Length; ++i)
        {
            btns[i].Init();
        }
    }

    public void UpdateBtn(LobbyButtonType type)
    {
        for (int i = 0; i < btns.Length; ++i)
        {
            if (btns[i].type == type)
            {
                btns[i].SetBtnDown();
                continue;
            }

            btns[i].SetBtnUp();
        }
    }
}
