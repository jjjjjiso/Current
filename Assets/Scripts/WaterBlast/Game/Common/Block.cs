using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class Block : BlockEntity
    {
        [SerializeField]
        protected BlockType type = BlockType.empty;

        public BlockType _BlockType
        {
            get { return type; }
            set { type = value; }
        }
    }
}