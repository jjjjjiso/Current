using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Manager;
using WaterBlast.Game.UI;

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
        public virtual int BonusScore()
        {
            return 0;
        }

        public virtual List<BlockEntity> Match(int x, int y)
        {
            return new List<BlockEntity>();
        }

        public virtual List<BlockEntity> ComboMatch(int x, int y)
        {
            return new List<BlockEntity>();
        }

        public virtual void CreateParticle(bool isCombo)
        {

        }

        public void OnPressed()
        {
            if (state == State.move || state == State.booster_move) return;
            GameMgr.G.StageUpdate(this);
        }

        protected void AddBlock(List<BlockEntity> blocks, int x, int y)
        {
            Stage stage = GameMgr.G._Stage;
            if (x < 0 || x >= stage.width ||
                y < 0 || y >= stage.height) return;

            var entity = stage.blockEntities[x, y];
            if(entity != null)
            {
                if (!entity.gameObject.activeSelf) return;
                var block = entity as Block;
                if (block != null && block._BlockType == BlockType.empty) return;
                if (blocks.Contains(entity)) return;

                blocks.Add(entity);
            }
        }

        protected bool IsValidBlock(int x, int y)
        {
            GameMgr gameMgr = GameMgr.G;
            return x >= 0 && x < gameMgr._Stage.width && y >= 0 && y < gameMgr._Stage.height;
        }

        public void UpdateSprite(string strName)
        {
            if (uiSprite != null) uiSprite.spriteName = strName;
        }

        protected void CreateParticle(GameObject particles, Vector2 localPosition)
        {
            particles.transform.localPosition = localPosition;
            particles.GetComponent<BlockParticles>().Playing();
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