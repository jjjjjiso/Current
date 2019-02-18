using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class ItemClicked : MonoBehaviour
    {
        private ItemExplanation explanation = null;

        private void Awake()
        {
            explanation = gameObject.GetComponentInChildren<ItemExplanation>();
        }

        public void SetInfo(bool isActive, ItemType type)
        {
            gameObject.SetActive(isActive);
            explanation.SetInfo(type);
        }

        public void ResetInfo(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}