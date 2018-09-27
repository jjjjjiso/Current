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

            bomb1 = type;
            bool isCombo = GetCombo(x, y, type);
            if (!isCombo)
            {
                ArrowBombMatch(blocks, s.width, s.height, x, y, arrowType);
            }
            else
            {
                SpecialCombo(blocks, s.width, s.height, x, y);
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