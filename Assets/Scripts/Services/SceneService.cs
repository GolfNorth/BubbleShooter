using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubbleShooter
{
    public sealed class SceneService
    {
        private Scene _currentScene;

        public SceneService()
        {
            _currentScene = SceneManager.GetActiveScene();
        }

        public Scene CurrentScene => _currentScene;

        public void LoadScene(string sceneName)
        {
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                SceneManager.LoadScene(sceneName);

                Context.Instance.NotificationService.Notify(NotificationType.SceneLoaded);

                _currentScene = SceneManager.GetActiveScene();
            }
            else
            {
                Debug.LogError("Scene not found");
            }
        }
    }
}