using System.Collections;
using UnityEngine;

namespace WaterBlast.System
{
    public class Fade : MonoBehaviour
    {
        [SerializeField]
        private UITexture uiFadeImg = null;

        protected float currentTime = 0f;
        public float fadeTime = 1f;

        protected virtual IEnumerator Co_FadeIn()
        {
            Color color = uiFadeImg.color;
            while (color.a > 0)
            {
                currentTime += Time.deltaTime / fadeTime;
                color.a = Mathf.Lerp(1, 0, currentTime);
                uiFadeImg.color = color;
                yield return null;
            }

            uiFadeImg.alpha = 0;
            currentTime = 0;

            yield return new WaitForSecondsRealtime(0.35f);
        }

        protected virtual IEnumerator Co_FadeOut()
        {
            //FadeOut 
            Color color = uiFadeImg.color;
            while (color.a < 1f)
            {
                currentTime += Time.deltaTime / fadeTime;
                color.a = Mathf.Lerp(0, 1, currentTime);
                uiFadeImg.color = color;
                yield return null;
            }

            uiFadeImg.alpha = 1;
            currentTime = 0;

            yield return new WaitForSecondsRealtime(0.35f);
        }
    }
}