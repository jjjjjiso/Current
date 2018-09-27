using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public class Ranbow : Special
    {
        public override List<BlockDef> Match(int x, int y)
        {
            Stage s = GameMgr.Get()._Stage;
            List<BlockDef> blocks = new List<BlockDef>();

            bomb1 = type;
            bool isCombo = GetCombo(x, y, type);
            if (!isCombo)
            {
                RanbowMatch(blocks, x, y);
            }
            else
            {
                SpecialCombo(blocks, s.width, s.height, x, y);
            }

            return blocks;
        }
    }
}