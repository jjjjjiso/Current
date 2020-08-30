using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

public class UILobbyBg : MonoBehaviour
{
    public UITexture bg;
    public GameObject objClouds;

    public Texture[] bgs;

    private enum BgType { night, dawn, morning }
    private BgType type;

    private int curLevel;

    public void UpdateBg()
    {
        curLevel = GameDataMgr.G.endLevel;
        if (1 <= curLevel && 50 >= curLevel)
        {   // 1-50 밤
            type = BgType.night;
        }
        else if (51 <= curLevel && 80 >= curLevel)
        {    // 51-80 새벽
            type = BgType.dawn;
        }
        else
        {   // 81-100 아침
            type = BgType.morning;
        }

        bg.mainTexture = bgs[(int)type];
        objClouds.SetActive(type == BgType.night);
    }
}
