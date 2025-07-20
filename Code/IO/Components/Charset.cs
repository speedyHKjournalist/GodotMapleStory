using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class Charset
    {
        public enum Alignment
        {
            LEFT,
            CENTER,
            RIGHT
        };

        private Dictionary<int, MapleTexture> chars = [];
        private Alignment alignment;

        public Charset(Wz_Node src, Alignment alignment)
        {
            this.alignment = alignment;

            foreach (Wz_Node node in src.Nodes)
            {
                char c = node.Text[0];
                chars[c] = new MapleTexture(node);
            }
        }

        public void Render(CanvasItem canvas, int character, List<DrawArgument> args)
        {
            if (chars.TryGetValue(character, out MapleTexture? texture) && texture != null)
                foreach (DrawArgument arg in args)
                    texture.Render(canvas, arg);
        }

        public int GetWidth(int character)
        {
            return chars.TryGetValue(character, out var texture) ? texture.Width() : 0;
        }
    }
}