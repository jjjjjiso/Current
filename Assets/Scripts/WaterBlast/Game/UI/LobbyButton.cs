using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

public class LobbyButton : MonoBehaviour
{
    public LobbyButtonType type;

    public UISprite bg;
    public UISprite icon;

    public ButtonObject btn;

    public void Init()
    {
        btn.fncClick = OnPressed;
    }

    private bool isClick = false;

    public void OnPressed()
    {
        if (!isClick)
        {
            //LobbyMgr.G.uiLobbyBtnGrid.UpdateBtn(type);
        }
        else
        {
            SetBtnUp();
        }

        Debug.Log(type + " : " + bg.spriteName);
        /*if(!isClick)
        {
            isClick = true;
            bg.spriteName = "lobby_on_button";
            icon.color = new Color32(0, 106, 170, 255);
        }
        else
        {
            isClick = false;
            bg.spriteName = "lobby_button";
            icon.color = new Color32(62, 203, 219, 255);
        }*/
    }

    public void SetBtnDown()
    {
        isClick = true;
        bg.spriteName = "lobby_on_button";
        icon.color = new Color32(0, 106, 170, 255);

        Debug.Log(type + " : " + bg.spriteName);
    }

    public void SetBtnUp()
    {
        isClick = false;
        bg.spriteName = "lobby_button";
        icon.color = new Color32(62, 203, 219, 255);

        Debug.Log(type + " : " + bg.spriteName);
    }
}
