﻿using UnityEngine;

namespace BubbleShooter
{
    public sealed class MenuController : MonoBehaviour
    {
        #region Methods

        public void LoadScene(string sceneName)
        {
            Context.Instance.SceneService.LoadScene(sceneName);
        }

        public void ExitGame()
        {
            Context.Instance.ExitGame();
        }

        #endregion
    }
}