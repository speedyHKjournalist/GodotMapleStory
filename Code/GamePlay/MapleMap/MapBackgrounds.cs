using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class MapBackgrounds : Node2D
    {
        private List<Background> backgrounds = new();
        private List<Background> foregrounds = new();
        private bool black;

        public void Init(Wz_Node source)
        {
            int no = 0;
            Wz_Node? backNode = source?.FindNodeByPath($"{no.ToString()}") ?? null;

            while (backNode != null)
            {
                Wz_Node frontNode = backNode.FindNodeByPath("front");
                bool front;
                if (frontNode != null)
                    front = frontNode.GetValue<int>() == 0 ? false : true;
                else
                    front = false;

                if (no == 0 && source?.FindNodeByPath("0").FindNodeByPath("bS").GetValue<string>() == string.Empty)
                {
                    no++;
                    backNode = source.FindNodeByPath($"{no.ToString()}");
                    continue;
                }

                Background back = new Background();
                back.Init(backNode);

                if (front)
                {
                    GetNode<CanvasLayer>("foregroundCanvas").AddChild(back);
                    foregrounds.Add(back);
                    back.ZIndex = no;
                }
                else
                {
                    GetNode<CanvasLayer>("backgroundCanvas").AddChild(back);
                    backgrounds.Add(back);
                    back.ZIndex = no;
                }

                no++;
                backNode = source?.FindNodeByPath($"{no.ToString()}");
            }

            black = source?.FindNodeByPath("0\\bS")?.GetValue<string>() == string.Empty;

            if (black)
            {
                ColorRect blackBackground = new ColorRect();
                blackBackground.Color = Colors.Black;
                blackBackground.Size = new Vector2(Constants.Constant.VWIDTH, Constants.Constant.VHEIGHT);
                blackBackground.ZIndex = -1;
                GetNode<CanvasLayer>("backgroundCanvas").AddChild(blackBackground);
            }
        }
    }
}