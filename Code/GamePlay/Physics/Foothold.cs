namespace MapleStory
{
    public class FootHold
    {
        public FootHold()
        {
            id = 0;
            layer = 0;
            prev = 0;
            next = 0;
            horizontal = new Range<int>();
            vertical = new Range<int>();
        }
        public int group { get; set; }
        public int layer { get; set; }
        public int id { get; set; }
        public Range<int> horizontal { get; set; }
        public Range<int> vertical { get; set; }
        public int prev { get; set; }
        public int next { get; set; }
        public int piece { get; set; }
        public int x1()
        {
            return horizontal.First;
        }

        public int x2()
        {
            return horizontal.Second;
        }

        public int y1()
        {
            return vertical.First;
        }

        public int y2()
        {
            return vertical.Second;
        }
        public int Prev()
        {
            return prev;
        }
        public int Next()
        {
            return next;
        }
        public int HDelta()
        {
            return horizontal.Delta;
        }
        public int VDelta()
        {
            return vertical.Delta;
        }
        public double Slope()
        {
            return IsWall() ? 0.0f : ((double)VDelta() / (double)HDelta());
        }
        public double GroundBelow(double x)
        {
            return IsFloor() ? y1() : (Slope() * (x - x1()) + y1());
        }
        public int Layer()
        {
            return layer;
        }
        public int Left()
        {
            return horizontal.Smaller;
        }
        public int Right()
        {
            return horizontal.Greater;
        }
        public int Top()
        {
            return vertical.Smaller;
        }
        public int Bottom()
        {
            return vertical.Greater;
        }
        public bool IsWall()
        {
            return (id != 0) && horizontal.Empty;
        }
        public bool IsFloor()
        {
            return (id != 0) && vertical.Empty;
        }
        public bool IsBlocking(Range<int> vertical)
        {
            return IsWall() && this.vertical.Overlaps(vertical);
        }
    }
}