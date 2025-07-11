using Godot;

namespace MapleStory
{
    public partial class MapObject : Node2D
    {
        protected int objectId;
        protected bool active;
        protected Physics? physics;
        protected PhysicsObject physicsObject = new();

        [Signal]
        public delegate void LayerChangedEventHandler(int objectId, int oldLayer, int newLayer);

        public override void _Ready()
        {
            physics = GetNode<Physics>("/root/Root/ViewportContainer/SubViewport/Stage/Physics");
            AddChild(physicsObject);
        }

        public void Init(int objectId, MaplePoint<int> position)
        {
            active = true;
            this.objectId = objectId;
            SetPosition(position);
        }

        public void SetPosition(double x, double y)
        {
            physicsObject.SetX(x);
            physicsObject.SetY(y);
        }

        public void SetPosition(MaplePoint<int> position)
        {
            SetPosition(position.X, position.Y);
        }

        virtual public void MakeActive()
        {
            active = true;
        }

        virtual public void Deactivate()
        {
            active = false;
        }

        virtual public bool IsActive()
        {
            return active;
        }

        virtual public int GetLayer()
        {
            return physicsObject.footHoldLayer;
        }

        public int GetObjectId()
        {
            return objectId;
        }

        public new MaplePoint<int> GetPosition()
        {
            return physicsObject.GetPosition();
        }

        public override void _PhysicsProcess(double delta)
        {
            int oldLayer = physicsObject.footHoldLayer;
            physics?.MoveObject(physicsObject);
            int newLayer = physicsObject.footHoldLayer;

            if (newLayer != oldLayer)
                EmitSignal(SignalName.LayerChanged, objectId, oldLayer, newLayer);
        }
    }
}