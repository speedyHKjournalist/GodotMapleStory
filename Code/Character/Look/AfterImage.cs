using Godot;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class AfterImage : Node2D
    {
        private MapleAnimation animation = new();
        private MapleRectangle<int> range = new();
        private int firstFrame;
        private bool displayed;

        public override void _Ready()
        {
            AddChild(animation);
            animation!.Visible = false;
        }

        public void Init(int skillId, string name, string stanceName, int level)
        {
            string strId = skillId.ToString("D7");
            Wz_Node src = WzLib.wzs.WzNode.FindNodeByPath(true, "Skill", $"{strId.Substring(0, 3)}.img", "skill", $"{strId}", "afterimage", $"{name}", $"{stanceName}");

            if (src == null)
                src = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", "Afterimage", $"{name}.img", $"{level / 10}", $"{stanceName}");

            if (src != null)
            {
                range = new MapleRectangle<int>(src);
                firstFrame = 0;
                displayed = false;

                foreach (Wz_Node subNode in src.Nodes)
                {
                    int frame = int.TryParse(subNode.Text, out int result) ? result : 255;

                    if (frame < 255)
                    {
                        animation = new MapleAnimation(subNode);
                        firstFrame = frame;
                    }
                }
            }
            else
            {
                firstFrame = 0;
                displayed = true;
            }
        }

        public void Interpolate(DrawArgument args)
        {
            animation?.Interpolate(args);
        }

        public override void _PhysicsProcess(double delta)
        {
            int stanceFrame = GetNode<CharLook>("../look").GetFrame();
            float stanceSpeed = GetParent<Character>().GetStanceSpeed();
            animation?.SetSpeed(stanceSpeed);
            
            if (!displayed && stanceFrame >= firstFrame)
                animation!.Visible = true;
            else
                animation!.Visible = false;

            displayed = animation!.IsAnimationEnd();
        }

        public int GetFirstFrame()
        {
            return firstFrame;
        }

        public MapleRectangle<int> GetRange()
        {
            return range;
        }
    }
}