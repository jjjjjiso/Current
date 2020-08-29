using UnityEngine;

using WaterBlast.System;

namespace WaterBlast.Game.Common
{
    public class Blocker : BlockEntity
    {
        public UISprite sprite;

        [SerializeField]
        protected BlockerType type = BlockerType.none;

        public BlockerType _BlockerType
        {
            get { return type; }
            set { type = value; }
        }
    }
}