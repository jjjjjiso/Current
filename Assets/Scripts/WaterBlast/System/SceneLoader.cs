using UnityEngine;
using UnityEngine.SceneManagement;

namespace WaterBlast.System
{
    public class SceneLoader : MonoBehaviour
    {
        //다음 씬 정보
        public string m_strNextSceneName = string.Empty;

        private Scene m_Scene;

        public void MoveScene(string _strScene)
        {
            m_strNextSceneName = _strScene;
            NextScene();
        }

        public void NextScene()
        {
            m_Scene = SceneManager.GetActiveScene();
            SceneManager.sceneLoaded += Loaded;
            SceneManager.LoadScene(m_strNextSceneName, LoadSceneMode.Additive);
        }

        private void Loaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (m_strNextSceneName == scene.name)
            {
                SceneManager.SetActiveScene(scene);
                SceneManager.UnloadSceneAsync(m_Scene);
                SceneManager.sceneLoaded -= Loaded;
            }
        }
    }
}