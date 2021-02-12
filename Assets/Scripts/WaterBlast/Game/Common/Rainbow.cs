using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class Rainbow : Booster
    {
        protected BlockType preType = BlockType.empty;
        protected ColorType preColType = ColorType.none;

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
                    UpdateSprite();
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

        public void UpdateSprite(BlockType pre)
        {
            preType = pre;
            switch (pre)
            {
                case BlockType.red: preColType = ColorType.red; break;
                case BlockType.orange: preColType = ColorType.orange; break;
                case BlockType.yellow: preColType = ColorType.yellow; break;
                case BlockType.green: preColType = ColorType.green; break;
                case BlockType.blue: preColType = ColorType.blue; break;
                case BlockType.purple: preColType = ColorType.purple; break;
            }
            UpdateSprite();
        }

        public void UpdateSprite(ColorType pre)
        {
            switch (pre)
            {
                case ColorType.none:
                    UpdateSprite((BlockType)Random.Range((int)BlockType.red, (int)BlockType.purple + 1));
                    return;
                case ColorType.red: preType = BlockType.red; break;
                case ColorType.orange: preType = BlockType.orange; break;
                case ColorType.yellow: preType = BlockType.yellow; break;
                case ColorType.green: preType = BlockType.green; break;
                case ColorType.blue: preType = BlockType.blue; break;
                case ColorType.purple: preType = BlockType.purple; break;
            }
            preColType = pre;
            UpdateSprite();
        }

        public void UpdateSprite()
        {
            string strTemp = string.Format("{0}_{1}", type, preType);
            UpdateSprite(strTemp);
        }

        public BlockType _PreType
        {
            get { return preType; }
            set { preType = value; }
        }

        public ColorType _PreColType
        {
            get { return preColType; }
            set { preColType = value; }
        }
    }
}