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
        
        public OpenInGameSetting uiSettingBtn;

        private Stage stage = null;
        private GameState gameState = null;

        [HideInInspector]
        public List<ColorType> availableColors = new List<ColorType>();

        private void Start()
        {
            //if (Screen.sleepTimeout != SleepTimeout.NeverSleep)
            //{
            //    Screen.sleepTimeout = SleepTimeout.NeverSleep;
            //    Screen.SetResolution(720, 1280, true);
            //}

            GameStart();
        }

        private void OnDestroy()
        {
            SoundMgr.G.RemoveGameEffect();
        }

        private void Reset()
        {
            UserDataMgr.G.SetUseStartItem();
            isGameEnd = false;
            gameState = new GameState();
            gameUI.progressBar.UpdateProgressBar(0);
            gameUI.SetSprite(false);
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
                    block.amount_plus = 0;
                    gameState.collectedBlocks.Add(block.blockType, 0);
                }
                else if (goal is CollectBoosterGoal)
                {
                    CollectBoosterGoal booster = goal as CollectBoosterGoal;
                    if (booster.boosterType != BoosterType.rainbow)
                        gameState.collectedBoosters.Add(booster.boosterType, 0);
                    else
                    {
                        if (booster.colorType == ColorType.none)
                            booster.colorType = (ColorType)UnityEngine.Random.Range((int)ColorType.red, (int)ColorType.purple + 1);

                        gameState.collectedRainbows.Add(booster.colorType, 0);
                    } 
                }
                else if (goal is CollectBlockerGoal)
                {
                    CollectBlockerGoal blocker = goal as CollectBlockerGoal;
                    blocker.amount_plus = 0;
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

        public void GameStart()
        {
            level = GameDataMgr.G.level;
            availableColors = level.availableColors;
            gameUI.progressBar.Init(level.score1, level.score2, level.score3);
            gameUI.SetBG(level.id);
            Reset();
            uiSettingBtn.btn.enabled = true;

            PopupGoal.Open("Goal Popup", level.goals);
        }
        
        public void StageUpdate(BlockEntity blockEntity)
        {
            if (isGameEnd) return;
            if (stage == null) return;
            if (stage.isWait) return;
            GameDataMgr gameDataMgr = GameDataMgr.G;
            
            if (blockEntity is Block || blockEntity is Blocker)
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
                    if (stage.UseItem((ItemType)index, blockEntity))
                    {
                        gameDataMgr.isUseInGameItem[index] = false;

                        if (UserDataMgr.G.availableInGameItemCount[index] > 0)
                            --UserDataMgr.G.availableInGameItemCount[index];

                        itemUIElements.UpdateInGameItem((ItemType)index);
                    }
                }
                else
                {
                    stage.NormMatch(blockEntity);
                }
            }
            else 
            {
                //booster
                if (gameDataMgr.IsUseInGameItem()) return;
                stage.BoosterMatches(blockEntity);
            }

            //GameEnd();
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

        public void UpdateGoalUI(LevelBlock lvBlock)
        {
            gameUI.goalUI.UpdateTargetAmount(lvBlock);
        }

        public void GameEnd()
        {
            if (isGameEnd) return;

            //success ending
            if (currentLimit >= 0 && IsGameEnd())
            {
                isGameEnd = true;
                if (GameDataMgr.G.isTestScene)
                {
                    StartCoroutine(Co_TestReset());
                    return;
                }

                gameUI.goalUI.UpdateGoalUI(gameState);
                gameUI.SetSprite(true);
                
                StartCoroutine(Co_Success());
            }

            //faild ending
            if (currentLimit == 0 && !isGameEnd)
            {
                isGameEnd = true;
                if (GameDataMgr.G.isTestScene)
                {
                    StartCoroutine(Co_TestReset());
                    return;
                }

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
        IEnumerator Co_TestReset()
        {
            DebugX.LogError("SCORE = " + gameState.score);
            yield return new WaitForSeconds(0.5f);
            GameDataMgr.G.UpdateLevel();
            yield return new WaitForSeconds(0.5f);
            GameStart();
            isGameEnd = false;
        }

        IEnumerator Co_Success()
        {
            uiSettingBtn.btn.enabled = false;
            yield return new WaitForSeconds(0.5f);

            SoundMgr.G.GameEffectPlay(EffectSound.win);
            textAnim.SetTrigger("ClearOn");
            yield return new WaitForSeconds(0.8f);

            stage.FinalFinale(currentLimit);
            while (stage.isFinale) yield return null;
            yield return new WaitForSeconds(0.5f);

            string level = string.Format("LEVEL {0}", (GameDataMgr.G.endLevel).ToString());
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/GamePopup", "SuccessPopup", level, null, "CONTINUE");
            temp.GetComponent<GamePopup>().OnPopup(GamePopupState.success);
            temp.GetComponentInChildren<SuccessPopup>().SetInfo(gameState.score, gameUI.progressBar.GetStars(), gameUI.goalUI.group);

            temp.onConfirm += () =>
            {
                GoLooby();
            };

            temp.onExit += () =>
            {
                GoLooby();
            };

            ++GameDataMgr.G.endLevel;
            GameDataMgr.G.UpdateLevel();
        }

        IEnumerator Co_Failed()
        {
            uiSettingBtn.btn.enabled = false;
            while (stage.isWait) yield return null;
            while (stage.IsMoving(State.move)) yield return null;

            yield return new WaitForSeconds(.5f);

            SoundMgr.G.GameEffectPlay(EffectSound.lose);
            textAnim.SetTrigger("FailedOn");
            yield return new WaitForSeconds(0.8f);

            PopupConfirm noMoves = PopupConfirm.Open("Prefabs/Popup/NoMovesPopup", "Failed Moves Popup", null, null, "PLAY ON", false);
            PopupNoMoves popupNoMoves = noMoves.GetComponent<PopupNoMoves>();
            if (popupNoMoves != null) popupNoMoves.SetCoin();

            noMoves.onConfirm += () =>
            {
                int useCoin = 100;
                if (UserDataMgr.G.IsCoins(useCoin))
                {
                    currentLimit = 5;
                    UpdateLimitCount();
                    stage.BlockIconSetting();
                    isGameEnd = false;
                    uiSettingBtn.btn.enabled = true;
                    UserDataMgr.G.CoinsUsed(useCoin);
                    if (popupNoMoves != null) popupNoMoves.SetCoin();
                    noMoves.Close();
                }
                else
                {
                    PopupMgr.G.ShowAdsPopup(null, "Watch ads and get rewarded.", "OK", () =>
                    {
                        AdsMgr.G.fncSuccess = () =>
                        {
                            if (popupNoMoves != null) popupNoMoves.SetCoin();
                        };
                    });
                }
            };

            noMoves.onExit += () =>
            {
                noMoves.Close();

                Failed();
            };
        }

        public void Failed()
        {
            UserDataMgr.G.UpdateLife();

            string levelNumber = string.Format("LEVEL {0}", GameDataMgr.G.endLevel.ToString());
            PopupConfirm temp = PopupConfirm.Open("Prefabs/Popup/GamePopup", "FailedPopup", levelNumber, null, "TRY AGAIN", false);

            temp.GetComponent<GamePopup>().OnPopup(GamePopupState.failed);
            GamePopupItemGroup item = temp.GetComponentInChildren<GamePopupItemGroup>();
            if (item != null)
            {
                item.BoosterItemSetting();
                item.gameObject.GetComponent<UIWidget>().bottomAnchor.absolute = -4;
            }

            temp.onConfirm += () =>
            {
                if (UserDataMgr.G.life > 0)
                {
                    temp.Close();
                    Reset();
                    PopupGoal.Open("Goal Popup", level.goals);
                }
                else
                {
                    int count = 1;
                    if (UserDataMgr.G.IsItemCoins(count))
                    {
                        PopupMgr.G.ShowItemPopup("Life Item Popup", "LIFE", "You can play the game with life!", "BUY",
                                                 "life_icon", count, GameDataMgr.G.itemCost, () =>
                                                 {
                                                     UserDataMgr.G.CoinsUsed(GameDataMgr.G.itemCost * count);
                                                     UserDataMgr.G.life += count;
                                                     UserDataMgr.G.SetTimeToNextLife();
                                                 }, true);
                    }
                    else
                    {
                        PopupMgr.G.ShowAdsPopup(null, "Watch ads and get rewarded.", "OK");
                    }
                } 
            };

            temp.onExit += () =>
            {
                GoLooby();
            };
        }

        public void GoLooby()
        {
            sceneFade.delayTime = 0.15f;
            sceneFade.fadeTime = 0.3f;
            sceneFade.OnPressed();
        }

        //Property
        public Stage _Stage { get { return (instance == null) ? null : instance.stage; } }
        public GameState _GameState { get { return (instance == null) ? null : instance.gameState; } }
    }
}