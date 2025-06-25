using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class Hair
    {
        public enum Layer : int
        {
            NONE = 0,
            DEFAULT,
            BELOWBODY,
            OVERHEAD,
            SHADE,
            BACK,
            BELOWCAP,
            BELOWCAPNARROW,
            BELOWCAPWIDE,
            NUM_LAYERS
        }

        private static Dictionary<string, Layer> layersByName = new Dictionary<string, Layer>
    {
        { "hair", Layer.DEFAULT },
        { "hairBelowBody", Layer.BELOWBODY },
        { "hairOverHead", Layer.OVERHEAD },
        { "hairShade", Layer.SHADE },
        { "backHair", Layer.BACK },
        { "backHairBelowCap", Layer.BELOWCAP },
        { "backHairBelowCapNarrow", Layer.BELOWCAPNARROW },
        { "backHairBelowCapWide", Layer.BELOWCAPWIDE }
    };
        public static Layer LayerByName(string name)
        {
            if (layersByName.ContainsKey(name))
            {
                return layersByName[name];
            }

            if (!string.IsNullOrEmpty(name))
            {
                GD.Print($"[Hair::LayerByName] Unknown Layer name: [{name}]");
            }

            return Layer.NONE;
        }

        Dictionary<int, MapleTexture>[,] stances;
        public Hair(int hairId, BodyDrawInfo drawinfo)
        {
            int stanceLength = (int)Stance.Id.LENGTH;
            int layerLength = (int)Layer.NUM_LAYERS;
            stances = new Dictionary<int, MapleTexture>[stanceLength, layerLength];

            for (int i = 0; i < stanceLength; i++)
                for (int j = 0; j < layerLength; j++)
                    stances[i, j] = [];

            Wz_Node hairNode = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", "Hair", $"000{hairId}.img");

            foreach (var keyValuePair in Stance.StanceUtils.Names)
            {
                Stance.Id stance = keyValuePair.Key;
                string stanceName = keyValuePair.Value;
                Wz_Node stanceHairNode = hairNode.FindNodeByPath(stanceName);

                if (stanceHairNode == null)
                    continue;

                Wz_Node frameNode;
                for (int frame = 0; (frameNode = stanceHairNode.FindNodeByPath(frame.ToString())) != null; frame++)
                {
                    foreach (Wz_Node layerNode in frameNode.Nodes)
                    {
                        string layerName = layerNode.Text;
                        Layer layer = LayerByName(layerName);
                        if (layer == Layer.NONE)
                            continue;

                        Wz_Vector browvec;
                        MapleTexture texture;
                        if (layerName == "hairShade")
                        {
                            browvec = layerNode.ResolveUol().FindNodeByPath("0").FindNodeByPath("map").FindNodeByPath("brow").GetValue<Wz_Vector>();
                            texture = new MapleTexture(layerNode.ResolveUol().FindNodeByPath("0"));
                        }
                        else
                        {
                            browvec = layerNode.ResolveUol().FindNodeByPath("map").FindNodeByPath("brow").GetValue<Wz_Vector>();
                            texture = new MapleTexture(layerNode.ResolveUol());
                        }
                        MaplePoint<int> brow = new MaplePoint<int>(browvec.X, browvec.Y);
                        MaplePoint<int> shift = drawinfo.GetHairPostionShift(stance, frame) - brow;
                        texture.Shift(shift);
                        stances[(int)stance, (int)layer].Add(frame, texture);
                    }
                }
            }
        }

        public void Render(CanvasItem canvas, Layer layer, Stance.Id stance, int frame, DrawArgument args)
        {
            if (stances[(int)stance, (int)layer].TryGetValue(frame, out var texture))
                texture.Render(canvas, args);
        }
    }
}