using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WonderBlast.Game.Common
{
    public class LevelBlock
    {
        public BlockerType blockerType;
    }

    public class LevelBlockType : LevelBlock
    {
        public BlockType type;
    }

    public class LevelSpecialType : LevelBlock
    {
        public SpecialType type;
    }
}