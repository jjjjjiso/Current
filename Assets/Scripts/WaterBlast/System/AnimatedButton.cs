using System.Collections;
using UnityEngine;

namespace WaterBlast.System
{
    public class AnimatedButton : MonoBehaviour
    {
        private Animator animator = null;
        private bool isBlockInput = false;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void OnPressed()
        {
            if (isBlockInput) return;

            isBlockInput = true;
            animator.SetTrigger("Pressed");
            StartCoroutine(BlockInputTemporarily());
        }

        private IEnumerator BlockInputTemporarily()
        {
            yield return new WaitForSecondsRealtime(0.5f);
            isBlockInput = false;
        }
    }
}