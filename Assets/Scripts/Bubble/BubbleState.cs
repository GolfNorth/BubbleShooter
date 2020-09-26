namespace BubbleShooter
{
    public abstract class BubbleState
    {
        public readonly BubbleStateType State;
        protected readonly Bubble Bubble;

        protected BubbleState(Bubble bubble, BubbleStateType state)
        {
            Bubble = bubble;
            State = state;
        }

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
    }
}