namespace MapleStory
{
    public class DrawArgument
    {
        private MaplePoint<int> position;
        private MaplePoint<int> center;
        private MaplePoint<int> stretch;
        private float xScale;
        private float yScale;
        public float angle;
        public MapleColor color;

        public DrawArgument()
            : this(0, 0) { }

        public DrawArgument(int x, int y)
            : this(new MaplePoint<int>(x, y)) { }

        public DrawArgument(MaplePoint<int> position)
            : this(position, 1.0f) { }

        public DrawArgument(MaplePoint<int> position, float xScale, float yScale)
            : this(position, position, xScale, yScale, 1.0f) { }

        public DrawArgument(MaplePoint<int> position, MaplePoint<int> stretch)
            : this(position, position, stretch, 1.0f, 1.0f, new MapleColor(1.0f, 1.0f, 1.0f, 1.0f), 0.0f) { }

        public DrawArgument(MaplePoint<int> position, bool flip)
            : this(position, flip ? -1.0f : 1.0f, 1.0f) { }

        public DrawArgument(float angle, MaplePoint<int> position, float opacity)
            : this(angle, position, false, opacity) { }

        public DrawArgument(MaplePoint<int> position, float opacity)
            : this(position, false, opacity) { }

        public DrawArgument(MaplePoint<int> position, MapleColor color)
            : this(position, position, new MaplePoint<int>(0, 0), 1.0f, 1.0f, color, 0.0f) { }

        public DrawArgument(MaplePoint<int> position, bool flip, MaplePoint<int> center)
            : this(position, center, flip ? -1.0f : 1.0f, 1.0f, 1.0f) { }

        public DrawArgument(MaplePoint<int> position, MaplePoint<int> center, float xScale, float yScale, float opacity)
            : this(position, center, new MaplePoint<int>(0, 0), xScale, yScale, opacity, 0.0f) { }

        public DrawArgument(bool flip)
            : this(flip ? -1.0f : 1.0f, 1.0f, 1.0f) { }

        public DrawArgument(float xScale, float yScale, float opacity)
            : this(new MaplePoint<int>(0, 0), xScale, yScale, opacity) { }

        public DrawArgument(MaplePoint<int> position, float xScale, float yScale, float opacity)
            : this(position, position, xScale, yScale, opacity) { }

        public DrawArgument(MaplePoint<int> position, bool flip, float opacity)
            : this(position, position, flip ? -1.0f : 1.0f, 1.0f, opacity) { }

        public DrawArgument(float angle, MaplePoint<int> position, bool flip, float opacity)
            : this(position, position, new MaplePoint<int>(0, 0), flip ? -1.0f : 1.0f, 1.0f, opacity, angle) { }

        public DrawArgument(MaplePoint<int> position, MaplePoint<int> center, MaplePoint<int> stretch, float xScale, float yScale, float opacity, float angle)
        {
            this.position = new MaplePoint<int>(position.X, position.Y);
            this.center = new MaplePoint<int>(center.X, center.Y);
            this.stretch = new MaplePoint<int>(stretch.X, stretch.Y);
            this.xScale = xScale;
            this.yScale = yScale;
            this.color = new MapleColor(1.0f, 1.0f, 1.0f, opacity);
            this.angle = angle;
        }

        public DrawArgument(MaplePoint<int> position, MaplePoint<int> center, MaplePoint<int> stretch, float xScale, float yScale, MapleColor color, float angle)
        {
            this.position = new MaplePoint<int>(position.X, position.Y);
            this.center = new MaplePoint<int>(center.X, center.Y);
            this.stretch = new MaplePoint<int>(stretch.X, stretch.Y);
            this.xScale = xScale;
            this.yScale = yScale;
            this.color = color;
            this.angle = angle;
        }

        public DrawArgument(DrawArgument other)
        {
            position = new MaplePoint<int>(other.position.X, other.position.Y);
            center = new MaplePoint<int>(other.center.X, other.center.Y);
            stretch = new MaplePoint<int>(other.stretch.X, other.stretch.Y);
            xScale = other.xScale;
            yScale = other.yScale;
            angle = other.angle;
            color = new MapleColor(other.color.R, other.color.G, other.color.B, other.color.A);
        }

        public MapleRectangle<int> GetRectangle(MaplePoint<int> origin, MaplePoint<int> dimensions)
        {
            // Calculate width and height of the rectangle
            int w = stretch.X == 0 ? dimensions.X : stretch.X;
            int h = stretch.Y == 0 ? dimensions.Y : stretch.Y;

            MaplePoint<int> rlt = position - center - origin;
            int rl = rlt.X;
            int rr = rlt.X + w;
            int rt = rlt.Y;
            int rb = rlt.Y + h;
            int cx = center.X;
            int cy = center.Y;

            return new MapleRectangle<int>(
                xScale > 0 ? (cx + rl) : (cx - rl),
                xScale > 0 ? (cx + rr) : (cx - rr),
                yScale > 0 ? (cy + rt) : (cx - rt),
                yScale > 0 ? (cy + rb) : (cx - rb));
        }

        public static DrawArgument operator +(DrawArgument arg, MaplePoint<int> offset)
        {
            return new DrawArgument(
                arg.position + offset,
                arg.center + offset,
                arg.stretch,
                arg.xScale,
                arg.yScale,
                arg.color,
                arg.angle
            );
        }

        public static DrawArgument operator +(DrawArgument arg, float opacityMultiplier)
        {
            return new DrawArgument(
                arg.position,
                arg.center,
                arg.stretch,
                arg.xScale,
                arg.yScale,
                new MapleColor(arg.color.R, arg.color.G, arg.color.B, arg.color.A * opacityMultiplier),
                arg.angle
            );
        }

        public static DrawArgument operator +(DrawArgument a, DrawArgument b)
        {
            return new DrawArgument(
                a.position + b.position,
                a.center + b.center,
                a.stretch + b.stretch,
                a.xScale * b.xScale,
                a.yScale * b.yScale,
                a.color * b.color,
                a.angle + b.angle
            );
        }

        public static DrawArgument operator -(DrawArgument a, DrawArgument b)
        {
            return new DrawArgument(
                a.position - b.position,
                a.center - b.center,
                a.stretch - b.stretch,
                a.xScale / b.xScale,
                a.yScale / b.yScale,
                a.color / b.color,
                a.angle - b.angle
            );
        }
        public MaplePoint<int> GetPosition()
        {
            return position.Clone();
        }
        public MaplePoint<int> GetStretch()
        {
            return stretch.Clone();
        }
        public float GetXScale()
        {
            return xScale;
        }
        public float GetYScale()
        {
            return yScale;
        }
        public MapleColor GetColor()
        {
            return color;
        }
        public float GetAngle()
        {
            return angle;
        }

        public MaplePoint<int> GetCenter()
        {
            return center.Clone();
        }
    }
}