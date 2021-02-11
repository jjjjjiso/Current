using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class GoalUI : MonoBehaviour
    {
        public UIGrid group = null;

        public void UpdateGoalUI(GameState gameState)
        {
            foreach (var element in group.GetComponentsInChildren<GoalUIElement>())
            {
                element.UpdateGoal(gameState);
            }
        }

        public void UpdateTargetAmount(LevelBlock lvBlock)
        {
            foreach (var element in group.GetComponentsInChildren<GoalUIElement>())
            {
                element.UpdateTargetAmount(lvBlock);
            }
        }
    }
}