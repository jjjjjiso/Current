using UnityEngine;

using WaterBlast.Game.Manager;

namespace WaterBlast.Game.UI
{
    public class Item : MonoBehaviour
    {
        [SerializeField]
        protected UIButton itemBtnUI = null;
        [SerializeField]
        protected UISprite itemIconUI   = null;
        [SerializeField]
        protected UILabel itemCntUI = null;
        [SerializeField]
        protected GameObject lockUI = null;
        [SerializeField]
        protected ItemClicked itemClicked = null;

        protected bool isLock = false;

        public void UnLockUI(int index, bool isUnLock)
        {
            if(isUnLock)
            {
                itemBtnUI.normalSprite = "BlueSquareButton";
                Color col = itemIconUI.color;
                col.a = 1;
                itemIconUI.color = col;
            }
            else
            {
                itemBtnUI.normalSprite = "GreySquareButton";
                Color col = itemIconUI.color;
                col.a = 0.45f;
                itemIconUI.color = col;
            }

            isLock = !isUnLock;
            lockUI.SetActive(!isUnLock);

            UpdateInGameItemCount(index);
        }

        public void UpdateInGameItemCount(int index)
        {
            int count = UserDataMgr.G.availableInGameItemCount[index];
            itemCntUI.text = (count > 0) ? count.ToString() : "+";
        }

        public void DepthSetting(int value)
        {
            itemBtnUI.GetComponent<UISprite>().depth = value;
            itemIconUI.depth = value + 1;
            itemCntUI.depth = value + 2;
        }

        public void ResetInfo()
        {
            itemClicked.ResetInfo(false);
            DepthSetting(2);
        }

        public bool IsWhetherOrNotToUse()
        {
            if (GameMgr.G.isGameEnd) return false;
            if (GameMgr.G._Stage.isWait) return false;

            return true;
        }
    }
}