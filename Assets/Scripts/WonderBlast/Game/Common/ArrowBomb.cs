using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public enum ArrowType
    {
        horizontal,
        vertical,
    }

    public class ArrowBomb : Special
    {
        protected ArrowType arrowType = ArrowType.horizontal;

        public override List<BlockDef> Match(int x, int y)
        {
            Stage s = GameMgr.Get()._Stage;
            List<BlockDef> blocks = new List<BlockDef>();

            switch (arrowType)
            {
                case ArrowType.horizontal:
                    {
                        //left
                        for (int ix = x; ix >= 0; --ix)
                        {
                            AddBlock(blocks, ix, y);
                        }
                        //right
                        for (int ix = x + 1; ix < s.width; ++ix)
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
                        for (int iy = y + 1; iy < s.height; ++iy)
                        {
                            AddBlock(blocks, x, iy);
                        }
                    }
                    break;
            }

            return blocks;
        }

        public override List<BlockDef> ComboMatch(int x, int y)
        {
            Stage s = GameMgr.Get()._Stage;
            List<BlockDef> blocks = new List<BlockDef>();

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
            for (int ix = x + 1; ix < s.width; ++ix)
            {
                AddBlock(blocks, ix, y);
            }
            //down
            for (int iy = y + 1; iy < s.height; ++iy)
            {
                AddBlock(blocks, x, iy);
            }

            return blocks;
        }

        public void UpdateSprite(ArrowType type)
        {
            this.arrowType = type;
            UpdateSprite(arrowType.ToString());
        }

        public void UpdateSprite(int type)
        {
            this.arrowType = (ArrowType)type;
            UpdateSprite(arrowType.ToString());
        }
        
        public ArrowType _ArrowType
        {
            get { return arrowType; }
            set { arrowType = value; }
        }
    }
}