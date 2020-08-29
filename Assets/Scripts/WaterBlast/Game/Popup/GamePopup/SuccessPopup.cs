using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Manager;
using WaterBlast.Game.UI;

namespace WaterBlast.Game.Popup
{
    public class SuccessPopup : MonoBehaviour
    {
        [SerializeField] private Star[] stars     = null;
        [SerializeField] private UIGrid goalGroup = null;
        [SerializeField] private UILabel uiScore  = null;

        public void SetInfo(int score, int starCount, UIGrid group)
        {
            uiScore.text = score.ToString();

            for (int i=0; i<starCount; ++i)
            {
                stars[i].Activate();
            }

            foreach (var goal in group.GetComponentsInChildren<GoalUIElement>())
            {
                var goalObject = Instantiate(goal);

                goalObject.gameObject.layer = gameObject.layer;
                goalObject.GetComponent<UIWidget>().depth = 400;

                goalObject.image.depth = 401;
                goalObject.tickImage.depth = 402;

                goalObject.image.gameObject.layer = gameObject.layer;
                goalObject.amountText.gameObject.layer = gameObject.layer;
                goalObject.tickImage.gameObject.layer = gameObject.layer;

                goalObject.transform.SetParent(goalGroup.transform, false);
                goalObject.GetComponent<GoalUIElement>().SetCompletedTick(goal.isCompleted);
                goalObject.GetComponent<GoalUIElement>().transform.localScale = new Vector3(2f, 2f, 1f);
            }
        }
    }
}