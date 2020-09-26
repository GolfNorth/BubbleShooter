using UnityEngine;

namespace BubbleShooter
{
    public sealed class BubbleIdleState : BubbleState
    {
        #region Constructor

        public BubbleIdleState(Bubble bubble) : base(bubble, BubbleStateType.Idle)
        {
        }

        #endregion

        #region Methods

        public override void Enter()
        {
            Bubble.gameObject.layer = LayerMask.NameToLayer("Untouchable");
            Bubble.transform.SetParent(null);
            Bubble.Rigidbody.bodyType = RigidbodyType2D.Static;
        }

        public override void Exit()
        {
            Bubble.transform.SetParent(Context.Instance.LevelController.Board.transform);
        }

        #endregion
    }
}