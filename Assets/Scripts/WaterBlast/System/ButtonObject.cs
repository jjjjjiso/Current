using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WaterBlast.System
{
    public class ButtonObject : MonoBehaviour
    {
        public GameObject obj;
        public Transform tran;

        private Vector3 vecPress = new Vector3(0.9f, 0.9f, 0.9f);
        private Vector3 vecRelese = new Vector3(1.1f, 1.1f, 1.1f);
        private float pressTime = 0.2f;
        private float releseTime = 0.15f;

        public UnityEngine.Events.UnityAction fncClick;

        private bool isPress = false;

        public void OnPress()
        {
            if (isPress) return;
            isPress = true;
            LeanTween.cancel(obj);
            LeanTween.scale(obj, vecPress, pressTime);
        }

        public void OnRelese()
        {
            if (!isPress) return;
            LeanTween.cancel(obj);
            LeanTween.scale(obj, vecRelese, releseTime).setOnComplete(() =>
            {
                LeanTween.scale(obj, Vector3.one, releseTime).setOnComplete(() => isPress = false);
            });

            if (fncClick != null) fncClick();
        }
    }
}