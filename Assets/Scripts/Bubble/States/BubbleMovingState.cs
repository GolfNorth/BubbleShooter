using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleMovingState : BubbleState
    {
        private float _t;
        private Vector2 _nextPosition;
        private Vector2 _prevPosition;
        private float _timeToReachTarget;
        
        public BubbleMovingState(Bubble bubble) : base(bubble, BubbleStateType.Moving)
        {
        }

        public override void Enter()
        {
            Bubble.gameObject.layer = LayerMask.NameToLayer("Untouchable");
            Bubble.transform.SetParent(null);
            Bubble.Rigidbody.bodyType = RigidbodyType2D.Static;

            Bubble.StartCoroutine(MoveBubble());
        }

        public override void Exit()
        {
            Bubble.gameObject.layer = LayerMask.NameToLayer("Bubble");
            Bubble.transform.SetParent(Context.Instance.LevelController.Board.transform);
        }

        private IEnumerator MoveBubble()
        {
            var points = Bubble.Trajectory.Points;
            var timeStamps = new List<float>(points.Keys);
            
            for (var i = 1; i < timeStamps.Count; i++)
            {
                _t = 0;
                _timeToReachTarget = timeStamps[i] - timeStamps[i - 1];
                _prevPosition = Bubble.transform.position;
                _nextPosition = points[timeStamps[i]];
                
                yield return new WaitForSeconds(_timeToReachTarget);
            }

            if (Bubble.Trajectory.Collided)
            {
                Context.Instance.LevelController.Board.StickBubble(Bubble);
            }
            else
            {
                Context.Instance.LevelController.BubbleController.RemoveBubble(Bubble);
            }
            
            Context.Instance.NotificationService.Notify(NotificationType.BubbleLaunched);
        }

        public override void Update()
        {
            _t += Time.deltaTime / _timeToReachTarget;
            Bubble.transform.position = Vector3.Lerp(_prevPosition, _nextPosition, _t);
        }
    }
}