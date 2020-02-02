using System.Collections;
using UnityEngine;

using WaterBlast.System;
using WaterBlast.Game.UI;

namespace WaterBlast.Game.Common
{
    public class BlockEntity : MonoBehaviour
    {
        //public field
        

        //private field
        [SerializeField]
        protected State state = State.idle;
        [SerializeField]
        protected BlockIcon iconUI = null;

        protected BlockDef blockDef = null; //index
        protected UISprite uiSprite = null;
        protected UIWidget uiWidget = null;
        protected Animator anim = null;

        protected int animLayer = 0;

        //default Method
        protected void Awake()
        {
            uiSprite = gameObject.GetComponentInChildren<UISprite>();
            uiWidget = gameObject.GetComponent<UIWidget>();
            anim = gameObject.GetComponent<Animator>();
        }

        //public Method
        public virtual void Show()
        {
            state = State.idle;
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            state = State.wait;
            gameObject.SetActive(false);
        }

        public void PlayAnim(string name)
        {
            anim.SetTrigger(name);
        }

        public void MixMove(Vector2 endPos, int x, int y)
        {
            StartCoroutine(Co_MixMove(endPos, x, y));
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
            blockDef.x = (int)_LocalPosition.x / _BlockWidthSize;
            blockDef.y = (int)_LocalPosition.y / _BlockHeightSize;
        }

        public void SetPosition(int x, int y)
        {
            _LocalPosition = new Vector2(x, y);
            blockDef.x = x;
            blockDef.y = y;
        }

        public void SetDepth(int depth)
        {
            if (uiWidget != null) uiWidget.depth = depth;
            if (uiSprite != null) uiSprite.depth = depth;
            if (iconUI != null) iconUI.SetDepth(depth + 1);
        }

        public void SetIcon(string name)
        {
            iconUI.SpriteUpdate(name, _BlockDepth + 1);
        }

        //private Method

        //coroutine Method
        protected IEnumerator Co_MixMove(Vector2 endPos, int x, int y)
        {
            state = State.move;

            yield return StartCoroutine(Co_Move2(endPos, .3f));

            state = State.idle;
            SetData(x, y);
            _LocalPosition = endPos;
            SetDepth(y + 11);
        }

        protected IEnumerator Co_DownMove(Vector2 endPos, int x, int y)
        {
            state = State.move;

            yield return StartCoroutine(Co_Move(endPos, 3.5f));
            
            state = State.idle;
            SetData(x, y);
            _LocalPosition = endPos;
            SetDepth(y + 11);
            if(anim != null) anim.SetTrigger("Falling");
        }

        protected IEnumerator Co_TargetMove(Vector2 endPos)
        {
            state = State.booster_move;
            UIWidget widget = gameObject.GetComponent<UIWidget>();
            int depth = _BlockDepth;
            widget.depth = 30;

            yield return StartCoroutine(Co_Move(endPos, 5.5f));

            widget.depth = depth;
            Hide();
            _LocalPosition = endPos;
        }

        protected IEnumerator Co_Move(Vector2 endPos, float duration)
        {
            Vector2 startPos = this._LocalPosition;
            float time = 0;
            float totalTime = 1f / duration;

            while (time < totalTime)
            {
                time += Time.deltaTime;
                _LocalPosition = Vector3.Lerp(startPos, endPos, time * duration);
                yield return null;
            }
        }

        protected IEnumerator Co_Move2(Vector2 endPos, float duration)
        {
            Vector2 startPos = this._LocalPosition;
            float startTime = Time.time;

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

        public int _BlockWidthSize
        {
            get { return (uiWidget != null) ? uiWidget.width : -1; }
        }

        public int _BlockHeightSize
        {
            get { return (uiWidget != null) ? uiWidget.height : -1; }
        }

        public int _BlockDepth
        {
            get { return (uiWidget != null) ? uiWidget.depth : -1; }
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
