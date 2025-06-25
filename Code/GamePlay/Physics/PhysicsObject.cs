using Godot;
using System;

namespace MapleStory
{
    public partial class MovingObject : Node2D
    {
        public Linear<double> x = new Linear<double>();
        public Linear<double> y = new Linear<double>();
        public double hspeed = 0.0;
        public double vspeed = 0.0;

        protected float alpha;
        public void Normalize()
        {
            x.Normalize();
            y.Normalize();
        }
        public void Move()
        {
            x.UpdateValue(x + hspeed);
            y.UpdateValue(y + vspeed);
        }
        public void SetX(double d)
        {
            x.SetAllValue(d);
        }
        public void SetY(double d)
        {
            y.SetAllValue(d);
        }
        public void LimitX(double d)
        {
            x.UpdateValue(d);
            hspeed = 0.0;
        }
        public void LimitY(double d)
        {
            y.UpdateValue(d);
            vspeed = 0.0;
        }
        public void MoveXUntil(double d, int delay, double physicDelta)
        {
            if (delay != 0)
            {
                double hdelta = d - x.Get();
                hspeed = physicDelta * hdelta / delay;
            }
        }
        public void MoveYUntil(double d, int delay, double physicDelta)
        {
            if (delay != 0)
            {
                double vdelta = d - y.Get();
                vspeed = physicDelta * vdelta / delay;
            }
        }
        public bool HMobile()
        {
            return hspeed != 0.0;
        }
        public bool VMobile()
        {
            return vspeed != 0.0;
        }
        public bool Mobile()
        {
            return HMobile() || VMobile();
        }
        public double CurrentX()
        {
            return x.Get();
        }
        public double CurrentY()
        {
            return y.Get();
        }
        public double NextX()
        {
            return x + hspeed;
        }
        public double NextY()
        {
            return y + vspeed;
        }
        public int GetX()
        {
            return (int)Math.Round(x.Get());
        }
        public int GetY()
        {
            return (int)Math.Round(y.Get());
        }
        public int GetLastX()
        {
            return (int)Math.Round(x.Last());
        }
        public int GetLastY()
        {
            return (int)Math.Round(y.Last());
        }
        public new MaplePoint<int> GetPosition()
        {
            return new MaplePoint<int>(GetX(), GetY());
        }
        public int GetAbsoluteX(double viewX)
        {
            double interX = x.Normalized() ? Math.Round(x.Get()) : x.Get(alpha);
            return (int)Math.Round(interX + viewX);
        }
        public int GetAbsoluteY(double viewY)
        {
            double interY = y.Normalized() ? Math.Round(y.Get()) : y.Get(alpha);
            return (int)Math.Round(interY + viewY);
        }
        public MaplePoint<int> GetAbsolute(double viewX, double viewY)
        {
            return new MaplePoint<int>(GetAbsoluteX(viewX), GetAbsoluteY(viewY));
        }
        public override void _Process(double delta)
        {
            alpha = (float)Engine.GetPhysicsInterpolationFraction();
        }

        public override void _PhysicsProcess(double delta)
        {
            Move();
        }
    }

    public partial class PhysicsObject : MovingObject
    {
        public enum Type
        {
            Normal,
            Ice,
            Swimming,
            Flying,
            Fixated
        }
        [Flags]
        public enum Flag
        {
            NoGravity = 0x0001,
            TurnAtEdges = 0x0002,
            CheckBelow = 0x0004
        }
        public Type objectType { get; set; } = Type.Normal;
        public int flags { get; set; } = 0;
        public int footHoldId { get; set; } = 0;
        public double footHoldSlope { get; set; } = 0;
        public int footHoldLayer { get; set; } = 0;
        public double groundBelow { get; set; } = 0;
        public bool onGround { get; set; } = true;
        public bool enableJumpDown { get; set; } = false;

        public double hForce { get; set; } = 0;
        public double vForce { get; set; } = 0;
        public double hAcceleration { get; set; } = 0;
        public double vAcceleration { get; set; } = 0;

        public bool IsFlagSet(Flag flag) => (flags & (int)flag) != 0;

        public bool IsFlagNotSet(Flag flag) => !IsFlagSet(flag);

        public void SetFlag(Flag flag) => flags |= (int)flag;

        public void ClearFlag(Flag flag) => flags &= ~(int)flag;

        public void ClearFlags() => flags = 0;

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
        }
    }
}