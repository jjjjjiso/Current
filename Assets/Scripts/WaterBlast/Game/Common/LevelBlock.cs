using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.Common
{
    public class LevelBlock
    {
        public BlockerType blockerType;
    }

    public class LevelBlockType : LevelBlock
    {
        public BlockType type;
    }

    public class LevelBoosterType : LevelBlock
    {
        public BoosterType type;
    }
}