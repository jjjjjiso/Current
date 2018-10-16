using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class LimitUI : MonoBehaviour
    {
        [SerializeField]
        private UILabel limit = null;

        public void SetLimit(int limit)
        {
            this.limit.text = limit.ToString();
        }
    }
}