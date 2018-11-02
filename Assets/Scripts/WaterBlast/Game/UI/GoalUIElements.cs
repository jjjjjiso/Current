using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class GoalUIElements : MonoBehaviour
    {
        public UIGrid grid = null;
        public LimitUI limitUI = null;

        public Dictionary<Goal, GoalUI> goalUIList = new Dictionary<Goal, GoalUI>();

        public void Attatch(BlockType type, GoalUI goalUI)
        {
            goalUI.transform.parent = grid.transform;
            goalUI.transform.Reset();

            //if (!goalUIList.ContainsKey(type)) goalUIList[type] = goalUI;
        }

        public void CreateGoalUI(Goal goal)
        {
            GoalUI obj = Resources.Load<GoalUI>("Prefabs/GoalUI");
            if (obj == null) return;
            GoalUI goalUI = Instantiate(obj);
            goalUI.GoalUISetting(goal);

            goalUI.transform.parent = grid.transform;
            goalUI.transform.Reset();

            if (goal is CollectBlockGoal)
            {
                CollectBlockGoal block = goal as CollectBlockGoal;
                if (!goalUIList.ContainsKey(block)) goalUIList[block] = goalUI;
            }
            else if (goal is CollectBlockerGoal)
            {
                CollectBlockerGoal blocker = goal as CollectBlockerGoal;
                if (!goalUIList.ContainsKey(blocker)) goalUIList[blocker] = goalUI;
            }
        }

        public void ChildDestroy()
        {
            while(transform.childCount > 0)
            {
                Destroy(transform.GetChild(0));
            }
        }

        public void SetLimit(int limit)
        {
            limitUI.SetLimit(limit);
        }

        public void UpdateGoalUI(Goal goal, bool isComplete)
        {
            goalUIList[goal].Complete(isComplete);
        }
    }
}