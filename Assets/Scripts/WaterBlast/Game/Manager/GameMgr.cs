using System;

using UnityEngine;

using FullSerializer;

using WaterBlast.System;
using WaterBlast.Game.Common;
using WaterBlast.Game.UI;

namespace WaterBlast.Game.Manager
{
    public class GameMgr : MonoSingleton<GameMgr>
    {
        public GamePool gamePools = null;
        public GoalUIElements goalUIElements = null;

        [NonSerialized]
        public Level level = null;

        [SerializeField]
        private Transform backgroundParent = null;
        [SerializeField]
        private UIAtlas backgroundAtlas= null;

        private Stage stage = null;

        private GameState gameState = null;

        private int min = 0;
        private int max = 0;

        private void Start()
        {
            gameState = new GameState();
            var serializer = new fsSerializer();
            level = FileUtils.LoadJsonFile<Level>(serializer, "Levels/" + 1/*GameDataMgr.Get().endLevelNumber*/);
            min = (int)BlockType.red;
            max = level.availableColors.Count;
            GoalSetting();
            CreateStage();
        }

        private void GoalSetting()
        {
            GoalUI obj = Resources.Load<GoalUI>("Prefabs/GoalUI");
            if (obj == null) return;
            foreach(Goal goal in level.goals)
            {
                GoalUI ui = Instantiate(obj);
                ui.GoalUISetting(goal);
                goalUIElements.Attatch(ui);

                if (goal is CollectBlockGoal)
                {
                    CollectBlockGoal block = goal as CollectBlockGoal;
                    gameState.collectedBlocks.Add(block.blockType, 0);
                }
                else if (goal is CollectBlockerGoal)
                {
                    CollectBlockerGoal blocker = goal as CollectBlockerGoal;
                    gameState.collectedBlockers.Add(blocker.blockerType, 0);
                }
            }

            goalUIElements.grid.Reposition();

            goalUIElements.SetLimit(level.limit);
        }

        private void CreateStage()
        {
            stage = Stage.Create(backgroundParent, backgroundAtlas);
        }

        public void StageUpdate(int x, int y, LevelBlock levelBlock = null)
        {
            //if (level.limit == 0) return;
            if (stage == null) return;
            if (stage.isBoosterWait) return;

            if(levelBlock != null)
            {
                if (levelBlock is LevelBlockType)
                {
                    //BlockType type = (levelBlock as LevelBlockType).type;
                    stage.NormMatch(x, y, (levelBlock as LevelBlockType));
                }
            }  
            else
            {
                stage.BoosterMatches(x, y);
            }

            goalUIElements.SetLimit(level.limit);

            if (GameEnd())
            {
                Debug.Log("Game End");
            }
        }

        public bool GameEnd()
        {
            foreach (var goal in level.goals)
            {
                if (!goal.IsComplete(gameState))
                {
                    return false;
                }
            }

            return true;
        }

        //Property
        public Stage _Stage { get { return (instance == null) ? null : instance.stage; } }
        public GameState _GameStage { get { return (instance == null) ? null : instance.gameState; } }
        public int Min { get { return min; } }
        public int Max { get { return max; } }
    }
}