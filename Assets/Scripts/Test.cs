using System;
using BubbleShooter;
using UnityEngine;

namespace DefaultNamespace
{
    public class Test : MonoBehaviour
    {
        private void Update()
        {
            var board = Context.Instance.LevelController.Board;
            
            Debug.Log(board.CollisionCheck(transform.position));
        }
    }
}