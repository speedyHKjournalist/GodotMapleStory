using Godot;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class Tile : Node2D
    {
        private MapleTexture texture;
        private MaplePoint<int> position;
        int zIndex;

        private Camera? camera;

        public override void _Ready()
        {
            camera = GetNode<Camera>($"/root/Root/ViewportContainer/SubViewport/Stage/Camera");
        }

        public Tile(Wz_Node source, string tileSet)
        {
            string resourceUrl = string.Format("Map/Tile/{0}/{1}/{2}",
                tileSet,
                source.FindNodeByPath("u").GetValue<string>(),
                source.FindNodeByPath("no").GetValue<int>());
            Wz_Node dsrc = WzLib.wzs.WzNode.FindNodeByPath(true, resourceUrl.Split("/"));
            Wz_Node _outlink = dsrc.FindNodeByPath("_outlink");

            if (_outlink != null)
            {
                string path = _outlink.GetValue<string>();
                string delimiter = "/";

                string file = string.Empty;
                int pos = path.IndexOf(delimiter);

                if (pos != -1)
                {
                    file = path.Substring(0, pos);
                    path = path.Substring(pos + delimiter.Length);
                }

                if (file == "Map")
                    dsrc = WzLib.wzs.WzNode.FindNodeByPath(true, "Map", $"{path}");
                else
                    GD.Print("Tile::Tile file not handled: " + file);
            }

            texture = new MapleTexture(dsrc);
            position = new MaplePoint<int>(source.FindNodeByPath("x").GetValue<int>(),
                                        source.FindNodeByPath("y").GetValue<int>());
            zIndex = dsrc.FindNodeByPath("z").GetValueEx<int>(0);

            if (zIndex == 0)
                zIndex = dsrc.FindNodeByPath("zM")?.GetValue<int>() ?? 0;

            Position = position.ToVector2();
        }

        public new int GetZIndex()
        {
            return zIndex;
        }

        public override void _Process(double delta)
        {
            QueueRedraw();
        }

        public override void _Draw()
        {
            float alpha = (float)Engine.GetPhysicsInterpolationFraction();
            MaplePoint<int> viewPosition = camera!.CurrentPosition(alpha);
            texture.Render(this, new DrawArgument(viewPosition));
        }
    }
}