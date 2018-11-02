using UnityEngine;

namespace WaterBlast.Game.UI
{
    public class BlockIcon : MonoBehaviour
    {
        private UISprite uiSprite = null;

        private void Awake()
        {
            uiSprite = gameObject.GetComponent<UISprite>();
        }

        public void SpriteUpdate(string name, int depth)
        {
            uiSprite.spriteName = name;
            SetDepth(depth);
        }

        public void SetDepth(int depth)
        {
            uiSprite.depth = depth;
        }
    }
}