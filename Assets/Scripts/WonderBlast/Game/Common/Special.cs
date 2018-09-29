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
        protected BlockType preType = BlockType.none;

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

        //protected bool GetCombo(int x, int y)
        //{
        //    var up = new BlockDef(x, y - 1);
        //    var down = new BlockDef(x, y + 1);
        //    var left = new BlockDef(x - 1, y);
        //    var right = new BlockDef(x + 1, y);

        //    bool isCombo = false;

        //    if (IsCombo(x, y, up.x, up.y)) isCombo = true;
        //    if (IsCombo(x, y, down.x, down.y)) isCombo = true;
        //    if (IsCombo(x, y, left.x, left.y)) isCombo = true;
        //    if (IsCombo(x, y, right.x, right.y)) isCombo = true;

        //    return isCombo;
        //}

        //protected bool IsCombo(int pickX, int pickY, int x, int y)
        //{
        //    if (!IsValidBlock(x, y)) return false;
        //    Stage s = GameMgr.Get()._Stage;
        //    Special special = s.blockEntities[x, y].GetComponent<Special>();
        //    if (special == null) return false;
        //    if (!special.gameObject.activeSelf) return false;

        //    if(!types.Contains(special._SpecialType)) types.Add(special._SpecialType);

        //    Ranbow ranbow = special.GetComponent<Ranbow>();
        //    if (ranbow != null) _PreType = ranbow._PreType;

        //    special.TargetMove(s.blockEntities[pickX, pickY]._LocalPosition);
        //    special._isCombo = true;
        //    return true;
        //}

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

        public BlockType _PreType
        {
            get { return preType; }
            set { preType = value; }
        }

        public bool _IsCombo
        {
            get { return isCombo; }
            set { isCombo = value; }
        }
    }
}