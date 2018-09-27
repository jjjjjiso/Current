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

            switch(arrowType)
            {
                case ArrowType.horizontal:
                    {
                        //up
                        for (int iy = y; iy >= 0; --iy)
                        {
                            AddBlock(blocks, x, iy);
                        }
                        //down
                        for (int iy = y+1; iy < s.height; ++iy)
                        {
                            AddBlock(blocks, x, iy);
                        }
                    }
                    break;
                case ArrowType.vertical:
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
            }

            return blocks;
        }

        public void UpdateSprite(ArrowType type)
        {
            this.arrowType = type;
            UpdateSprite();
        }

        public void UpdateSprite(int type)
        {
            this.arrowType = (ArrowType)type;
            UpdateSprite();
        }

        protected void UpdateSprite()
        {
            string strName = arrowType.ToString();
            if (uiSprite != null) uiSprite.spriteName = strName;
        }

        public ArrowType _ArrowType
        {
            get { return arrowType; }
            set { arrowType = value; }
        }
    }
}