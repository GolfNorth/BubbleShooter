namespace BubbleShooter
{
    public struct Coordinate
    {
        public int Row;
        public int Column;


        public Coordinate(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool IsRowEven => Row % 2 == 0;

        public override string ToString()
        {
            return "{" + Row + ", " + Column + "}";
        }
    }
}