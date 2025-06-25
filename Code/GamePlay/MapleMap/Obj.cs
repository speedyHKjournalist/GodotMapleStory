using Godot;
using WzComparerR2.WzLib;

namespace MapleStory
{
    // Represents a map decoration (object) on a map
    public partial class Obj : Node2D
    {
        private MapleAnimation animation;
        private MaplePoint<int> position;
        private int zIndex;
        private bool flip;

        private MaplePoint<int> cameraPosition;
        public override void _Ready()
        {
            Stage stage = GetNode<Stage>($"/root/Root/ViewportContainer/SubViewport/Stage");
            stage.CameraPositionChanged += OnCameraPositionChanged;

            AddChild(animation);
        }

        public Obj(Wz_Node source)
        {
            string resourceUrl = string.Format("Map/Obj/{0}.img/{1}/{2}/{3}",
                source.FindNodeByPath("oS").GetValue<string>(),
                source.FindNodeByPath("l0").GetValue<string>(),
                source.FindNodeByPath("l1").GetValue<string>(),
                source.FindNodeByPath("l2").GetValue<string>()
            );

            Wz_Node objNode = WzLib.wzs.WzNode.FindNodeByPath(true, resourceUrl.Split('/'));

            animation = new MapleAnimation(objNode);
            position = new MaplePoint<int>(source.FindNodeByPath("x").GetValueEx<int>(0), source.FindNodeByPath("y").GetValueEx<int>(0));
            flip = source.FindNodeByPath("f").GetValueEx<int>(0) == 1 ? true : false;
            zIndex = source.FindNodeByPath("z").GetValueEx<int>(0);
        }

        public void Interpolate(MaplePoint<int> viewPosition)
        {
            animation.Interpolate(new DrawArgument(position + viewPosition, flip));
        }

        public new int GetZIndex()
        {
            return zIndex;
        }

        private void OnCameraPositionChanged(int viewPositionX, int viewPositionY)
        {
            cameraPosition = new MaplePoint<int>(viewPositionX, viewPositionY);
        }

        public override void _Process(double delta)
        {
            Interpolate(cameraPosition);
        }
    }
}