using Godot;
using System;

namespace MapleStory
{
    public partial class Camera : Node2D
    {
        // Movement variables.
        private Linear<double> x = new();
        private Linear<double> y = new();

        // View limits.
        private Range<int> hbounds = new();
        private Range<int> vbounds = new();

        private int VWIDTH = Constants.Constant.VWIDTH;
        private int VHEIGHT = Constants.Constant.VHEIGHT;

        // Initialize everything to 0, we need the player's spawnpoint first to properly set the position.
        public void Init()
        {
            x.SetAllValue(0.0d);
            y.SetAllValue(0.0d);
        }

        // Update the view with the current player position. (Or any other target)
        public void Update(MaplePoint<int> position)
        {
            double nextX = x.Get();
            double hdelta = VWIDTH / 2 - position.X - nextX;

            if (Mathf.Abs(hdelta) >= 5.0)
                nextX += hdelta * (24.0 / VWIDTH);

            double nextY = y.Get();
            double vdelta = VHEIGHT / 2 - position.Y - nextY;

            if (Mathf.Abs(vdelta) >= 5.0)
                nextY += vdelta * (24.0 / VHEIGHT);

            if (nextX > hbounds.First || hbounds.Length < VWIDTH)
                nextX = hbounds.First;
            else if (nextX < hbounds.Second + VWIDTH)
                nextX = hbounds.Second + VWIDTH;

            if (nextY > vbounds.First || vbounds.Length < VHEIGHT)
                nextY = vbounds.First;
            else if (nextY < vbounds.Second + VHEIGHT)
                nextY = vbounds.Second + VHEIGHT;

            x.UpdateValue(nextX);
            y.UpdateValue(nextY);
        }

        // Set the position, changing the view immediately.
        public void SetPosition(MaplePoint<int> position)
        {
            x.SetAllValue(VWIDTH / 2 - position.X);
            y.SetAllValue(VHEIGHT / 2 - position.Y);

            Position = new Vector2((float)x.Get(), (float)y.Get());
        }

        // Updates the view's boundaries. Determined by mapinfo or footholds.
        public void SetView(Range<int> mapWalls, Range<int> mapBorders)
        {
            hbounds = -mapWalls;
            vbounds = -mapBorders;
        }

        // Return the current position.
        public MaplePoint<int> CurrentPosition()
        {

            float shortX = (float)Math.Round(x.Get());
            float shortY = (float)Math.Round(y.Get());

            return new MaplePoint<int>((int)shortX, (int)shortY);
        }

        // Return the interpolated position.
        public MaplePoint<int> CurrentPosition(float alpha)
        {
            float interX = (float)Math.Round(x.Get(alpha));
            float interY = (float)Math.Round(y.Get(alpha));

            return new MaplePoint<int>((int)interX, (int)interY);
        }

        // Return the interpolated position.
        public MaplePoint<double> RealPosition(float alpha)
        {
            return new MaplePoint<double>(x.Get(alpha), y.Get(alpha));
        }
    }
}