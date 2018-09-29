using System.Collections.Generic;
using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public partial class Special : BlockEntity
    {
        //public field

        //private field
        [SerializeField]
        protected SpecialType type = SpecialType.none;//폭탄 타입

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
            if (state == State.move || state == State.special_move) return;
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
        public SpecialType _SpecialType
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