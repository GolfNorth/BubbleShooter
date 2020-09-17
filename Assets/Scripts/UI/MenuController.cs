using UnityEngine;

namespace BubbleShooter
{
    public sealed class MenuController : MonoBehaviour
    {
        public void LoadScene(string sceneName)
        {
            Context.Instance.LoadScene(sceneName);
        }
        
        public void ExitGame()
        {
            Context.Instance.ExitGame();
        }
    }
}