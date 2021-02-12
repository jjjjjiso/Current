﻿using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class GoalUIElement : MonoBehaviour
    {
        public UISprite image      = null;
        public UILabel  amountText = null;
        public UISprite tickImage  = null;
        public UISprite crossImage = null;

        public bool isCompleted { get; private set; }

        private Goal currentGoal;
        private int targetAmount;
        private int currentAmount;

        private Vector3 vec3;

        LevelBlock levelBlock;

        private void Awake()
        {
            if(tickImage != null) tickImage.gameObject.SetActive(false);
        }

        public void GoalUISetting(Goal goal)
        {
            currentGoal = goal;
            var blockGoal = goal as CollectBlockGoal;
            if (blockGoal != null)
            {
                image.spriteName = blockGoal.blockType.ToString();
                targetAmount     = blockGoal.amount;
                amountText.text  = targetAmount.ToString();

                levelBlock = new LevelBlockType() { type = blockGoal.blockType };
            }
            else
            {
                var boosterGoal = goal as CollectBoosterGoal;
                if (boosterGoal != null)
                {
                    string name = boosterGoal.boosterType.ToString();
                    switch (boosterGoal.boosterType)
                    {
                        case BoosterType.arrow:
                            int type = Random.Range((int)ArrowType.horizon, (int)ArrowType.vertical + 1);
                            name = ((ArrowType)type).ToString();
                            break;
                        case BoosterType.rainbow:
                            name = string.Format("{0}_{1}", boosterGoal.boosterType, boosterGoal.colorType);
                            break;
                    }

                    image.spriteName = name;
                    targetAmount = boosterGoal.amount;
                    amountText.text = targetAmount.ToString();

                    levelBlock = new LevelBoosterType() { type = boosterGoal.boosterType };
                }
                else
                {
                    var blockerGoal = goal as CollectBlockerGoal;
                    if (blockerGoal == null) return;
                    image.spriteName = blockerGoal.blockerType.ToString();
                    targetAmount = blockerGoal.amount;
                    amountText.text = targetAmount.ToString();

                    levelBlock = new LevelBlock() { blockerType = blockerGoal.blockerType };
                }
            }
        }

        public void UpdateTargetAmount(LevelBlock lvBlock)
        {
            if (levelBlock is LevelBoosterType) return;
            bool isChange = false;
            if (levelBlock is LevelBlockType && lvBlock is LevelBlockType)
            {   // block
                if ((levelBlock as LevelBlockType).type == (lvBlock as LevelBlockType).type)
                {
                    ++targetAmount;
                    isChange = true;
                }
            }
            else if (levelBlock is LevelBlock && lvBlock is LevelBlock)
            {   // blocker
                if ((levelBlock as LevelBlock).blockerType == (lvBlock as LevelBlock).blockerType)
                {
                    ++targetAmount;
                    isChange = true;
                }
            }
            if (isChange) amountText.text = (targetAmount - currentAmount).ToString();
        }

        public void UpdateGoal(GameState state)
        {
            if (currentAmount == targetAmount) return;

            var newAmount = 0;
            var blockGoal = currentGoal as CollectBlockGoal;
            if(blockGoal != null)
            {
                newAmount = state.collectedBlocks[blockGoal.blockType];
            }
            else
            {
                var boosterGoal = currentGoal as CollectBoosterGoal;
                if (boosterGoal != null)
                {
                    if (boosterGoal.boosterType != BoosterType.rainbow)
                        newAmount = state.collectedBoosters[boosterGoal.boosterType];
                    else
                        newAmount = state.collectedRainbows[boosterGoal.colorType];
                }
                else
                {
                    var blockerGoal = currentGoal as CollectBlockerGoal;
                    if (blockerGoal != null)
                    {
                        newAmount = state.collectedBlockers[blockerGoal.blockerType];
                    }
                }
            }

            if (newAmount == currentAmount) return;

            currentAmount = newAmount;
            if(currentAmount >= targetAmount)
            {
                currentAmount = targetAmount;
                SetCompletedTick(true);
            }

            amountText.text = (targetAmount - currentAmount).ToString();
        }

        public void SetCompletedTick(bool completed)
        {
            isCompleted = completed;
            amountText.gameObject.SetActive(false);

            vec3 = transform.localPosition;
            vec3.y = -10;
            transform.localPosition = vec3;

            if (completed)
            {
                if(tickImage != null)
                    tickImage.gameObject.SetActive(true);
            }
            else
            {
                if(crossImage != null)
                    crossImage.gameObject.SetActive(true);
            }
        }
    }
}