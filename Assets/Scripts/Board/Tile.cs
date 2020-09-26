namespace BubbleShooter
{
    public sealed class Tile
    {
        #region Properties

        public bool Processed { get; set; }
        public Coordinate Coordinate => Anchor ? Anchor.Coordinate : new Coordinate();
        public Anchor Anchor { get; set; }
        public Bubble Bubble { get; set; }

        #endregion
    }
}