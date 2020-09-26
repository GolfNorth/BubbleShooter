using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleFallingState : BubbleState
    {
        #region Fields

        private float _bottomBorder;

        #endregion

        #region Methods

        public BubbleFallingState(Bubble bubble) : base(bubble, BubbleStateType.Falling)
        {
        }

        #endregion

        #region Methods

        public override void Enter()
        {
            _bottomBorder = Context.Instance.BoundsService.Bounds.Bottom
                            - Context.Instance.Settings.BubbleRadius;

            Bubble.gameObject.layer = LayerMask.NameToLayer("Untouchable");
            Bubble.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            Bubble.Rigidbody.gravityScale = Context.Instance.Settings.BubbleDefaultGravity;
            Bubble.Rigidbody.drag = 0;
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            if (Bubble.transform.position.y < _bottomBorder)
            {
                Context.Instance.LevelController.AddScore();
                Context.Instance.LevelController.BubbleController.RemoveBubble(Bubble);
            }
        }

        #endregion
    }
}