using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.UI
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        protected GameObject itemUI = null;
        [SerializeField]
        protected GameObject lockUI = null;
        [SerializeField]
        protected ItemClicked itemClicked = null;

        public void LockUI(bool isLock)
        {
            lockUI.SetActive(!isLock);
            itemUI.SetActive(isLock);
        }

        public void ResetInfo()
        {
            itemClicked.ResetInfo(false);
            itemUI.GetComponent<UISprite>().depth = 2;
        }

        public bool IsWhetherOrNotToUse()
        {
            if (GameMgr.Get().isGameEnd) return false;
            if (GameMgr.Get()._Stage.isWait) return false;

            return true;
        }
    }
}