using System.Collections.Generic;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class Rainbow : Booster
    {
        protected BlockType preType = BlockType.empty;

        public override int BonusScore() { return 70; }

        public override List<BlockEntity> Match(int x, int y)
        {
            List<BlockEntity> blocks = new List<BlockEntity>();
            Stage s = GameMgr.G._Stage;

            AddBlock(blocks, x, y);
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

        public override List<BlockEntity> ComboMatch(int x, int y)
        {
            List<BlockEntity> blocks = new List<BlockEntity>();
            Stage s = GameMgr.G._Stage;
            
            for (int ix = 0; ix < s.width; ++ix)
            {
                for (int iy = 0; iy < s.height; ++iy)
                {
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