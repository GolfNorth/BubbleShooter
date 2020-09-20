namespace BubbleShooter
{
    public abstract class Node
    {
        protected Context Context;
        
        protected Node()
        {
            Context = Context.Instance;
        }
    }
}