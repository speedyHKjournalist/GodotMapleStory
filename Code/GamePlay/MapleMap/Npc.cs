// Represents a NPC on the current map
// Implements the 'MapObject' interface to be used in a 'MapObjects' template
using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class Npc : MapObject
    {
        private Dictionary<string, MapleAnimation> animations;
        private Dictionary<string, List<string>> lines;
        private List<string> states;

        private ChatBalloon chatBalloon = GD.Load<PackedScene>("res://Scene/NpcDialogue.tscn").Instantiate<ChatBalloon>();
        private Label textLabel = new();
        private Label funcLabel = new();

        private RandomNumberGenerator random;
        private MaplePoint<double> cameraRealPosition;
        private string name;
        private string func;
        private bool hideName;
        private bool scripted;
        private bool mouseOnly;

        private int npcId;
        private bool flip;
        private string stance = string.Empty;
        private bool control;

        public override void _Ready()
        {
            base._Ready();

            Stage stage = GetNode<Stage>($"/root/Root/ViewportContainer/SubViewport/Stage");
            stage.CameraRealPositionChanged += OnCameraRealPositionChanged;

            AddChild(textLabel);
            AddChild(funcLabel);
            AddChild(chatBalloon);
            chatBalloon.Init(0, Colors.Brown);
        }

        public Npc(int npcId, int objectId, bool mirrored, int footholdId, bool control, MaplePoint<int> position)
        {
            Init(objectId, position);
            random = new RandomNumberGenerator();
            animations = new Dictionary<string, MapleAnimation>();
            lines = new Dictionary<string, List<string>>();
            states = new List<string>();

            string strId = npcId.ToString().PadLeft(7, '0') + ".img";
            Wz_Node src = WzLib.wzs.WzNode.FindNodeByPath(true, "Npc", $"{strId}");
            Wz_Node stringSrc = WzLib.wzs.WzNode.FindNodeByPath(true, "String", "Npc.img", $"{npcId.ToString()}");
            string link = src.FindNodeByPath(true, "info", "link")?.GetValue<string>() ?? string.Empty;

            if (link != string.Empty)
            {
                link += ".img";
                src = WzLib.wzs.WzNode.FindNodeByPath(true, "Npc", $"{link}");
            }

            Wz_Node info = src.FindNodeByPath("info");

            hideName = (info.FindNodeByPath("hideName")?.GetValue<int>() ?? 0) == 1;
            mouseOnly = (info.FindNodeByPath("talkMouseOnly")?.GetValue<int>() ?? 0) == 1;
            scripted = ((info.FindNodeByPath("script")?.Nodes.Count ?? 0) > 0) || ((info.FindNodeByPath("shop")?.GetValue<int>() ?? 0) == 1);

            foreach (Wz_Node npcNode in src.Nodes)
            {
                string state = npcNode.Text;

                if (state != "info")
                {
                    animations[state] = new MapleAnimation(npcNode);
                    animations[state].Visible = false;
                    AddChild(animations[state]);
                    states.Add(state);
                }

                if (npcNode.FindNodeByPath("speak") != null)
                {
                    foreach (Wz_Node speakNode in npcNode.FindNodeByPath("speak").Nodes)
                    {
                        string speak = stringSrc.FindNodeByPath($"{speakNode.GetValue<string>()}")?.GetValue<string>() ?? string.Empty;
                        if (speak != string.Empty)
                        {
                            if (!lines.ContainsKey(state))
                                lines[state] = new List<string>();
                            lines[state].Add(speak);
                        }
                    }
                }
            }

            name = stringSrc.FindNodeByPath("name")?.GetValue<string>() ?? string.Empty;
            func = stringSrc.FindNodeByPath("func")?.GetValue<string>() ?? string.Empty;

            if (ContainsKoreanCharacters(func))
            {
                func = "";
            }

            ConfigureLabels();

            this.npcId = npcId;
            flip = !mirrored;
            this.control = control;
            physicsObject.footHoldId = footholdId;
            stance = "stand";
            SetStance(stance);
        }

        public void Update()
        {
            if (!active)
                return;

            if (animations.TryGetValue(stance, out MapleAnimation? anim))
            {
                bool animationEnd = anim.IsAnimationEnd();
                if (animationEnd && states.Count > 0)
                {
                    int nextStance = random.RandiRange(0, states.Count - 1);
                    SetStance(states[nextStance]);
                    if (lines.TryGetValue(states[nextStance], out List<string>? line))
                    {
                        int nextChat = random.RandiRange(0, line.Count - 1);
                        chatBalloon.ChangeText(lines[stance][nextChat]);
                    }
                }
            }
        }

        private void ConfigureLabels()
        {
            Font font = GD.Load<Font>("res://Fonts/arial.ttf");

            textLabel.AddThemeFontOverride("font", font);
            textLabel.AddThemeFontSizeOverride("font_size", 13);
            textLabel.Text = name;
            textLabel.HorizontalAlignment = HorizontalAlignment.Center;
            textLabel.VerticalAlignment = VerticalAlignment.Center;
            textLabel.Modulate = Colors.Yellow;
            textLabel.Visible = !hideName;

            funcLabel.AddThemeFontOverride("font", font);
            funcLabel.AddThemeFontSizeOverride("font_size", 13);
            funcLabel.Text = func;
            funcLabel.HorizontalAlignment = HorizontalAlignment.Center;
            funcLabel.VerticalAlignment = VerticalAlignment.Center;
            funcLabel.Modulate = Colors.Yellow;
            funcLabel.Visible = !hideName;

            StyleBoxFlat nameTagBackground = new StyleBoxFlat();
            nameTagBackground.BgColor = new Color(0, 0, 0, 0.5f);
            nameTagBackground.CornerRadiusBottomLeft = 4;
            nameTagBackground.CornerRadiusBottomRight = 4;
            nameTagBackground.CornerRadiusTopLeft = 4;
            nameTagBackground.CornerRadiusTopRight = 4;
            nameTagBackground.ContentMarginLeft = 8;
            nameTagBackground.ContentMarginRight = 8;
            nameTagBackground.ContentMarginTop = 2;
            nameTagBackground.ContentMarginBottom = 2;

            textLabel.AddThemeStyleboxOverride("normal", nameTagBackground);
            funcLabel.AddThemeStyleboxOverride("normal", nameTagBackground);
        }

        public override void _Process(double delta)
        {
            MaplePoint<int> absPosition = physicsObject.GetAbsolute(cameraRealPosition.X, cameraRealPosition.Y);

            if (animations.TryGetValue(stance, out MapleAnimation? anim))
                anim.Interpolate(new DrawArgument(absPosition, flip));
        }

        public override void OnCameraRealPositionChanged(double viewRealPositionX, double viewRealPositionY)
        {
            cameraRealPosition = new MaplePoint<double>(viewRealPositionX, viewRealPositionY);
            MaplePoint<int> absPosition = physicsObject.GetAbsolute(cameraRealPosition.X, cameraRealPosition.Y);

            textLabel.GlobalPosition = (absPosition - new MaplePoint<int>((int)(textLabel.Size.X / 2f), -2)).ToVector2();
            funcLabel.GlobalPosition = (absPosition - new MaplePoint<int>((int)(funcLabel.Size.X / 2f), -24)).ToVector2();
            chatBalloon.GlobalPosition = (absPosition - new MaplePoint<int>(-20, 70)).ToVector2();
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
            Update();
        }

        public void SetStance(string stance)
        {
            animations[this.stance].Visible = false;

            if (this.stance != stance)
            {
                this.stance = stance;
                if (animations.ContainsKey(stance))
                    animations[stance].Reset();
            }

            animations[stance].Visible = true;
        }

        public bool IsScripted()
        {
            return scripted;
        }

        public bool InRange(MaplePoint<int> cursorPosition, MaplePoint<int> viewPosition)
        {
            if (!active)
                return false;

            MaplePoint<int> absPosition = GetPosition() + viewPosition;

            MaplePoint<int> dimension = animations.ContainsKey(stance) ? animations[stance].GetDimension() : new MaplePoint<int>();

            return new MapleRectangle<int>(absPosition.X - dimension.X / 2,
                absPosition.X + dimension.X / 2,
                absPosition.Y - dimension.Y,
                absPosition.Y).Contains(cursorPosition);
        }
        public new string GetName()
        {
            return name;
        }
        public string GetFunc()
        {
            return func;
        }
        private bool ContainsKoreanCharacters(string text)
        {
            foreach (char c in text)
            {
                if (c >= 0x1100 && c <= 0x11FF) // Korean character range
                    return true;
            }
            return false;
        }
    }
}