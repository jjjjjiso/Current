using System;
using System.Collections.Generic;

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
        public ProgressBar progressBar = null;
        public ItemUIElements itemUIElements = null;

        public GameObject[] gameItemAnim = null;

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
            //if (Screen.sleepTimeout != SleepTimeout.NeverSleep)
            //{
            //    Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //    Screen.SetResolution(720, 1280, true);
            //}

            gameState = new GameState();
            var serializer = new fsSerializer();
            level = FileUtils.LoadJsonFile<Level>(serializer, "Levels/" + 1/*GameDataMgr.Get().endLevelNumber*/);
            min = (int)BlockType.red;
            max = level.availableColors.Count;
            progressBar.Init(level.score1, level.score2, level.score3);
            GoalSetting();
            ItemSetting();
            CreateStage();
        }

        //임시방편.
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if (UNITY_ANDROID && UNITY_EDITOR)
                UnityEditor.EditorApplication.isPlaying = false;

#elif (UNITY_ANDROID)
                    Application.Quit();
#endif
            }
        }

        private void GoalSetting()
        {
            //GoalUI obj = Resources.Load<GoalUI>("Prefabs/GoalUI");
            //if (obj == null) return;
            foreach(Goal goal in level.goals)
            {
                //GoalUI ui = Instantiate(obj);
                //ui.GoalUISetting(goal);
                //goalUIElements.Attatch(block.blockType, ui);

                goalUIElements.CreateGoalUI(goal);

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

        private void ItemSetting()
        {
            foreach (KeyValuePair<ItemType, bool> keyValue in level.availableItem)
            {
                itemUIElements.ItemSetting(keyValue.Key, keyValue.Value);
            }
        }

        private void CreateStage()
        {
            stage = Stage.Create(backgroundParent, backgroundAtlas, level.width, level.height);
        }

        public void StageUpdate(int x, int y, LevelBlock levelBlock = null)
        {
            //if (level.limit == 0) return;
            if (stage == null) return;
            if (stage.isWait) return;
            GameDataMgr gameDataMgr = GameDataMgr.Get();

            if(levelBlock != null)
            {
                if (levelBlock is LevelBlockType)
                {
                    int index = -1;
                    
                    if (gameDataMgr.isUseItem[(int)ItemType.hammer])
                    {
                        index = (int)ItemType.hammer;
                    }
                    else if(gameDataMgr.isUseItem[(int)ItemType.horizon])
                    {
                        index = (int)ItemType.horizon;
                    }
                    else if (gameDataMgr.isUseItem[(int)ItemType.vertical])
                    {
                        index = (int)ItemType.vertical;
                    }
                    else if (gameDataMgr.isUseItem[(int)ItemType.mix])
                    {
                        index = (int)ItemType.mix;
                    }

                    if(index != -1)
                    {
                        gameDataMgr.isUseItem[index] = false;

                        if (gameDataMgr.availableItemCount[index] > 0)
                            --gameDataMgr.availableItemCount[index];

                        stage.UseItem((ItemType)index, x, y);
                        itemUIElements.ItemReset((ItemType)index);
                    }
                    else
                    {
                        stage.NormMatch(x, y, (levelBlock as LevelBlockType));
                    }
                }
                //blocker type 일때 여기서
            }  
            else
            {
                if (gameDataMgr.IsUseItem()) return;
                stage.BoosterMatches(x, y);
            }

            goalUIElements.SetLimit(level.limit);

            GameEnd();
        }

        public void GameEnd()
        {
            if (IsGameEnd())
            {
                //Debug.Log("Game End");
            }
        }

        public bool IsGameEnd()
        {
            bool isGameEnd = false;
            foreach (var goal in level.goals)
            {
                isGameEnd = goal.IsComplete(gameState);
                goalUIElements.UpdateGoalUI(goal, isGameEnd);
            }

            return isGameEnd;
        }

        //Property
        public Stage _Stage { get { return (instance == null) ? null : instance.stage; } }
        public GameState _GameState { get { return (instance == null) ? null : instance.gameState; } }
        public int Min { get { return min; } }
        public int Max { get { return max; } }
    }
}