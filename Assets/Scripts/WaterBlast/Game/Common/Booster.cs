using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public partial class Booster : BlockEntity
    {
        //public field

        //private field
        [SerializeField]
        protected BoosterType type = BoosterType.none;//폭탄 타입

        protected float delayTime = 0.3f;

        protected bool isCombo = false;

        //default Method

        //public Method
        public virtual List<BlockDef> Match(int x, int y)
        {
            return new List<BlockDef>();
        }

        public virtual List<BlockDef> ComboMatch(int x, int y)
        {
            return new List<BlockDef>();
        }

        public void OnPressed()
        {
            if (state == State.move || state == State.booster_move) return;
            GameMgr.Get().StageUpdate(_X, _Y, type);
        }

        protected void AddBlock(List<BlockDef> blocks, int x, int y)
        {
            GameMgr gameMgr = GameMgr.Get();
            if (x < 0 || x >= gameMgr._Stage.width ||
                y < 0 || y >= gameMgr._Stage.height) return;

            BlockDef def = new BlockDef(x, y);
            if (!blocks.Contains(def)) blocks.Add(def);
        }

        protected bool IsValidBlock(int x, int y)
        {
            GameMgr gameMgr = GameMgr.Get();
            return x >= 0 && x < gameMgr._Stage.width && y >= 0 && y < gameMgr._Stage.height;
        }

        public void UpdateSprite(string strName)
        {
            if (uiSprite != null) uiSprite.spriteName = strName;
        }

        //coroutine Method

        //Property
        public BoosterType _BoosterType
        {
            get { return type; }
            set { type = value; }
        }

        public bool _IsCombo
        {
            get { return isCombo; }
            set { isCombo = value; }
        }
    }
}