using System.Collections;
using UnityEngine;

namespace WonderBlast.Game.Common
{
    public class BlockEntity : MonoBehaviour
    {
        //public field

        //private field
        [SerializeField]
        protected State state = State.idle;
        protected BlockDef blockDef = null; //index
        protected UISprite uiSprite = null;

        //default Method
        protected void Awake()
        {
            uiSprite = gameObject.GetComponentInChildren<UISprite>();
        }

        //public Method
        public virtual void Show()
        {
            state = State.idle;
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public void DownMove(Vector2 endPos, int x, int y)
        {
            StartCoroutine(Co_DownMove(endPos, x, y));
        }

        public void TargetMove(Vector2 endPos)
        {
            StartCoroutine(Co_TargetMove(endPos));
        }

        public void SetData(int x, int y)
        {
            blockDef = new BlockDef(x, y);
        }

        public void SetData(BlockDef blockDef)
        {
            this.blockDef = blockDef;
        }

        public void SetData()
        {
            blockDef.x = (int)_LocalPosition.x;
            blockDef.y = (int)_LocalPosition.y;
        }

        public void SetPosition(int x, int y)
        {
            _LocalPosition = new Vector2(x, y);
            blockDef.x = x;
            blockDef.y = y;
        }

        //private Method

        //coroutine Method
        protected IEnumerator Co_DownMove(Vector2 endPos, int x, int y)
        {
            state = State.move;

            yield return StartCoroutine(Co_Move(endPos, .35f));

            state = State.idle;
            SetData(x, y);
            _LocalPosition = endPos;
        }

        protected IEnumerator Co_TargetMove(Vector2 endPos)
        {
            state = State.special_move;
            UIWidget widget = gameObject.GetComponent<UIWidget>();
            widget.depth = 10;

            yield return StartCoroutine(Co_Move(endPos, .2f));

            widget.depth = 1;
            state = State.wait;
            Hide();
            _LocalPosition = endPos;
        }

        protected IEnumerator Co_Move(Vector2 endPos, float duration)
        {
            Vector2 startPos = this._LocalPosition;
            float startTime = Time.time;
            //float duration = .35f;
            while (Time.time - startTime <= duration)
            {
                _LocalPosition = Vector2.Lerp(startPos, endPos, (Time.time - startTime) / duration);
                yield return null;
            }
        }

        //Property
        public Vector2 _LocalPosition
        {
            get { return transform.localPosition; }
            set { transform.localPosition = value; }
        }

        public Vector2 _LocalScale
        {
            get { return transform.localScale; }
            set { transform.localScale = value; }
        }

        public int _SpriteWidthSize
        {
            get { return (uiSprite != null) ? uiSprite.width : -1; }
        }

        public int _SpriteHeightSize
        {
            get { return (uiSprite != null) ? uiSprite.height : -1; }
        }

        public BlockDef _BlockData
        {
            get { return blockDef; }
        }

        public State _State
        {
            get { return state; }
            set { state = value; }
        }

        public int _X
        {
            get { return blockDef.x; }
        }

        public int _Y
        {
            get { return blockDef.y; }
        }
    }
}
