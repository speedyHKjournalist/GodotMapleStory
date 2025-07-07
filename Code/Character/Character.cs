// Base for characters, e.g. the player and other clients on the same map.
using Godot;
using System;

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

        protected CharLook? look;
        protected State state;
        private EffectLayer? effects;

        private TimedBool invincible = new();
        private TimedBool ironBody = new();

        protected bool attacking = false;
        protected bool facingRight;

        private ChatBalloon chatBalloon = GD.Load<PackedScene>("res://Scene/NpcDialogue.tscn").Instantiate<ChatBalloon>();
        private Label nameLabel = new();

        private Camera? camera;

        public override void _Ready()
        {
            base._Ready();

            ConfigureLabels();

            effects = GetNode<EffectLayer>("effects");
            look = GetNode<CharLook>("look");

            effects.ZIndex = 0;
            look.ZIndex = 0;
            nameLabel.ZIndex = 3;
            chatBalloon.ZIndex = 4;

            AddChild(nameLabel);
            AddChild(chatBalloon);
            chatBalloon.Init(0, Colors.Black);

            camera = GetNode<Camera>($"/root/Root/ViewportContainer/SubViewport/Stage/Camera");
            LayerChanged += OnLayerChanged;
        }

        public void Init(int objectId, LookEntry lookEntry, string name)
        {
            nameLabel.Text = name;
            base.Init(objectId, new MaplePoint<int>());
            look?.Init(lookEntry);
        }

        // Return a reference to this characters's physics
        public PhysicsObject GetPhysicObject()
        {
            return physicsObject;
        }

        public bool IsClimbing()
        {
            return state == State.LADDER || state == State.ROPE;
        }

        public virtual void SetState(State newState)
        {
            state = newState;
            Stance.Id stance = Stance.StanceUtils.ByState((int)newState);
            look?.SetStance(stance);
        }

        public virtual void SetDirection(bool flipped)
        {
            facingRight = flipped;
            look?.SetDirection(flipped);
        }

        private void ConfigureLabels()
        {
            Font font = GD.Load<Font>("res://Fonts/arial.ttf");

            nameLabel.AddThemeFontOverride("font", font);
            nameLabel.AddThemeFontSizeOverride("font_size", 13);
            nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            nameLabel.VerticalAlignment = VerticalAlignment.Center;
            nameLabel.Modulate = Colors.White;

            StyleBoxFlat nameTagBackground = new StyleBoxFlat
            {
                BgColor = new Color(0, 0, 0, 0.5f),
                CornerRadiusBottomLeft = 4,
                CornerRadiusBottomRight = 4,
                CornerRadiusTopLeft = 4,
                CornerRadiusTopRight = 4,
                ContentMarginLeft = 8,
                ContentMarginRight = 8,
                ContentMarginTop = 2,
                ContentMarginBottom = 2
            };

            nameLabel.AddThemeStyleboxOverride("normal", nameTagBackground);
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
            float alpha = (float)Engine.GetPhysicsInterpolationFraction();
            MaplePoint<double> realPosition = camera!.RealPosition(alpha);
            MaplePoint<int> absPosition = physicsObject.GetAbsolute(realPosition.X, realPosition.Y);

            effects?.Interpolate(absPosition);
            MapleColor color;

            if (invincible == true)
            {
                float phi = invincible.Alpha() * 30f;
                float rgb = 0.9f - 0.5f * MathF.Abs(MathF.Sin(phi));

                color = new MapleColor(rgb, rgb, rgb, 1.0f);
            }
            else
                color = new(MapleColor.ColorCode.CWHITE);

            look?.Interpolate(new DrawArgument(absPosition, color));

            if (ironBody == true)
            {
                float ibalpha = ironBody.Alpha();
                float scale = 1.0f + ibalpha;
                float opacity = 1.0f - ibalpha;

                look?.Interpolate(new DrawArgument(absPosition, scale, scale, opacity));
            }

            nameLabel.GlobalPosition = (absPosition - new MaplePoint<int>((int)(nameLabel.Size.X / 2f), -2)).ToVector2();
            chatBalloon.GlobalPosition = (absPosition - new MaplePoint<int>(0, 85)).ToVector2();
        }

        public override void _PhysicsProcess(double delta)
        {
            invincible.Update((uint)(delta * 1000));
            ironBody.Update((uint)(delta * 1000));

            // Render at layer 7 when character is climbing
            if (IsClimbing())
                OnLayerChanged(0, physicsObject.footHoldLayer, (int)Layer.Id.SEVEN);

            base._PhysicsProcess(delta);
        }
    }
}