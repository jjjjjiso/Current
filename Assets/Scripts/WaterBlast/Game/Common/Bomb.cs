﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.Common
{
    public class Bomb : Booster
    {
        public override int BonusScore() { return 30; }

        public override List<BlockEntity> Match(int x, int y)
        {
            List<BlockEntity> blocks = new List<BlockEntity>();

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

        public override List<BlockEntity> ComboMatch(int x, int y)
        {
            List<BlockEntity> blocks = Match(x, y);

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
                particles = GameMgr.G.gamePools.bombParticlesPool.GetObj();
            }
            else
            {
                particles = GameMgr.G.gamePools.bombComboParticlesPool.GetObj();
            }

            CreateParticle(particles, _LocalPosition);
        }
    }
}