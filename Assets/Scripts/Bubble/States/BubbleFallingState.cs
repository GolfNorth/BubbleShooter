using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleFallingState : BubbleState
    {
        public BubbleFallingState(Bubble bubble) : base(bubble, BubbleStateType.Falling)
        {
        }

        public override void Enter()
        {
            Bubble.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            Bubble.Rigidbody.gravityScale = Context.Instance.Settings.BubbleDefaultGravity;
            Bubble.Rigidbody.drag = 0;
        }
        
        public override void Exit()
        {
        }
    }
}