using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.UI
{
    public class Star : MonoBehaviour
    {
        [SerializeField]
        private UISprite uiStarImg = null;

        public bool isActive = false;

        private void Awake()
        {
            Activate(false);
        }

        public void Activate()
        {
            Activate(true);
        }

        private void Activate(bool isActive)
        {
            this.isActive = isActive;

            if (isActive)
                uiStarImg.spriteName = "StarYellow";
            else
                uiStarImg.spriteName = "StarGrey";
        }
    }
}