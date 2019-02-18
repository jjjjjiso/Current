using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using FullSerializer;

using WaterBlast.System;
using WaterBlast.Game.Common;
using WaterBlast.Game.UI;
using WaterBlast.Game.Popup;

namespace WaterBlast.Game.Manager
{
    public class GameMgr : MonoSingleton<GameMgr>
    {
        public SceneFadeInOut sceneFade = null;
        public GamePool gamePools = null;
        public GameUI gameUI = null;
        public ItemUIElements itemUIElements = null;

        public GameObject[] gameItemAnim = null;

        [NonSerialized]
        public Level level = null;

        private int currentLimit;

        public bool isGameEnd = false;

        [SerializeField]
        private Transform backgroundParent = null;
        [SerializeField]
        private Sprite backgroundSprite = null;

        private Stage stage = null;

        private GameState gameState = null;

        public List<ColorType> availableColors = new List<ColorType>();

        private void Start()
        {
            //if (Screen.sleepTimeout != SleepTimeout.NeverSleep)
            //{
            //    Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //    Screen.SetResolution(720, 1280, true);
            //}

            
            var serializer = new fsSerializer();
            level = FileUtils.LoadJsonFile<Level>(serializer, "Levels/" + 1/*GameDataMgr.G.endLevelNumber*/);
            availableColors = level.availableColors;
            gameUI.progressBar.Init(level.score1, level.score2, level.score3);
            Reset();
        }

        private void Reset()
        {
            isGameEnd = false;
            gameState = new GameState();
            gameUI.progressBar.UpdateProgressBar(0);
            GoalSetting();
            ItemSetting();
            CreateStage();
        }

        private void GoalSetting()
        {
            foreach(Goal goal in level.goals)
            {
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

            currentLimit = level.limit;
            UpdateLimitCount();

            GameObject obj = Resources.Load<GoalUIElement>("Prefabs/GoalUIElement").gameObject;
            gameUI.SetGoals(level.goals, obj);
        }

        private void ItemSetting()
        {
            foreach (KeyValuePair<ItemType, bool> keyValue in level.availableInGameItem)
            {
                itemUIElements.ItemSetting(keyValue.Key, keyValue.Value);
            }
        }

        private void CreateStage()
        {
            if (stage != null)
            {
                stage.Reset();
                Destroy(stage.gameObject);
            }
            stage = Stage.Create(backgroundParent, backgroundSprite, level.width, level.height);
        }

        public void StageUpdate(int x, int y, LevelBlock levelBlock = null)
        {
            if (isGameEnd) return;
            if (stage == null) return;
            if (stage.isWait) return;
            GameDataMgr gameDataMgr = GameDataMgr.G;

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

                        if (UserDataMgr.G.availableInGameItemCount[index] > 0)
                            --UserDataMgr.G.availableInGameItemCount[index];

                        stage.UseItem((ItemType)index, x, y);
                        itemUIElements.UpdateInGameItem((ItemType)index);
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

            

            GameEnd();
        }

        public void ReduceTheNumberOfLimitCount()
        {
            --currentLimit;
            if (currentLimit < 0)
            {
                currentLimit = 0;
            }
            
            UpdateLimitCount();
        }

        public void UpdateLimitCount()
        {
            gameUI.UpdateLimitText(currentLimit.ToString());

            if (currentLimit > 5)
                gameUI.SetLimitTextColor(Color.white);
            else
                gameUI.SetLimitTextColor(Color.red);
        }

        public void UpdateGoalUI()
        {
            gameUI.goalUI.UpdateGoalUI(gameState);
        }

        public void GameEnd()
        {
            if (isGameEnd) return;
            if (IsGameEnd())
            {
                isGameEnd = true;
                gameUI.goalUI.UpdateGoalUI(gameState);
                stage.FinalFinale(currentLimit);

                if (currentLimit >= 0)
                {
                    //success ending
                    ++GameDataMgr.G.endLevel;
                    StartCoroutine(Co_Success());
                }
            }

            //faild ending
            if (currentLimit == 0 && !isGameEnd)
            {
                isGameEnd = true;
                StartCoroutine(Co_Failed());
            }
        }

        public bool IsGameEnd()
        {
            foreach (var goal in level.goals)
            {
                if (!goal.IsComplete(gameState)) return false;
            }

            return true;
        }

        IEnumerator Co_Success()
        {
            while (stage.isFinale) yield return null;
            yield return new WaitForSecondsRealtime(.5f);

            string level = string.Format("Level {0}", (GameDataMgr.G.endLevel-1).ToString());
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/GamePopup", "SuccessPopup", level, null, "Continue");
            temp.GetComponent<GamePopup>().OnPopup(GamePopupState.success);
            temp.GetComponentInChildren<SuccessPopup>().SetInfo(gameState.score, gameUI.progressBar.GetStars(), gameUI.goalUI.group);

            temp.onConfirm += () =>
            {
                sceneFade.delayTime = 0.15f;
                sceneFade.fadeTime = 0.3f;
                sceneFade.OnPressed();
            };

            temp.onEixt += () =>
            {
                sceneFade.delayTime = 0.15f;
                sceneFade.fadeTime = 0.3f;
                sceneFade.OnPressed();
            };
        }

        IEnumerator Co_Failed()
        {
            yield return new WaitForSecondsRealtime(.5f);

            PopupConfirm noMoves = PopupConfirm.Open("Prefabs/Popup/NoMovesPopup", "Failed Moves Popup", null, null, "Play On");
            noMoves.onConfirm += () =>
            {
                currentLimit = 5;
                UpdateLimitCount();
                isGameEnd = false;
            };

            noMoves.onEixt += () =>
            {
                noMoves.Close();

                Failed();
            };
        }

        public void Failed()
        {
            string level = string.Format("Level {0}", GameDataMgr.G.endLevel.ToString());
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/GamePopup", "FailedPopup", level, null, "Try Again");

            temp.GetComponent<GamePopup>().OnPopup(GamePopupState.failed);
            GamePopupItemCount item = temp.GetComponentInChildren<GamePopupItemCount>();
            if (item != null)
            {
                item.SetItemCount(UserDataMgr.G.availableStartItemCount);
                item.gameObject.GetComponent<UIWidget>().topAnchor.absolute = -150;
            }

            temp.onConfirm += () =>
            {
                Reset();
            };

            temp.onEixt += () =>
            {
                sceneFade.delayTime = 0.15f;
                sceneFade.fadeTime = 0.3f;
                sceneFade.OnPressed();
            };
        }

        //Property
        public Stage _Stage { get { return (instance == null) ? null : instance.stage; } }
        public GameState _GameState { get { return (instance == null) ? null : instance.gameState; } }
    }
}