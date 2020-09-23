using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleIdleState : BubbleState
    {
        public BubbleIdleState(Bubble bubble) : base(bubble, BubbleStateType.Idle)
        {
        }

        public override void Enter()
        {
            Bubble.gameObject.layer = LayerMask.NameToLayer("Untouchable");
            Bubble.transform.SetParent(null);
            Bubble.Rigidbody.bodyType = RigidbodyType2D.Static;
        }
        
        public override void Exit()
        {
            Bubble.gameObject.layer = LayerMask.NameToLayer("Bubble");
            Bubble.transform.SetParent(Context.Instance.LevelController.Board.transform);
        }
    }
}