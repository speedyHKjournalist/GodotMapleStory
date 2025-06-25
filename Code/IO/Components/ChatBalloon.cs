using Godot;

namespace MapleStory
{
    public partial class ChatBalloon : Control
    {
        // How long a line stays on screen
        private const int DURATION = 4000; // 4 seconds
        private const float MAX_TEXT_WIDTH = 100;
        private MapleFrame? frame;
        private TextureRect? arrow;
        private MarginContainer? textContainer;
        private Label? ballonText;
        private Timer? displayTimer;
        private bool _needsLayoutUpdate = false;

        public override void _Ready()
        {
            frame = GetNode<MapleFrame>("frame");
            textContainer = GetNode<MarginContainer>("textContainer");
            ballonText = GetNode<Label>("textContainer/text");
            arrow = GetNode<TextureRect>("arrow");
            displayTimer = GetNode<Timer>("displayTimer");

            if (!displayTimer.IsConnected("timeout", Callable.From(HideDialogue)))
            {
                displayTimer.Timeout += HideDialogue;
            }

        }
        public void Init(int type, Color color)
        {
            Visible = false;

            arrow!.Texture = MapleFrame.LoadTexture($"res://Assets/ChatBalloon/{type}/arrow.png");
            arrow!.StretchMode = TextureRect.StretchModeEnum.Keep;
            arrow!.ExpandMode = TextureRect.ExpandModeEnum.KeepSize;

            frame?.Init($"res://Assets/ChatBalloon/{type}/");
            ballonText!.AddThemeColorOverride("font_color", color);
        }

        public void ChangeText(string text)
        {
            Visible = true;

            ballonText!.Text = text;
            ballonText.AutowrapMode = TextServer.AutowrapMode.WordSmart;
            ballonText.HorizontalAlignment = HorizontalAlignment.Center;
            ballonText.VerticalAlignment = VerticalAlignment.Center;

            textContainer!.CustomMinimumSize = new Vector2(MAX_TEXT_WIDTH, 0);
            textContainer!.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
            textContainer!.SizeFlagsVertical = SizeFlags.Expand;

            ballonText.SizeFlagsHorizontal = SizeFlags.Fill;
            ballonText.SizeFlagsVertical = SizeFlags.Expand;
            ballonText.CustomMinimumSize = new Vector2(0, 0);

            _needsLayoutUpdate = true;
        }

        public override void _Process(double delta)
        {
            if (_needsLayoutUpdate)
            {
                _needsLayoutUpdate = false;

                Vector2 containerSize = textContainer!.GetCombinedMinimumSize();

                float framePaddingX = 10;
                float framePaddingY = 10;
                float finalFrameWidth = containerSize.X + framePaddingX * 2; // Assuming 10px padding on each side
                float finalFrameHeight = containerSize.Y + framePaddingY * 2; // Assuming 10px padding on top/bottom

                frame?.ResetLayout((int)finalFrameWidth, (int)finalFrameHeight);
                textContainer!.Position = new Vector2(-finalFrameWidth / 2 + framePaddingX, -(finalFrameHeight - 5) + framePaddingY);
                frame!.Position = new Vector2(-finalFrameWidth / 2, -(finalFrameHeight - 5));

                displayTimer!.WaitTime = DURATION / 1000.0;
                displayTimer.Start();

                QueueRedraw();
            }
        }

        public void HideDialogue()
        {
            Visible = false;
            displayTimer?.Stop();
        }
    }
}
