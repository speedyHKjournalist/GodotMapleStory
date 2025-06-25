// Base for characters, e.g. the player and other clients on the same map.
using Godot;

namespace MapleStory
{
    public partial class Character : MapObject
    {
        // Player states which determine animation and state 
        // Values are used in movement packets (Add one if facing left)
        public enum State : int
        {
            WALK = 2,
            STAND = 4,
            FALL = 6,
            ALERT = 8,
            PRONE = 10,
            SWIM = 12,
            LADDER = 14,
            ROPE = 16,
            DIED = 18,
            SIT = 20
        };

        public static State ByValue(int value)
        {
            return (State)value;
        }

        protected MaplePoint<double> cameraRealPosition = new();
        protected CharLook? look;

        public override void _Ready()
        {
            base._Ready();

            look = GetNode<CharLook>("look");
            Stage stage = GetNode<Stage>($"/root/Root/ViewportContainer/SubViewport/Stage");
            stage.CameraRealPositionChanged += OnCameraRealPositionChanged;
            LayerChanged += OnLayerChanged;
        }

        public void Init(int objectId, LookEntry lookEntry, string name)
        {
            base.Init(objectId, new MaplePoint<int>());
            look?.Init(lookEntry);
        }

        // Return a reference to this characters's physics
        public PhysicsObject GetPhysicsObject()
        {
            return physicsObject;
        }

        public override void OnCameraRealPositionChanged(double viewRealPositionX, double viewRealPositionY)
        {
            cameraRealPosition = new MaplePoint<double>(viewRealPositionX, viewRealPositionY);
        }

        public void OnLayerChanged(int objectId, int oldLayer, int newLayer)
        {
            Node2D? previousPlayerNode = GetParent<Node2D>();
            CanvasLayer targetPlayerNode = GetNode<CanvasLayer>($"/root/Root/ViewportContainer/SubViewport/Stage/Layer{newLayer}/Player_5");

            previousPlayerNode?.RemoveChild(this);
            targetPlayerNode.AddChild(this);
        }

        public override void _Process(double delta)
        {
            MaplePoint<int> absPosition = physicsObject.GetAbsolute(cameraRealPosition.X, cameraRealPosition.Y);
            MapleColor color = new (MapleColor.ColorCode.CWHITE);
            look?.Interpolate(new DrawArgument(absPosition, color));
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
        }
    }
}