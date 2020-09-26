using UnityEngine;
using UnityEngine.SceneManagement;

namespace BubbleShooter
{
    public sealed class SceneService
    {
        #region Fields

        private Scene _currentScene;

        #endregion

        #region Constructor

        public SceneService()
        {
            _currentScene = SceneManager.GetActiveScene();
        }

        #endregion

        #region Properties

        public Scene CurrentScene => _currentScene;

        #endregion

        #region Methods

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

        #endregion
    }
}