using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Common;
using WaterBlast.Game.Manager;

namespace WaterBlast.Game.UI
{
    public class GameUI : MonoBehaviour
    {
        public UILabel uiLimitText = null;
        public GoalUI goalUI = null;
        public ProgressBar progressBar = null;
        public UISprite sprite = null;
        public UISprite sprBG = null;

        public void SetGoals(List<Goal> goals, GameObject prefab)
        {
            var childrenToRemove = goalUI.group.GetComponentsInChildren<GoalUIElement>();
            foreach (var child in childrenToRemove)
            {
                Destroy(child.gameObject);
            }
            goalUI.group.transform.DetachChildren();

            foreach (var goal in goals)
            {
                if (!(goal is CollectBlockGoal) && !(goal is CollectBlockerGoal)) continue;
                var goalObj = Instantiate(prefab);
                goalObj.transform.SetParent(goalUI.group.transform, false);
                goalObj.GetComponent<GoalUIElement>().GoalUISetting(goal);
            }
            
            goalUI.group.Reposition();
        }

        public void UpdateLimitText(string limit)
        {
            uiLimitText.text = limit;
        }

        public void SetLimitTextColor(Color col)
        {
            uiLimitText.color = col;
        }

        public void SetSprite(bool isSuccess)
        {
            if (sprite != null) sprite.spriteName = isSuccess ? "bitmuri2" : "bitmuri";
        }

        public void SetBG(int stageLv)
        {
            if (sprBG != null) sprBG.spriteName = stageLv < 50 ? "background_ingame_1" : "background_ingame_2";
        }
    }
}