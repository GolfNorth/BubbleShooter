namespace BubbleShooter
{
    public abstract class BubbleState
    {
        #region Fields

        public readonly BubbleStateType State;
        protected readonly Bubble Bubble;

        #endregion

        #region Properties

        protected BubbleState(Bubble bubble, BubbleStateType state)
        {
            Bubble = bubble;
            State = state;
        }

        #endregion

        #region Methods

        public abstract void Enter();

        public abstract void Exit();

        public virtual void Update()
        {
        }

        public virtual void FixedUpdate()
        {
        }

        public virtual void LateUpdate()
        {
        }

        #endregion
    }
}