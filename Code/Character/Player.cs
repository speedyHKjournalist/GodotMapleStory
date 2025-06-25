namespace MapleStory
{
    public partial class Player : Character
    {
        public override void _Ready()
        {
            base._Ready();
        }

        public void Init(CharEntry entry)
        {
            base.Init(entry.id, entry.look, entry.stats.name);
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);
        }
    }
}