using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

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

        public void OnPressed()
        {
            if (state == State.move || state == State.booster_move) return;
            GameMgr.G.StageUpdate(this);
        }
    }
}