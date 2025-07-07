using Godot;
using System;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class Background : Node2D
    {
        public enum Type : int
        {
            NORMAL = 0,
            HTILED,
            VTILED,
            TILED,
            HMOVEA,
            VMOVEA,
            HMOVEB,
            VMOVEB
        }

        MapleAnimation animation = new();
        MovingObject moveObject = new();

        private int WOFFSET;
        private int HOFFSET;

        private bool animated;
        private int cx;
        private int cy;
        private double rx;
        private double ry;
        private int hTile;
        private int vTile;
        private float opacity;
        private bool flipped;

        private Camera? camera;

        public override void _Ready()
        {
            camera = GetNode<Camera>($"/root/Root/ViewportContainer/SubViewport/Stage/Camera");

            AddChild(animation);
            AddChild(moveObject);
        }

        public void Init(Wz_Node source)
        {
            WOFFSET = Constants.Constant.VWIDTH / 2;
            HOFFSET = Constants.Constant.VHEIGHT / 2;

            int ani = source.FindNodeByPath("ani")?.GetValue<int>() ?? 0;
            animated = (ani == 1) ? true : false;
            string resourceUrl = string.Format("Map/Back/{0}.img/{1}/{2}",
                source.FindNodeByPath("bS").GetValue<string>(),
                ani switch
                {
                    0 => "back",
                    1 => "ani",
                    2 => "spine",
                    _ => throw new Exception($"unknown back ani={ani} at {source.FullPathToFile}"),
                },
                source.FindNodeByPath("no").GetValue<string>()
            );


            Wz_Node backNode = WzLib.wzs.WzNode.FindNodeByPath(true, resourceUrl.Split("/"));
            animation = new MapleAnimation(backNode);

            opacity = (float)(source.FindNodeByPath("a")?.GetValueEx<int>() ?? 0);
            int f = source.FindNodeByPath("f")?.GetValueEx<int>() ?? 0;
            flipped = (f == 1) ? true : false;
            cx = source.FindNodeByPath("cx")?.GetValueEx<int>() ?? 0;
            cy = source.FindNodeByPath("cy")?.GetValueEx<int>() ?? 0;
            rx = source.FindNodeByPath("rx")?.GetValueEx<int>() ?? 0;
            ry = source.FindNodeByPath("ry")?.GetValueEx<int>() ?? 0;

            int x = source.FindNodeByPath("x")?.GetValueEx<int>() ?? 0;
            int y = source.FindNodeByPath("y")?.GetValueEx<int>() ?? 0;

            moveObject = new MovingObject();
            moveObject.SetX(x);
            moveObject.SetY(y);

            int t = source.FindNodeByPath("type")?.GetValueEx<int>() ?? 0;
            Type type = TypeById(t);
            SetType(type);
        }

        public override void _Process(double delta)
        {
            float alpha = (float)Engine.GetPhysicsInterpolationFraction();
            MaplePoint<double> realPosition = camera!.RealPosition(alpha);
            Interpolate(realPosition);
        }

        private static Type TypeById(int id)
        {
            if (id >= (int)Type.NORMAL && id <= (int)Type.VMOVEB)
            {
                return (Type)id;
            }
            return Type.NORMAL;
        }
        public void SetType(Type type)
        {
            int dimX = (int)animation.GetDimension().X;
            int dimY = (int)animation.GetDimension().Y;

            // TODO: Double check for zero. Is this a WZ reading issue?
            if (cx == 0)
                cx = (dimX > 0) ? dimX : 1;

            if (cy == 0)
                cy = (dimY > 0) ? dimY : 1;

            hTile = 1;
            vTile = 1;

            switch (type)
            {
                case Type.HTILED:
                case Type.HMOVEA:
                    hTile = Constants.Constant.VWIDTH / cx + 3;
                    break;
                case Type.VTILED:
                case Type.VMOVEA:
                    vTile = Constants.Constant.VHEIGHT / cy + 3;
                    break;
                case Type.TILED:
                case Type.HMOVEB:
                case Type.VMOVEB:
                    hTile = Constants.Constant.VWIDTH / cx + 3;
                    vTile = Constants.Constant.VHEIGHT / cy + 3;
                    break;
            }

            switch (type)
            {
                case Type.HMOVEA:
                case Type.HMOVEB:
                    moveObject.hspeed = rx / 16;
                    break;
                case Type.VMOVEA:
                case Type.VMOVEB:
                    moveObject.vspeed = ry / 16;
                    break;
            }
        }

        private void Interpolate(MaplePoint<double> cameraRealPosition)
        {
            double x;

            if (moveObject.HMobile())
            {
                x = moveObject.GetAbsoluteX(cameraRealPosition.X);
            }
            else
            {
                double shiftX = rx * (WOFFSET - cameraRealPosition.X) / 100 + WOFFSET;
                x = moveObject.GetAbsoluteX(shiftX);
            }

            double y;

            if (moveObject.VMobile())
            {
                y = moveObject.GetAbsoluteY(cameraRealPosition.Y);
            }
            else
            {
                double shiftY = ry * (HOFFSET - cameraRealPosition.Y) / 100 + HOFFSET;
                y = moveObject.GetAbsoluteY(shiftY);
            }

            if (hTile == 1 && vTile == 1)
            {
                animation.Interpolate(new DrawArgument(new MaplePoint<int>((int)Math.Round(x), (int)Math.Round(y)), flipped, opacity / 255));
            }
            else
            {
                if (hTile > 1)
                {
                    while (x > 0)
                        x -= cx;

                    while (x < -cx)
                        x += cx;
                }

                if (vTile > 1)
                {
                    while (y > 0)
                        y -= cy;

                    while (y < -cy)
                        y += cy;
                }

                int ix = (int)Math.Round(x);
                int iy = (int)Math.Round(y);

                int tw = cx * hTile;
                int th = cy * vTile;

                List<DrawArgument> drawArguments = new();
                for (int tx = 0; tx < tw; tx += cx)
                    for (int ty = 0; ty < th; ty += cy)
                        drawArguments.Add(new DrawArgument(new MaplePoint<int>(ix + tx, iy + ty), flipped, opacity));

                animation.Interpolate(drawArguments);
            }
        }
    }
}