using System.Collections;
using UnityEngine;

using WaterBlast.Game.Popup;

namespace WaterBlast.System
{
    public class SceneFadeInOut : Fade
    {
        [SerializeField] protected Collider clickCollider = null;

        [Tooltip("FadeIn 후 바로 FadeOut 처리 원할때 true 체크")]
        [SerializeField] protected bool isFadeInOut = false;
        //in = true, out = false
        protected bool isFadeCheck = false;
        protected bool isBlockInput = false;

        public float delayTime = 0.5f;

        private void Awake()
        {
            if (isFadeInOut)
            {
                StartCoroutine(Co_FadeInOut());
            }
            else
            {
                StartCoroutine(Co_FadeIn());
            }
        }

        public void OnPressed()
        {
            if (isBlockInput) return;

            isBlockInput = true;

            StartCoroutine(Delay());
        }

        private IEnumerator Delay()
        {
            if (clickCollider != null) clickCollider.enabled = true;

            yield return new WaitForSecondsRealtime(delayTime);

            isBlockInput = false;

            if (!isFadeCheck)
                StartCoroutine(Co_FadeIn());
            else
            {
                StartCoroutine(Co_FadeOut());
            }
        }

        protected override IEnumerator Co_FadeIn()
        {
            if (!isFadeInOut) PopupMgr.G.isFadeCheck = false;

            yield return base.Co_FadeIn();

            isFadeCheck = true;

            if (!isFadeInOut) PopupMgr.G.isFadeCheck = true;

            if (clickCollider != null) clickCollider.enabled = false;
        }

        protected override IEnumerator Co_FadeOut()
        {
            if (!isFadeInOut) PopupMgr.G.isFadeCheck = false;

            yield return base.Co_FadeOut();

            isFadeCheck = false;
            gameObject.GetComponent<SceneLoader>().NextScene();
        }

        protected IEnumerator Co_FadeInOut()
        {
            yield return StartCoroutine(Co_FadeIn());

            StartCoroutine(Co_FadeOut());
        }
    }
}