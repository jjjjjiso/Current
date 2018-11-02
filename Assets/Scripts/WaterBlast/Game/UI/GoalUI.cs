using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class GoalUI : MonoBehaviour
    {
        public UISprite image = null;
        public UILabel number = null;
        public UISprite tick = null;
        
        public void GoalUISetting(Goal goal)
        {
            if(goal is CollectBlockGoal)
            {
                CollectBlockGoal block = goal as CollectBlockGoal;
                image.spriteName = block.blockType.ToString();
                number.text = block.amount.ToString();
            }
            else if(goal is CollectBlockerGoal)
            {
                CollectBlockerGoal blocker = goal as CollectBlockerGoal;
                image.spriteName = blocker.blockerType.ToString();
                number.text = blocker.amount.ToString();
            }
        }

        public void Complete(bool isComplete)
        {
            if (number != null) number.gameObject.SetActive(!isComplete);
            if (tick != null) tick.gameObject.SetActive(isComplete);
        }
    }
}