using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleBurstingState : BubbleState
    {
        private const string BurstsAnimation = "Bursts";

        public BubbleBurstingState(Bubble bubble) : base(bubble, BubbleStateType.Bursting)
        {
        }

        public override void Enter()
        {
            Bubble.gameObject.layer = LayerMask.NameToLayer("Untouchable");
            Bubble.Rigidbody.bodyType = RigidbodyType2D.Static;
            Bubble.Animator.Play(BurstsAnimation);
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
            if (!Bubble.Animator.GetCurrentAnimatorStateInfo(0).IsName(BurstsAnimation))
            {
                Context.Instance.LevelController.AddScore();
                Context.Instance.LevelController.BubbleController.RemoveBubble(Bubble);
            }
        }
    }
}