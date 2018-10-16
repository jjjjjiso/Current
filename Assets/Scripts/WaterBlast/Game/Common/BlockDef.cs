using System;
using System.Collections.Generic;

namespace WaterBlast.Game.Common
{
    public class BlockDef : IEquatable<BlockDef>
    {
        public int x;
        public int y;

        public BlockDef(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        //public int x { get; set; }
        //public int y { get; set; }

        public bool Equals(BlockDef other)
        {
            if (this.x == other.x && this.y == other.y) return true;

            return false;
        }
    }
}