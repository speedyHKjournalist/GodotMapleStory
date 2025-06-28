using Godot;
using WzComparerR2.WzLib;

namespace MapleStory
{
    // Combines an Animation with additional state
    public partial class MapleSprite : MapleAnimation
    {
        private DrawArgument stateArgs = new();
        private float speed = 1.0f;

        public MapleSprite(MapleAnimation animation, DrawArgument stateArgs) : base(animation)
        {
            this.stateArgs = stateArgs;
        }

        public MapleSprite(Wz_Node src, DrawArgument stateArgs) : base(src)
        {
            this.stateArgs = stateArgs;
        }

        public MapleSprite(Wz_Node src) : this(src, new DrawArgument()) { }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public void Interpolate(MaplePoint<int> parentPosition)
        {
            DrawArgument absArgs = stateArgs + parentPosition;
            base.Interpolate(absArgs);
        }

        public override void _PhysicsProcess(double delta)
        {
            alpha = (float)Engine.GetPhysicsInterpolationFraction();
            animationEnd = Update((int)(delta * speed * 1000));
        }
    }
}