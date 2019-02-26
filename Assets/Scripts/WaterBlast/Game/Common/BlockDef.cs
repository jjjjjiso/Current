using System;
using System.Collections.Generic;

namespace WaterBlast.Game.Common
{
    public class BlockDef : IEquatable<BlockDef>
    {
        public int x;
        public int y;

        //public int x { get; set; }
        //public int y { get; set; }

        //public BlockDef() { }

        public BlockDef(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "x: " + x + ", y: " + y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            BlockDef objAsBlockDef = obj as BlockDef;
            if (objAsBlockDef == null) return false;
            else return Equals(objAsBlockDef);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(BlockDef other)
        {
            if (other == null) return false;
            return (this.x.Equals(other.x) && this.y.Equals(other.y));
        }
    }
}