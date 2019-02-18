using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public enum ArrowType
    {
        horizon,
        vertical,
    }

    public class ArrowBomb : Booster
    {
        protected ArrowType arrowType = ArrowType.horizon;

        public override List<BlockDef> Match(int x, int y)
        {
            Stage s = GameMgr.G._Stage;
            List<BlockDef> blocks = new List<BlockDef>();

            switch (arrowType)
            {
                case ArrowType.horizon:
                    {
                        for(int ix = 0; ix < s.width; ++ix)
                        {
                            AddBlock(blocks, ix, y);
                        }
                    }
                    break;
                case ArrowType.vertical:
                    {
                        for (int iy = 0; iy < s.height; ++iy)
                        {
                            AddBlock(blocks, x, iy);
                        }
                    }
                    break;
            }

            return blocks;
        }

        public override List<BlockDef> ComboMatch(int x, int y)
        {
            Stage s = GameMgr.G._Stage;
            List<BlockDef> blocks = new List<BlockDef>();

            for (int ix = 0; ix < s.width; ++ix)
            {
                AddBlock(blocks, ix, y);
            }
            for (int iy = 0; iy < s.height; ++iy)
            {
                AddBlock(blocks, x, iy);
            }

            return blocks;
        }

        public override void CreateParticle(bool isCombo)
        {
            GameObject particles = null;
            Vector2 localPosition = Vector2.zero;

            if(!isCombo)
            {
                switch (arrowType)
                {
                    case ArrowType.horizon:
                        {
                            particles = GameMgr.G.gamePools.lineHorizontalParticlesPool.GetObj();
                            localPosition = particles.transform.localPosition;
                            localPosition.y = _LocalPosition.y;
                        }
                        break;
                    case ArrowType.vertical:
                        {
                            particles = GameMgr.G.gamePools.lineVerticalParticlesPool.GetObj();
                            localPosition = particles.transform.localPosition;
                            localPosition.x = _LocalPosition.x;
                        }
                        break;
                }

                CreateParticle(particles, localPosition);
            }
            else
            {
                particles = GameMgr.G.gamePools.lineHorizontalParticlesPool.GetObj();
                localPosition = particles.transform.localPosition;
                localPosition.y = _LocalPosition.y;
                CreateParticle(particles, localPosition);

                particles = GameMgr.G.gamePools.lineVerticalParticlesPool.GetObj();
                localPosition = particles.transform.localPosition;
                localPosition.x = _LocalPosition.x;
                CreateParticle(particles, localPosition);
            }
        }

        public void UpdateSprite(ArrowType type)
        {
            this.arrowType = type;
            UpdateSprite(arrowType.ToString());
        }

        public void UpdateSprite(int type)
        {
            this.arrowType = (ArrowType)type;
            UpdateSprite(arrowType.ToString());
        }
        
        public ArrowType _ArrowType
        {
            get { return arrowType; }
            set { arrowType = value; }
        }
    }
}