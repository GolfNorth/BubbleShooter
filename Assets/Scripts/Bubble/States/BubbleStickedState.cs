using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleStickedState : BubbleState
    {
        public BubbleStickedState(Bubble bubble) : base(bubble, BubbleStateType.Sticked)
        {
        }

        public override void Enter()
        {
            Bubble.Rigidbody.bodyType = RigidbodyType2D.Dynamic;
            Bubble.Rigidbody.gravityScale = 0;
            Bubble.Rigidbody.drag = Context.Instance.Settings.BubbleStickedDrag;
            Bubble.SpringJoint.enabled = true;
            Bubble.SpringJoint.connectedBody  = Bubble.Anchor.Rigidbody;
        }

        public override void Exit()
        {
            Bubble.Rigidbody.gravityScale = Context.Instance.Settings.BubbleDefaultGravity;
            Bubble.Rigidbody.drag = 0;
            Bubble.SpringJoint.enabled = false;
        }
    }
}