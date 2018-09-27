using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public partial class Special : BlockEntity
    {
        protected SpecialType bomb1 = SpecialType.none;//합성 스킬 타입
        protected SpecialType bomb2 = SpecialType.none;//합성 스킬 타입

        protected void SpecialCombo(List<BlockDef> blocks, int w, int h, int x, int y)
        {
            if (bomb1 == bomb2)
            {
                switch (bomb1)
                {
                    case SpecialType.arrow:
                        ArrowBombAndArrowBomb(blocks, w, h, x, y);
                        break;
                    case SpecialType.bomb:
                        BombAndBomb(blocks, x, y);
                        break;
                    case SpecialType.ranbow:
                        RanbowAndRanbow(blocks, w, h, x, y);
                        break;
                }
            }
            else
            {
                if ((bomb1 == SpecialType.arrow && bomb2 == SpecialType.bomb) ||
                    (bomb1 == SpecialType.bomb && bomb2 == SpecialType.arrow))
                {
                    ArrowBombAndBomb(blocks, w, h, x, y);
                }
                else if ((bomb1 == SpecialType.arrow && bomb2 == SpecialType.ranbow) ||
                         (bomb1 == SpecialType.ranbow && bomb2 == SpecialType.arrow))
                {
                    ArrowBombAndRanbow(blocks);
                }
                else if ((bomb1 == SpecialType.bomb && bomb2 == SpecialType.ranbow) ||
                         (bomb1 == SpecialType.ranbow && bomb2 == SpecialType.bomb))
                {
                    BombAndRanbow(blocks);
                }
            }

            bomb1 = SpecialType.none;
            bomb2 = SpecialType.none;
        }
        protected void ArrowBombMatch(List<BlockDef> blocks, int w, int h, int x, int y, ArrowType type)
        {
            switch (type)
            {
                case ArrowType.horizontal:
                    {
                        //left
                        for (int ix = x; ix >= 0; --ix)
                        {
                            AddBlock(blocks, ix, y);
                        }
                        //right
                        for (int ix = x + 1; ix < w; ++ix)
                        {
                            AddBlock(blocks, ix, y);
                        }
                    }
                    break;
                case ArrowType.vertical:
                    {
                        //up
                        for (int iy = y; iy >= 0; --iy)
                        {
                            AddBlock(blocks, x, iy);
                        }
                        //down
                        for (int iy = y + 1; iy < h; ++iy)
                        {
                            AddBlock(blocks, x, iy);
                        }
                    }
                    break;
            }
        }

        protected void BombMatch(List<BlockDef> blocks, int x, int y)
        {
            AddBlock(blocks, x, y);
            AddBlock(blocks, x - 1, y);
            AddBlock(blocks, x + 1, y);
            AddBlock(blocks, x, y - 1);
            AddBlock(blocks, x, y + 1);
            AddBlock(blocks, x - 1, y - 1);
            AddBlock(blocks, x + 1, y - 1);
            AddBlock(blocks, x - 1, y + 1);
            AddBlock(blocks, x + 1, y + 1);
        }

        protected void RanbowMatch(List<BlockDef> blocks, int x, int y)
        {
            Stage s = GameMgr.Get()._Stage;

            AddBlock(blocks, x, y);

            for (int ix = 0; ix < s.width; ++ix)
            {
                for (int iy = 0; iy < s.height; ++iy)
                {
                    BlockEntity entity = s.blockEntities[ix, iy];
                    Block block = entity.GetComponent<Block>();
                    if (block == null) continue;
                    if (preType == BlockType.none || preType != block._BlockType) continue;
                    string strTemp = string.Format("{0}_{1}", type, preType);
                    UpdateSprite(strTemp);
                    AddBlock(blocks, ix, iy);
                }
            }
        }

        protected void ArrowBombAndArrowBomb(List<BlockDef> blocks, int w, int h, int x, int y)
        {
            //left
            for (int ix = x; ix >= 0; --ix)
            {
                AddBlock(blocks, ix, y);
            }
            //up
            for (int iy = y; iy >= 0; --iy)
            {
                AddBlock(blocks, x, iy);
            }
            //right
            for (int ix = x + 1; ix < w; ++ix)
            {
                AddBlock(blocks, ix, y);
            }
            //down
            for (int iy = y + 1; iy < h; ++iy)
            {
                AddBlock(blocks, x, iy);
            }
        }

        protected void ArrowBombAndBomb(List<BlockDef> blocks, int w, int h, int x, int y)
        {
            for (int ix = x - 1; ix <= x + 1; ++ix)
            {
                //up
                for (int iy = y; iy >= 0; --iy)
                {
                    AddBlock(blocks, ix, iy);
                }
                //down
                for (int iy = y + 1; iy < h; ++iy)
                {
                    AddBlock(blocks, ix, iy);
                }
            }

            for (int iy = y - 1; iy <= y + 1; ++iy)
            {
                //left
                for (int ix = x; ix >= 0; --ix)
                {
                    AddBlock(blocks, ix, iy);
                }
                //right
                for (int ix = x + 1; ix < w; ++ix)
                {
                    AddBlock(blocks, ix, iy);
                }
            }
        }

        protected void ArrowBombAndRanbow(List<BlockDef> blocks)
        {
            Stage s = GameMgr.Get()._Stage;
            for (int ix = 0; ix < s.width; ++ix)
            {
                for (int iy = 0; iy < s.height; ++iy)
                {
                    BlockEntity entity = s.blockEntities[ix, iy];
                    Block block = entity.GetComponent<Block>();
                    if (block == null) continue;
                    if (preType == BlockType.none || preType != block._BlockType) continue;
                    int random = Random.Range(0, 1 + 1);
                    ArrowType randomArrow = (ArrowType)random;
                    block.SetSprite(randomArrow.ToString());
                    ArrowBombMatch(blocks, s.width, s.height, ix, iy, randomArrow);
                }
            }
        }

        protected void BombAndBomb(List<BlockDef> blocks, int x, int y)
        {
            BombMatch(blocks, x, y);

            AddBlock(blocks, x, y - 2);
            AddBlock(blocks, x, y + 2);
            AddBlock(blocks, x - 1, y - 2);
            AddBlock(blocks, x - 1, y + 2);
            AddBlock(blocks, x - 2, y - 2);
            AddBlock(blocks, x - 2, y - 1);
            AddBlock(blocks, x - 2, y);
            AddBlock(blocks, x - 2, y + 1);
            AddBlock(blocks, x - 2, y + 2);
            AddBlock(blocks, x + 1, y - 2);
            AddBlock(blocks, x + 1, y + 2);
            AddBlock(blocks, x + 2, y - 2);
            AddBlock(blocks, x + 2, y - 1);
            AddBlock(blocks, x + 2, y);
            AddBlock(blocks, x + 2, y + 1);
            AddBlock(blocks, x + 2, y + 2);
        }

        protected void BombAndRanbow(List<BlockDef> blocks)
        {
            Stage s = GameMgr.Get()._Stage;
            for (int ix = 0; ix < s.width; ++ix)
            {
                for (int iy = 0; iy < s.height; ++iy)
                {
                    BlockEntity entity = s.blockEntities[ix, iy];
                    Block block = entity.GetComponent<Block>();
                    if (block == null) continue;
                    if (preType == BlockType.none || preType != block._BlockType) continue;
                    int random = Random.Range(0, 1 + 1);
                    block.SetSprite(SpecialType.bomb.ToString());
                    BombMatch(blocks, ix, iy);
                }
            }
        }

        protected void RanbowAndRanbow(List<BlockDef> blocks, int w, int h, int x, int y)
        {
            for (int ix = 0; ix < w; ++ix)
            {
                for (int iy = 0; iy < h; ++iy)
                {
                    AddBlock(blocks, ix, iy);
                }
            }
        }
    }
}