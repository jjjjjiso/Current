using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaterBlast.Game.UI
{
    public class Star : MonoBehaviour
    {
        [SerializeField]
        private GameObject yellowStar = null;
        [SerializeField]
        private GameObject grayStart = null;

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
            yellowStar.SetActive(isActive);
            grayStart.SetActive(!isActive);
        }
    }
}