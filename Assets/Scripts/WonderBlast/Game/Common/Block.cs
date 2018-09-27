using UnityEngine;

using WonderBlast.Game.Manager;

namespace WonderBlast.Game.Common
{
    public class Block : BlockEntity
    {
        //public field

        //private field
        [SerializeField]
        private BlockType type = BlockType.none;

        //default Method

        //public Method
        public override void Show()
        {
            SetRandomColor(GameMgr.Get().Min, GameMgr.Get().Max);
            base.Show();
        }

        public void OnPressed()
        {
            if (state == State.move || state == State.special_move) return;
            GameMgr.Get().StageUpdate(_X, _Y, type);
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

        public void SetBlockType(BlockType type)
        {
            this.type = type;
            UpdateSprite();
        }

        public void SetBlockType(int type)
        {
            this.type = (BlockType)type;
            UpdateSprite();
        }

        public void SetRandomColor()
        {
            int random = Random.Range((int)BlockType.red, (int)BlockType.purple + 1);
            SetBlockType(random);
        }

        public void SetRandomColor(int min, int max)
        {
            int random = Random.Range(min, max + 1);
            SetBlockType(random);
        }

        public void SetSprite(string strName)
        {
            UpdateSprite(strName);
        }

        //private Method
        private void UpdateSprite()
        {
            string strName = type.ToString();
            if (uiSprite != null) uiSprite.spriteName = strName;
        }

        private void UpdateSprite(string strName)
        {
            if (uiSprite != null) uiSprite.spriteName = strName;
        }

        //coroutine Method
        
        //Property
        public BlockType _BlockType
        {
            get { return type; }
            set { type = value; }
        }
    }
}