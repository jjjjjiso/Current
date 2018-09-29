using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public enum specialSynthesis
    {
        none,
        arrowBombAndBomb,
        arrowBombAndRanbow,
        BombAndRanbow,
    }

    public partial class Special : BlockEntity
    {
        //protected List<SpecialType> types = new List<SpecialType>();

        //protected SpecialType bomb1 = SpecialType.none;//합성 스킬 타입
        //protected SpecialType bomb2 = SpecialType.none;//합성 스킬 타입

        //protected void BombSynthesisTypeSetting()
        //{
        //    types.Add(bomb1);

        //    types.Sort( delegate(SpecialType a, SpecialType b) { return (b.CompareTo(a)); });

        //    bomb1 = types[0];
        //    bomb2 = types[1];

        //    types.Clear();
        //}

        //protected void SpecialCombo(List<BlockDef> blocks, int w, int h, int x, int y)
        //{
        //    BombSynthesisTypeSetting();
            
        //    if (bomb1 == bomb2)
        //    {
        //        switch (bomb1)
        //        {
        //            case SpecialType.arrow:
        //                ArrowBombAndArrowBomb(blocks, w, h, x, y);
        //                break;
        //            case SpecialType.bomb:
        //                BombAndBomb(blocks, x, y);
        //                break;
        //            case SpecialType.ranbow:
        //                RanbowAndRanbow(blocks, w, h, x, y);
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        switch(SpecialCombo(bomb1, bomb2))
        //        {
        //            case specialSynthesis.arrowBombAndBomb:
        //                ArrowBombAndBomb(blocks, w, h, x, y);
        //                break;
        //            case specialSynthesis.arrowBombAndRanbow:
        //                ArrowBombAndRanbow(blocks);
        //                break;
        //            case specialSynthesis.BombAndRanbow:
        //                BombAndRanbow(blocks);
        //                break;
        //        }
        //    }

        //    bomb1 = SpecialType.none;
        //    bomb2 = SpecialType.none;
        //}

        //protected specialSynthesis SpecialCombo(SpecialType a, SpecialType b)
        //{
        //    specialSynthesis type = specialSynthesis.none;
        //    switch (a)
        //    {
        //        case SpecialType.arrow:
        //            if     (b == SpecialType.bomb  ) type  = specialSynthesis.arrowBombAndBomb;
        //            else if(b == SpecialType.ranbow) type  = specialSynthesis.arrowBombAndRanbow;
        //            break;
        //        case SpecialType.bomb:
        //            if      (b == SpecialType.arrow ) type = specialSynthesis.arrowBombAndBomb;
        //            else if (b == SpecialType.ranbow) type = specialSynthesis.BombAndRanbow;
        //            break;
        //        case SpecialType.ranbow:
        //            if      (b == SpecialType.arrow) type  = specialSynthesis.arrowBombAndRanbow;
        //            else if (b == SpecialType.bomb ) type  = specialSynthesis.BombAndRanbow;
        //            break;
        //    }

        //    return type;
        //}

        //protected void ArrowBombMatch(List<BlockDef> blocks, int w, int h, int x, int y, ArrowType type)
        //{
        //    switch (type)
        //    {
        //        case ArrowType.horizontal:
        //            {
        //                //left
        //                for (int ix = x; ix >= 0; --ix)
        //                {
        //                    AddBlock(blocks, ix, y);
        //                }
        //                //right
        //                for (int ix = x + 1; ix < w; ++ix)
        //                {
        //                    AddBlock(blocks, ix, y);
        //                }
        //            }
        //            break;
        //        case ArrowType.vertical:
        //            {
        //                //up
        //                for (int iy = y; iy >= 0; --iy)
        //                {
        //                    AddBlock(blocks, x, iy);
        //                }
        //                //down
        //                for (int iy = y + 1; iy < h; ++iy)
        //                {
        //                    AddBlock(blocks, x, iy);
        //                }
        //            }
        //            break;
        //    }
        //}

        //protected void BombMatch(List<BlockDef> blocks, int x, int y)
        //{
        //    AddBlock(blocks, x, y);
        //    AddBlock(blocks, x - 1, y);
        //    AddBlock(blocks, x + 1, y);
        //    AddBlock(blocks, x, y - 1);
        //    AddBlock(blocks, x, y + 1);
        //    AddBlock(blocks, x - 1, y - 1);
        //    AddBlock(blocks, x + 1, y - 1);
        //    AddBlock(blocks, x - 1, y + 1);
        //    AddBlock(blocks, x + 1, y + 1);
        //}

        //protected void RanbowMatch(List<BlockDef> blocks, int x, int y)
        //{
        //    Stage s = GameMgr.Get()._Stage;

        //    AddBlock(blocks, x, y);

        //    for (int ix = 0; ix < s.width; ++ix)
        //    {
        //        for (int iy = 0; iy < s.height; ++iy)
        //        {
        //            BlockEntity entity = s.blockEntities[ix, iy];
        //            Block block = entity.GetComponent<Block>();
        //            if (block == null) continue;
        //            if (preType == BlockType.none || preType != block._BlockType) continue;
        //            string strTemp = string.Format("{0}_{1}", type, preType);
        //            UpdateSprite(strTemp);
        //            AddBlock(blocks, ix, iy);
        //        }
        //    }
        //}

        //protected void ArrowBombAndArrowBomb(List<BlockDef> blocks, int w, int h, int x, int y)
        //{
        //    //left
        //    for (int ix = x; ix >= 0; --ix)
        //    {
        //        AddBlock(blocks, ix, y);
        //    }
        //    //up
        //    for (int iy = y; iy >= 0; --iy)
        //    {
        //        AddBlock(blocks, x, iy);
        //    }
        //    //right
        //    for (int ix = x + 1; ix < w; ++ix)
        //    {
        //        AddBlock(blocks, ix, y);
        //    }
        //    //down
        //    for (int iy = y + 1; iy < h; ++iy)
        //    {
        //        AddBlock(blocks, x, iy);
        //    }
        //}

        //protected void ArrowBombAndBomb(List<BlockDef> blocks, int w, int h, int x, int y)
        //{
        //    for (int ix = x - 1; ix <= x + 1; ++ix)
        //    {
        //        //up
        //        for (int iy = y; iy >= 0; --iy)
        //        {
        //            AddBlock(blocks, ix, iy);
        //        }
        //        //down
        //        for (int iy = y + 1; iy < h; ++iy)
        //        {
        //            AddBlock(blocks, ix, iy);
        //        }
        //    }

        //    for (int iy = y - 1; iy <= y + 1; ++iy)
        //    {
        //        //left
        //        for (int ix = x; ix >= 0; --ix)
        //        {
        //            AddBlock(blocks, ix, iy);
        //        }
        //        //right
        //        for (int ix = x + 1; ix < w; ++ix)
        //        {
        //            AddBlock(blocks, ix, iy);
        //        }
        //    }
        //}

        //protected void ArrowBombAndRanbow(List<BlockDef> blocks)
        //{
        //    Stage s = GameMgr.Get()._Stage;
        //    Dictionary<BlockDef, ArrowType> temp = new Dictionary<BlockDef, ArrowType>();
        //    for (int x = 0; x < s.width; ++x)
        //    {
        //        for (int y = 0; y < s.height; ++y)
        //        {
        //            BlockEntity entity = s.blockEntities[x, y];
        //            Block block = entity.GetComponent<Block>();
        //            if (block == null) continue;
        //            if (preType != block._BlockType) continue;
        //            int random = Random.Range(0, 1 + 1);
        //            ArrowType randomArrow = (ArrowType)random;
        //            block.SetSprite(randomArrow.ToString());
        //            ArrowBombMatch(blocks, s.width, s.height, x, y, randomArrow);
        //        }
        //    }
        //}

        //protected void BombAndBomb(List<BlockDef> blocks, int x, int y)
        //{
        //    BombMatch(blocks, x, y);

        //    AddBlock(blocks, x, y - 2);
        //    AddBlock(blocks, x, y + 2);
        //    AddBlock(blocks, x - 1, y - 2);
        //    AddBlock(blocks, x - 1, y + 2);
        //    AddBlock(blocks, x - 2, y - 2);
        //    AddBlock(blocks, x - 2, y - 1);
        //    AddBlock(blocks, x - 2, y);
        //    AddBlock(blocks, x - 2, y + 1);
        //    AddBlock(blocks, x - 2, y + 2);
        //    AddBlock(blocks, x + 1, y - 2);
        //    AddBlock(blocks, x + 1, y + 2);
        //    AddBlock(blocks, x + 2, y - 2);
        //    AddBlock(blocks, x + 2, y - 1);
        //    AddBlock(blocks, x + 2, y);
        //    AddBlock(blocks, x + 2, y + 1);
        //    AddBlock(blocks, x + 2, y + 2);
        //}

        //protected void BombAndRanbow(List<BlockDef> blocks)
        //{
        //    Stage s = GameMgr.Get()._Stage;
        //    for (int x = 0; x < s.width; ++x)
        //    {
        //        for (int y = 0; y < s.height; ++y)
        //        {
        //            BlockEntity entity = s.blockEntities[x, y];
        //            Block block = entity.GetComponent<Block>();
        //            if (block == null || preType != block._BlockType) continue;
        //            block.SetSprite(SpecialType.bomb.ToString());
        //            BombMatch(blocks, x, y);
        //        }
        //    }
        //}

        //protected void RanbowAndRanbow(List<BlockDef> blocks, int w, int h, int x, int y)
        //{
        //    for (int ix = 0; ix < w; ++ix)
        //    {
        //        for (int iy = 0; iy < h; ++iy)
        //        {
        //            AddBlock(blocks, ix, iy);
        //        }
        //    }
        //}
    }
}