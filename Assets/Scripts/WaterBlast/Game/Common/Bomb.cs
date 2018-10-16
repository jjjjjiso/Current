using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class Bomb : Booster
    {
        public override List<BlockDef> Match(int x, int y)
        {
            List<BlockDef> blocks = new List<BlockDef>();

            AddBlock(blocks, x, y);
            AddBlock(blocks, x - 1, y);
            AddBlock(blocks, x + 1, y);
            AddBlock(blocks, x, y - 1);
            AddBlock(blocks, x, y + 1);
            AddBlock(blocks, x - 1, y - 1);
            AddBlock(blocks, x + 1, y - 1);
            AddBlock(blocks, x - 1, y + 1);
            AddBlock(blocks, x + 1, y + 1);

            return blocks;
        }

        public override List<BlockDef> ComboMatch(int x, int y)
        {
            List<BlockDef> blocks = Match(x, y);

            AddBlock(blocks, x, y - 2);
            AddBlock(blocks, x, y + 2);
            AddBlock(blocks, x - 1, y - 2);
            AddBlock(blocks, x - 1, y + 2);
            AddBlock(blocks, x - 2, y - 2);
            AddBlock(blocks, x - 2, y - 1);
            AddBlock(blocks, x - 2, y);
            AddBlock(blocks, x - 2, y + 1);
            AddBlock(blocks, x - 2, y + 2);
            AddBlock(blocks, x + 1, y - 2);
            AddBlock(blocks, x + 1, y + 2);
            AddBlock(blocks, x + 2, y - 2);
            AddBlock(blocks, x + 2, y - 1);
            AddBlock(blocks, x + 2, y);
            AddBlock(blocks, x + 2, y + 1);
            AddBlock(blocks, x + 2, y + 2);

            return blocks;
        }

        public override void CreateParticle(bool isCombo)
        {
            GameObject particles = null;

            if (!isCombo)
            {
                particles = GameMgr.Get().gamePools.bombParticlesPool.GetObject();
            }
            else
            {
                particles = GameMgr.Get().gamePools.bombComboParticlesPool.GetObject();
            }

            CreateParticle(particles, _LocalPosition);
        }
    }
}