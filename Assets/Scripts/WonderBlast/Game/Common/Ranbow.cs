using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public class Ranbow : Special
    {
        //public override List<BlockDef> Match(int x, int y)
        //{
        //    Stage s = GameMgr.Get()._Stage;
        //    List<BlockDef> blocks = new List<BlockDef>();

        //    //bomb1 = type;
        //    //bool isCombo = GetCombo(x, y);
        //    //if (!isCombo)
        //    //{
        //    //    RanbowMatch(blocks, x, y);
        //    //}
        //    //else
        //    //{
        //    //    SpecialCombo(blocks, s.width, s.height, x, y);
        //    //}

        //    return blocks;
        //}

        public override List<BlockDef> Match(int x, int y)
        {
            List<BlockDef> blocks = new List<BlockDef>();

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
                    AddBlock(blocks, ix, iy);
                }
            }

            return blocks;
        }
    }
}