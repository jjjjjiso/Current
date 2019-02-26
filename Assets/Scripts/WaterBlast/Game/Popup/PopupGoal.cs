using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Common;
using WaterBlast.Game.UI;

namespace WaterBlast.Game.Popup
{
    public class PopupGoal : Popup
    {
        [SerializeField] private UIGrid goalParent = null;
        [SerializeField] private GameObject goalPrefab = null;

        private const string path = "Prefabs/Popup/GoalPopup";

        static public PopupGoal Open(string ID, List<Goal> goals)
        {
            return Open(path, ID, goals);
        }

        static public PopupGoal Open(string path, string ID, List<Goal> goals)
        {
            PopupGoal temp = Popup.Create<PopupGoal>(path, ID);

            foreach (var goal in goals)
            {
                if (!(goal is CollectBlockGoal) && !(goal is CollectBlockerGoal)) continue;
                var goalObj = Instantiate(temp.goalPrefab);
                goalObj.transform.SetParent(temp.goalParent.transform, false);
                goalObj.GetComponent<GoalUIElement>().GoalUISetting(goal);
            }

            temp.goalParent.Reposition();
            temp.StartCoroutine(temp.Co_AutoClose());

            return temp;
        }

        IEnumerator Co_AutoClose()
        {
            yield return new WaitForSecondsRealtime(1.2f);
            Close();
        }
    }
}
