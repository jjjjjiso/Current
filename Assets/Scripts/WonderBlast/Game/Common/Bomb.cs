using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public class Bomb : Special
    {
        public override List<BlockDef> Match(int x, int y)
        {
            List<BlockDef> blocks = new List<BlockDef>();
            
            bomb1 = type;
            bool isCombo = GetCombo(x, y, type);
            if (!isCombo)
            {
                BombMatch(blocks, x, y);
            }
            else
            {
                SpecialCombo(blocks, GameMgr.Get()._Stage.width, GameMgr.Get()._Stage.height, x, y);
            }

            return blocks;
        }
    }
}