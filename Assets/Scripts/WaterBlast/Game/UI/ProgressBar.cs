using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField]
        private Star star1 = null;
        [SerializeField]
        private Star star2 = null;
        [SerializeField]
        private Star star3 = null;

        //private UIProgressBar uiProgressbar = null;
        public UISprite fill;

        private int starScore1 = 0;
        private int starScore2 = 0;
        private int starScore3 = 0;

        /*private void Awake()
        {
            uiProgressbar = gameObject.GetComponent<UIProgressBar>();
        }*/

        public void Init(int starScore1, int starScore2, int starScore3)
        {
            fill.fillAmount = 0;

            this.starScore1 = starScore1;
            this.starScore2 = starScore2;
            this.starScore3 = starScore3;
            
            int width = fill.width;//uiProgressbar.foregroundWidget.width;
            star1.transform.localPosition = star1.transform.localPosition +
                                            new Vector3(width * (GetProgressValue(starScore1) / 100f) - 10f, 0, 0);
            star2.transform.localPosition = star2.transform.localPosition +
                                            new Vector3(width * (GetProgressValue(starScore2) / 100f) - 10f, 0, 0);
            star3.transform.localPosition = star3.transform.localPosition +
                                            new Vector3(width * (GetProgressValue(starScore3) / 100f) - 10f, 0, 0);
        }

        public void UpdateProgressBar(int score)
        {
            fill.fillAmount = GetProgressValue(score) / 100f;

            if(starScore1 <= score)
            {
                star1.Activate();
            }
            if (starScore2 <= score)
            {
                star2.Activate();
            }
            if (starScore3 <= score)
            {
                star3.Activate();
            }
        }

        private int GetProgressValue(int value)
        {
            const int oldMin = 0;
            int oldMax = starScore3;
            const int newMin = 0;
            const int newMax = 100;
            int oldRange = oldMax - oldMin;
            const int newRange = newMax - newMin;
            int newValue = (((value - oldMin) * newRange) / oldRange) + newMin;
            return newValue;
        }

        public int GetStars()
        {
            int count = 0;
            if (star1.isActive) ++count;
            if (star2.isActive) ++count;
            if (star3.isActive) ++count;

            return count;
        }
    }
}