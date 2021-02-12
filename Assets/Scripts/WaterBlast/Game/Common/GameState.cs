using System;
using System.Collections.Generic;

using WaterBlast.System;

namespace WaterBlast.Game.Common
{
    public class GameState
    {
        public int score;
        public Dictionary<BlockType, int> collectedBlocks = new Dictionary<BlockType, int>();
        public Dictionary<BlockerType, int> collectedBlockers = new Dictionary<BlockerType, int>();
        public Dictionary<BoosterType, int> collectedBoosters = new Dictionary<BoosterType, int>();
        public Dictionary<ColorType, int> collectedRainbows = new Dictionary<ColorType, int>();

        public void Reset()
        {
            score = 0;
            collectedBlocks.Clear();
            collectedBlockers.Clear();
            collectedBoosters.Clear();
            collectedRainbows.Clear();
            foreach (BlockType value in Enum.GetValues(typeof(BlockType)))
            {
                collectedBlocks.Add(value, 0);
            }
            foreach (BlockerType value in Enum.GetValues(typeof(BlockerType)))
            {
                collectedBlockers.Add(value, 0);
            }
            foreach (BoosterType value in Enum.GetValues(typeof(BoosterType)))
            {
                collectedBoosters.Add(value, 0);
            }
            foreach (ColorType value in Enum.GetValues(typeof(ColorType)))
            {
                if (value == ColorType.none) continue;
                collectedRainbows.Add(value, 0);
            }
        }
    }
}