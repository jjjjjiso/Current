using UnityEngine;

namespace WaterBlast.Game.UI
{
    public class BlockIcon : MonoBehaviour
    {
        private UISprite uiSprite = null;
        private Color col;

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

        public void SetColor(float a = 1)
        {
            col = uiSprite.color;
            col.a = a;
            uiSprite.color = col;
        }
    }
}