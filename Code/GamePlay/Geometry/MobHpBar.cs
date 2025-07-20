using Godot;

namespace MapleStory
{
    public partial class MobHpBar : Node2D
    {
        [Export]
        public Vector2 BarSize { get; set; } = new Vector2(40, 4);

        [Export]
        public Color BackgroundColor { get; set; } = new Color(0.2f, 0.2f, 0.2f);

        [Export]
        public Color FillColor { get; set; } = new Color(0.8f, 0.1f, 0.1f);

        [Export]
        public Color BorderColor { get; set; } = Colors.Black;

        private int _hpPercent = 100;

        public void Interpolate(int hpPercent)
        {
            int newPercent = Mathf.Clamp(hpPercent, 0, 100);

            if (newPercent != _hpPercent)
            {
                _hpPercent = newPercent;
                QueueRedraw();
            }
        }

        public override void _Draw()
        {
            float fillWidth = BarSize.X * (_hpPercent / 100.0f);

            DrawRect(new Rect2(Vector2.Zero, BarSize), BackgroundColor);
            if (fillWidth > 0)
                DrawRect(new Rect2(Vector2.Zero, new Vector2(fillWidth, BarSize.Y)), FillColor);
            DrawRect(new Rect2(Vector2.Zero, BarSize), BorderColor, false, 1.0f);
        }
    }
}