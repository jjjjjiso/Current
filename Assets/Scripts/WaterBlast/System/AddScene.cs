using UnityEngine;
using UnityEngine.SceneManagement;

namespace WaterBlast.System
{
    public class AddScene : MonoBehaviour
    {
        public string[] strAddScene = null;

        private void Awake()
        {
            if (strAddScene == null) return;

            for (int iLoop = 0; iLoop < strAddScene.Length; ++iLoop)
            {
                SceneManager.LoadScene(strAddScene[iLoop], LoadSceneMode.Additive);
            }
        }
    }
}