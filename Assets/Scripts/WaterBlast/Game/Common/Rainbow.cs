using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class Rainbow : Booster
    {
        protected BlockType preType = BlockType.empty;

        public override List<BlockDef> Match(int x, int y)
        {
            List<BlockDef> blocks = new List<BlockDef>();

            Stage s = GameMgr.Get()._Stage;

            for (int ix = 0; ix < s.width; ++ix)
            {
                for (int iy = 0; iy < s.height; ++iy)
                {
                    if (ix == x && iy == y) continue;
                    Block block = s.blockEntities[ix, iy] as Block;
                    if (block == null) continue;
                    if (preType == BlockType.empty || preType != block._BlockType) continue;
                    string strTemp = string.Format("{0}_{1}", type, preType);
                    UpdateSprite(strTemp);
                    AddBlock(blocks, ix, iy);
                }
            }

            return blocks;
        }

        public override List<BlockDef> ComboMatch(int x, int y)
        {
            List<BlockDef> blocks = new List<BlockDef>();

            Stage s = GameMgr.Get()._Stage;

            for (int ix = 0; ix < s.width; ++ix)
            {
                for (int iy = 0; iy < s.height; ++iy)
                {
                    if (ix == x && iy == y) continue;
                    AddBlock(blocks, ix, iy);
                }
            }

            return blocks;
        }

        public BlockType _PreType
        {
            get { return preType; }
            set { preType = value; }
        }
    }
}