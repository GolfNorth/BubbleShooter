using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubbleShooter
{
    public class Context : MonoBehaviour
    {
        private static Context _instance;

        private Scene _currentScene;

        public static Context Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                _instance = FindObjectOfType<Context>();

                return _instance != null ? _instance : Create();
            }
        }

        private static Context Create()
        {
            var contextGameObject = new GameObject("Context");
            _instance = contextGameObject.AddComponent<Context>();

            return _instance;
        }

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            _currentScene = SceneManager.GetActiveScene();
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
            
            _currentScene = SceneManager.GetActiveScene();
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}