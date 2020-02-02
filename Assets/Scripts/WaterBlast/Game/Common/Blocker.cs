using UnityEngine;

using WaterBlast.System;

namespace WaterBlast.Game.Common
{
    public class Blocker : BlockEntity
    {
        [SerializeField]
        protected BlockerType type = BlockerType.none;

        public BlockerType _BlockerType
        {
            get { return type; }
            set { type = value; }
        }
    }
}