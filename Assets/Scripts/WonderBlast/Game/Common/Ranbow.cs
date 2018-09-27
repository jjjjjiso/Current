using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public class Ranbow : Special
    {
        private BlockType preType = BlockType.none;

        public override List<BlockDef> Match(int x, int y)
        {
            return new List<BlockDef>();
        }
        
        public void UpdateSprite(string strName)
        {
            if (uiSprite != null) uiSprite.spriteName = strName;
        }

        public BlockType _PreType
        {
            get { return preType; }
            set { preType = value; }
        }
    }
}