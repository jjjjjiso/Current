using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class ColorBlock : Block
    {
        //public override void Show()
        //{
        //    //SetRandomColor(GameMgr.G.Min, GameMgr.G.Max);
        //    base.Show();
        //}

        public void OnPressed()
        {
            if (state == State.move || state == State.booster_move) return;
            GameMgr.G.StageUpdate(_X, _Y, new LevelBlockType() { type = this.type});
        }

        public bool ColorMatch(BlockType type)
        {
            if (this.type != type) return false;
            state = State.wait;
            return true;
        }

        public bool ColorComparison(BlockType type)
        {
            if (this.type == type) return true;
            return false;
        }

        //public void SetBlockType(BlockType type)
        //{
        //    this.type = type;
        //    UpdateSprite();
        //}

        //public void SetBlockType(int type)
        //{
        //    this.type = (BlockType)type;
        //    UpdateSprite();
        //}

        //public void SetRandomColor()
        //{
        //    int random = Random.Range((int)BlockType.red, (int)BlockType.purple + 1);
        //    SetBlockType(random);
        //}

        //public void SetRandomColor(int min, int max)
        //{
        //    int random = Random.Range(min, max + 1);
        //    SetBlockType(random);
        //}

        //public void SetSprite(string strName)
        //{
        //    UpdateSprite(strName);
        //}

        //private void UpdateSprite()
        //{
        //    string strName = type.ToString();
        //    if (uiSprite != null) uiSprite.spriteName = strName;
        //}

        //private void UpdateSprite(string strName)
        //{
        //    if (uiSprite != null) uiSprite.spriteName = strName;
        //}
    }
}