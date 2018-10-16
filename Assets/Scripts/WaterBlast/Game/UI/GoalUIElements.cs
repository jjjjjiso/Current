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

        //public List<GoalUI> goalUIList = new List<GoalUI>();

        public void Attatch(GoalUI goalUI)
        {
            goalUI.transform.parent = grid.transform;
            goalUI.transform.Reset();

            //if (!goalUIList.Contains(goalUI)) goalUIList.Add(goalUI);
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
    }
}