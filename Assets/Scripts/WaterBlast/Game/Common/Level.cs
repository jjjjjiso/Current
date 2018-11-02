using System.Collections.Generic;

namespace WaterBlast.Game.Common
{
    public class Level
    {
        public int id;

        public int width;
        public int height;
        
        public List<LevelBlock> blocks = new List<LevelBlock>();

        public int limit;

        public List<Goal> goals = new List<Goal>();
        public List<ColorType> availableColors = new List<ColorType>();

        public int score1;
        public int score2;
        public int score3;

        public bool awardBoostersWithRemainingMoves;
        public BoosterType awardedBoosterType;

        public int collectableChance;

        public Dictionary<ItemType, bool> availableItem = new Dictionary<ItemType, bool>();
    }
}
