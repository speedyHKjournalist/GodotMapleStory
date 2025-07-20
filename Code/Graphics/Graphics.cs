using WzComparerR2.WzLib;
using System.Collections.Generic;
using Godot;
using System;
using System.Linq;

namespace MapleStory
{
    public partial class Frame : Node2D
    {
        private MapleTexture? texture;
        private MapleRectangle<int>? bounds;
        private ValueTuple<int, int> opacities;
        private ValueTuple<int, int> scales;
        private MaplePoint<int> head;

        private DrawArgument currentDrawArgument;
        private List<DrawArgument> currentDrawArguments;

        private bool drawMultiple;
        private int delay;

        public Frame()
        {
            delay = 0;
            opacities = new ValueTuple<int, int>(0, 0);
            scales = new ValueTuple<int, int>(0, 0);

            drawMultiple = false;
            currentDrawArgument = new DrawArgument();
            currentDrawArguments = new List<DrawArgument>();
        }

        public Frame(Wz_Node source)
        {
            texture = new MapleTexture(source);
            bounds = new MapleRectangle<int>(source);

            Wz_Vector head = source.FindNodeByPath("head")?.GetValue<Wz_Vector>() ?? new Wz_Vector(0, 0);
            this.head = new MaplePoint<int>(head.X, head.Y);
            delay = source.FindNodeByPath("delay")?.GetValue<int>() ?? 100;

            int a0 = source.FindNodeByPath("a0")?.GetValue<int>() ?? 255;
            int a1 = source.FindNodeByPath("a1")?.GetValue<int>() ?? a0;
            opacities = new ValueTuple<int, int>(a0, a1);

            Wz_Node z0 = source.FindNodeByPath("z0");
            Wz_Node z1 = source.FindNodeByPath("z1");
            if (z0 != null && z1 != null)
            {
                scales = new ValueTuple<int, int>(z0.GetValue<int>(), z1.GetValue<int>());
            }
            else if (z0 != null)
            {
                scales = new ValueTuple<int, int>(z0.GetValue<int>(), 0);
            }
            else if (z1 != null)
            {
                scales = new ValueTuple<int, int>(100, z1.GetValue<int>());
            }
            else
            {
                scales = new ValueTuple<int, int>(100, 100);
            }

            drawMultiple = false;
            currentDrawArgument = new DrawArgument();
            currentDrawArguments = new List<DrawArgument>();
        }

        public Frame Clone()
        {
            Frame copy = new()
            {
                texture = texture!.Clone(),
                delay = delay,
                bounds = new MapleRectangle<int>(bounds!.GetLeftTop(), bounds!.GetRightBottom()),
                opacities = opacities,
                scales = scales,
                head = new MaplePoint<int>(head.X, head.Y),
                currentDrawArgument = new DrawArgument(currentDrawArgument),
                drawMultiple = drawMultiple
            };

            if (currentDrawArguments != null)
            {
                copy.currentDrawArguments = currentDrawArguments
                    .Select(arg => new DrawArgument(arg))
                    .ToList();
            }
            else
            {
                copy.currentDrawArguments = new List<DrawArgument>();
            }


            return copy;
        }

        public override void _Draw()
        {
            if (texture == null || !texture.IsValid())
                return;

            if (drawMultiple)
            {
                foreach (DrawArgument drawArg in currentDrawArguments)
                {
                    texture.Render(this, drawArg);
                }
            }
            else
            {
                texture.Render(this, currentDrawArgument);
            }
        }

        public void SetDrawArgument(DrawArgument args)
        {
            currentDrawArgument = args;
            drawMultiple = false;
            QueueRedraw();
        }

        public void SetDrawArguments(List<DrawArgument> args)
        {
            currentDrawArguments = [.. args.Select(a => new DrawArgument(a))];
            drawMultiple = true;
            QueueRedraw();
        }

        public int StartOpacity()
        {
            return opacities.Item1;
        }
        public int StartScale()
        {
            return scales.Item1;
        }

        public int GetDelay()
        {
            return delay;
        }

        public MaplePoint<int> GetOrigin()
        {
            return texture!.GetOrigin();
        }

        public MaplePoint<int> GetDimension()
        {
            return texture!.GetDimension();
        }

        public MaplePoint<int> GetHead()
        {
            return head;
        }

        public float OpacityStep(int timeStep)
        {
            return timeStep * (float)(opacities.Item2 - opacities.Item1) / delay;
        }

        public float ScaleStep(int timeStep)
        {
            return timeStep * (float)(scales.Item2 - scales.Item1) / delay;
        }

        public MapleRectangle<int> GetBounds()
        {
            return new MapleRectangle<int>(bounds!.GetLeftTop(), bounds!.GetRightBottom());
        }
    }

    public partial class MapleAnimation : Node2D
    {
        private List<Frame> frames;
        private bool animated;
        private bool zigzag;
        protected bool animationEnd = false;

        private Nominal<int> frame;
        private Linear<float> opacity;
        private Linear<float> xyScale;

        protected float alpha;
        private float speed = 1.0f;
        private float opacityStep;
        private int delay;
        private int frameStep;

        public MapleAnimation()
        {
            frame = new Nominal<int>();
            opacity = new Linear<float>();
            xyScale = new Linear<float>();
            frames = [];

            animated = false;
            zigzag = false;

            frames.Add(new Frame());
            Reset();
        }

        public MapleAnimation Clone()
        {
            MapleAnimation copy = new()
            {
                frames = [.. frames.Select(f => f.Clone())],
                speed = speed,
                animated = animated,
                zigzag = zigzag,
                animationEnd = animationEnd,
                delay = delay,
                frameStep = frameStep,
                opacityStep = opacityStep,
                alpha = alpha,

                frame = frame.Clone(),
                opacity = opacity.Clone(),
                xyScale = xyScale.Clone()
            };

            foreach (Frame f in frames)
            {
                f.Visible = false;
                copy.AddChild(f);
            }

            copy.Reset();

            return copy;
        }

        public MapleAnimation(MapleAnimation animation) : this()
        {
            animated = animation.animated;
            zigzag = animation.zigzag;
            animationEnd = animation.animationEnd;
            delay = animation.delay;
            frameStep = animation.frameStep;
            opacityStep = animation.opacityStep;
            alpha = animation.alpha;

            // Deep clone Nominal/Linear objects
            frame = animation.frame.Clone();
            opacity = animation.opacity.Clone();
            xyScale = animation.xyScale.Clone();

            frames = [];
            foreach (Frame originalFrame in animation.frames)
            {
                Frame clonedFrame = originalFrame.Clone();
                frames.Add(clonedFrame);
                clonedFrame.Visible = false;
                AddChild(clonedFrame);
            }

            Reset();
        }
        public MapleAnimation(Wz_Node source)
        {
            frame = new Nominal<int>();
            opacity = new Linear<float>();
            xyScale = new Linear<float>();
            frames = new List<Frame>();

            if (source.ResolveUol().GetValue<Wz_Png>() != null)
            {
                Frame frame = new Frame(source.ResolveUol());
                frames.Add(frame);
            }
            else
            {
                SortedSet<int> frameIds = new SortedSet<int>();
                foreach (Wz_Node subNode in source.Nodes)
                {
                    if (subNode.ResolveUol().GetValue<Wz_Png>() != null)
                    {
                        int frameId = subNode.Text.ToInt();
                        if (frameId >= 0)
                        {
                            frameIds.Add(frameId);
                        }
                    }
                }

                foreach (int frameId in frameIds)
                {
                    Wz_Node subNode = source.FindNodeByPath(frameId.ToString()).ResolveUol();
                    if (subNode != null)
                    {
                        Frame frame = new Frame(subNode);
                        frames.Add(frame);
                    }
                }

                if (frames.Count == 0)
                {
                    frames.Add(new Frame());
                }

                animated = frames.Count > 1;

                Wz_Node zigzagNode = source.FindNodeByPath("zigzag");
                if (zigzagNode != null)
                {
                    zigzag = zigzagNode.GetValue<int>() == 1 ? true : false;
                }
                else
                {
                    zigzag = false;
                }
            }

            foreach (Frame f in frames)
            {
                f.Visible = false;
                AddChild(f);
            }

            Reset();
        }

        public void Interpolate(DrawArgument args)
        {
            int interFrame = frame.Get(alpha);
            float interOpacity = opacity.Get(alpha) / 255;
            float interScale = xyScale.Get(alpha) / 100;

            bool modifyOpacity = interOpacity != 1.0f;
            bool modifyScale = interScale != 1.0f;

            if (modifyOpacity || modifyScale)
            {
                frames[interFrame].SetDrawArgument(args + new DrawArgument(interScale, interScale, interOpacity));
            }
            else
            {
                frames[interFrame].SetDrawArgument(args);
            }
        }

        public void Interpolate(List<DrawArgument> args)
        {
            int interFrame = frame.Get(alpha);
            float interOpacity = this.opacity.Get(alpha) / 255;
            float interScale = xyScale.Get(alpha) / 100;

            bool modifyOpacity = interOpacity != 1.0f;
            bool modifyScale = interScale != 1.0f;

            if (modifyOpacity || modifyScale)
            {
                var transformedArgs = args.Select(a => a + new DrawArgument(interScale, interScale, interOpacity)).ToList();
                frames[interFrame].SetDrawArguments(transformedArgs);
            }
            else
            {
                frames[interFrame].SetDrawArguments(args);
            }
        }

        public void Reset()
        {
            frame.Set(0);
            opacity.SetAllValue(frames[0].StartOpacity());
            xyScale.SetAllValue(frames[0].StartScale());
            delay = frames[0].GetDelay();
            frameStep = 1;

            foreach (Frame frame in frames)
                frame.Visible = false;

            frames[0].Visible = true;
        }

        public bool Update(int timeStep)
        {
            Frame framedata = GetFrame();

            // Update opacity
            opacity.Add(framedata.OpacityStep(timeStep));

            if (opacity.Last() < 0.0f)
                opacity.SetAllValue(0.0f);
            else if (opacity.Last() > 255.0f)
                opacity.SetAllValue(255.0f);

            // Update xyscale
            xyScale.Add(framedata.ScaleStep(timeStep));

            if (xyScale.Last() < 0.0f)
                xyScale.SetAllValue(0.0f);

            // Handle frame updates
            if (timeStep >= delay)
            {
                int lastFrame = frames.Count - 1;
                int nextFrame;
                bool ended;

                if (zigzag && lastFrame > 0)
                {
                    if (frameStep == 1 && frame == lastFrame)
                    {
                        frameStep = -frameStep;
                        ended = false;
                    }
                    else if (frameStep == -1 && frame == 0)
                    {
                        frameStep = -frameStep;
                        ended = true;
                    }
                    else
                    {
                        ended = false;
                    }

                    nextFrame = frame + frameStep;
                }
                else
                {
                    if (frame == lastFrame)
                    {
                        nextFrame = 0;
                        ended = true;
                    }
                    else
                    {
                        nextFrame = frame + 1;
                        ended = false;
                    }
                }

                int delta = timeStep - delay;
                float threshold = (float)delta / timeStep;
                frame.Next(nextFrame, threshold);

                delay = frames[nextFrame].GetDelay();

                if (delay >= delta)
                    delay -= delta;

                opacity.SetAllValue(frames[nextFrame].StartOpacity());
                xyScale.SetAllValue(frames[nextFrame].StartScale());

                return ended;
            }
            else
            {
                frame.Normalize();
                delay -= timeStep;

                return false;
            }
        }

        public int GetDelay(int frameId)
        {
            return frameId < frames.Count ? frames[frameId].GetDelay() : 0;
        }

        public int GetDelayUntil(int frameId)
        {
            int total = 0;

            for (int i = 0; i < frameId; i++)
            {
                if (i >= frames.Count)
                {
                    break;
                }
                total += frames[i].GetDelay();
            }
            return total;
        }

        public MaplePoint<int> GetOrigin()
        {
            return GetFrame().GetOrigin();
        }

        public MaplePoint<int> GetDimension()
        {
            return GetFrame().GetDimension();
        }

        public MaplePoint<int> GetHead()
        {
            return GetFrame().GetHead();
        }

        public Frame GetFrame()
        {
            return frames[frame.Get()];
        }

        public MapleRectangle<int> GetBounds()
        {
            return GetFrame().GetBounds();
        }

        public bool IsAnimationEnd() => animationEnd;

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public override void _PhysicsProcess(double delta)
        {
            alpha = (float)Engine.GetPhysicsInterpolationFraction();

            if (frame.Last() != frame.Get())
            {
                frames[frame.Last()].Visible = false;
                frames[frame.Get()].Visible = true;
            }
            animationEnd = Update((int)(delta * speed * 1000));
        }
    }
}