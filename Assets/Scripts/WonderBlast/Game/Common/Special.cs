using System.Collections.Generic;
using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public class Special : BlockEntity
    {
        //test
        //int bbb = (int)SpecialType.left_right_arrow;
        //public bool isTest = false;

        //private void Update()
        //{
        //    if (!isTest) return;
        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        bbb++;
        //        if (bbb > (int)SpecialType.bomb)
        //            bbb = (int)SpecialType.left_right_arrow;

        //        SetSpecialType(bbb);
        //    }
        //}

        //public void OnTrue()
        //{
        //    isTest = true;
        //}

        //public void OnFale()
        //{
        //    isTest = false;
        //}
        //


        //public field

        //private field
        [SerializeField]
        protected SpecialType type = SpecialType.none;

        protected float delayTime = 0.3f;

        //default Method

        //public Method
        public virtual List<BlockDef> Match(int x, int y)
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

        protected bool GetCombo(int x, int y)
        {
            var up = new BlockDef(x, y - 1);
            var down = new BlockDef(x, y + 1);
            var left = new BlockDef(x - 1, y);
            var right = new BlockDef(x + 1, y);

            if (IsCombo(up.x, up.y) ||
                IsCombo(down.x, down.y) ||
                IsCombo(left.x, left.y) ||
                IsCombo(right.x, right.y))
            {
                return true;
            }
            return false;
        }

        protected bool IsCombo(int x, int y)
        {
            GameMgr gameMgr = GameMgr.Get();
            if (IsValidBlock(x, y) &&
                gameMgr._Stage.blockEntities[x, y] != null &&
                gameMgr._Stage.blockEntities[x, y].GetComponent<Bomb>() != null)
            {
                return true;
            }
            return false;
        }

        protected bool IsValidBlock(int x, int y)
        {
            GameMgr gameMgr = GameMgr.Get();
            return x >= 0 && x < gameMgr._Stage.width && y >= 0 && y < gameMgr._Stage.height;
        }

        //coroutine Method

        //Property
        public SpecialType _SpecialType
        {
            get { return type; }
            set { type = value; }
        }
    }
}