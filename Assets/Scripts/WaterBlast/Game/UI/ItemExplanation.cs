using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using WaterBlast.Game.Common;

namespace WaterBlast.Game.UI
{
    public class ItemExplanation : MonoBehaviour
    {
        [SerializeField]
        private UISprite uiPicture = null;
        [SerializeField]
        private UILabel uiTitle = null;
        [SerializeField]
        private UILabel uiContents = null;

        public void SetInfo(ItemType type)
        {
            if (uiPicture != null) uiPicture.spriteName = type.ToString();
            string title = null;
            string contents = null;
            switch (type)
            {
                case ItemType.hammer:
                    title = "Hammer";
                    contents = "망치 아이템은 블럭 하나를 제거할 수 있습니다.";
                    break;
                case ItemType.horizon:
                    title = "Horizontal Glove";
                    contents = "가로 글러브 아이템은 한 줄의 블럭들을 제거할 수 있습니다.";
                    break;
                case ItemType.vertical:
                    title = "Vertical Glove";
                    contents = "세로 글러브 아이템은 한 줄의 블럭들을 제거할 수 있습니다.";
                    break;
                case ItemType.mix:
                    title = "Tonado";
                    contents = "회오리 아이템은 전체 블럭들을 재배치 시켜줍니다.";
                    break;
            }
            
            if (uiTitle != null) uiTitle.text = title;
            if (uiContents != null) uiContents.text = contents;
        }
    }
}