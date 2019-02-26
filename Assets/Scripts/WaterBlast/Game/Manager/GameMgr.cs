using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

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
        private Animator textAnim = null; // Clear or Failed Text Animation
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
            
            level = GameDataMgr.G.level;
            availableColors = level.availableColors;
            gameUI.progressBar.Init(level.score1, level.score2, level.score3);
            Reset();

            PopupGoal.Open("Goal Popup", level.goals);
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
        
        public void StageUpdate(BlockEntity blockEntity)
        {
            if (isGameEnd) return;
            if (stage == null) return;
            if (stage.isWait) return;
            GameDataMgr gameDataMgr = GameDataMgr.G;

            Block block = blockEntity as Block;
            if (block != null)
            {
                int index = -1;
                if (gameDataMgr.isUseInGameItem[(int)ItemType.hammer])
                {
                    index = (int)ItemType.hammer;
                }
                else if (gameDataMgr.isUseInGameItem[(int)ItemType.horizon])
                {
                    index = (int)ItemType.horizon;
                }
                else if (gameDataMgr.isUseInGameItem[(int)ItemType.vertical])
                {
                    index = (int)ItemType.vertical;
                }
                else if (gameDataMgr.isUseInGameItem[(int)ItemType.mix])
                {
                    index = (int)ItemType.mix;
                }

                if (index != -1)
                {
                    gameDataMgr.isUseInGameItem[index] = false;

                    if (UserDataMgr.G.availableInGameItemCount[index] > 0)
                        --UserDataMgr.G.availableInGameItemCount[index];

                    stage.UseItem((ItemType)index, blockEntity);
                    itemUIElements.UpdateInGameItem((ItemType)index);
                }
                else
                {
                    stage.NormMatch(blockEntity);
                }
            }
            else
            {
                //blocker type 일때 여기서

                //booster
                if (gameDataMgr.IsUseInGameItem()) return;
                stage.BoosterMatches(blockEntity);
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

            //success ending
            if (currentLimit >= 0 && IsGameEnd())
            {
                isGameEnd = true;
                gameUI.goalUI.UpdateGoalUI(gameState);
                    
                ++GameDataMgr.G.endLevel;
                GameDataMgr.G.UpdateLevel();
                StartCoroutine(Co_Success());
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
            yield return new WaitForSecondsRealtime(.5f);

            textAnim.SetTrigger("ClearOn");
            yield return new WaitForSecondsRealtime(0.8f);

            stage.FinalFinale(currentLimit);
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
            while (stage.isWait) yield return null;
            while (stage.IsMoving(State.move)) yield return null;

            yield return new WaitForSecondsRealtime(.5f);

            textAnim.SetTrigger("FailedOn");
            yield return new WaitForSecondsRealtime(0.8f);

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
            string levelNumber = string.Format("Level {0}", GameDataMgr.G.endLevel.ToString());
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/GamePopup", "FailedPopup", levelNumber, null, "Try Again");

            temp.GetComponent<GamePopup>().OnPopup(GamePopupState.failed);
            GamePopupItemGroup item = temp.GetComponentInChildren<GamePopupItemGroup>();
            if (item != null)
            {
                item.BoosterItemSetting();
                item.gameObject.GetComponent<UIWidget>().topAnchor.absolute = -150;
            }

            temp.onConfirm += () =>
            {
                temp.Close();
                Reset();
                PopupGoal.Open("Goal Popup", level.goals);
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