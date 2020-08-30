using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class Block : BlockEntity
    {
        public UISprite img;

        [SerializeField]
        protected BlockType type = BlockType.empty;

        public BlockType _BlockType
        {
            get { return type; }
            set { type = value; }
        }

        public void OnPressed()
        {
            if (state == State.move || state == State.booster_move) return;
            GameMgr.G.StageUpdate(this);
        }
    }
}