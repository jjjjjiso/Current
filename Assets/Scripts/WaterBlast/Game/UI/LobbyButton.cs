using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyButton : MonoBehaviour
{
    public enum ButtonType { rank, home, shop }
    public ButtonType type; // 화면 전환시 사용 할것.

    public UISprite bg;
    public UISprite icon;

    private bool isClick = false;

    public void OnPressed()
    {
        if(!isClick)
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
        }
    }
}
