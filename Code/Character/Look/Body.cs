using Godot;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class Body
    {
        public enum Layer : int
        {
            NONE = 0,
            BODY,
            ARM,
            ARM_BELOW_HEAD,
            ARM_BELOW_HEAD_OVER_MAIL,
            ARM_OVER_HAIR,
            ARM_OVER_HAIR_BELOW_WEAPON,
            HAND_BELOW_WEAPON,
            HAND_OVER_HAIR,
            HAND_OVER_WEAPON,
            EAR,
            HEAD,
            HIGH_LEF_EAR,
            HUMAN_EAR,
            LEF_EAR,
            NUM_LAYERS
        };

        private static Dictionary<string, Layer> layersByName = new Dictionary<string, Layer>
    {
        { "body", Layer.BODY },
        { "backBody", Layer.BODY },
        { "arm", Layer.ARM },
        { "armBelowHead", Layer.ARM_BELOW_HEAD },
        { "armBelowHeadOverMailChest", Layer.ARM_BELOW_HEAD_OVER_MAIL },
        { "armOverHair", Layer.ARM_OVER_HAIR },
        { "armOverHairBelowWeapon", Layer.ARM_OVER_HAIR_BELOW_WEAPON },
        { "handBelowWeapon", Layer.HAND_BELOW_WEAPON },
        { "handOverHair", Layer.HAND_OVER_HAIR },
        { "handOverWeapon", Layer.HAND_OVER_WEAPON },
        { "ear", Layer.EAR },
        { "head", Layer.HEAD },
        { "backHead", Layer.HEAD },
        { "highlefEar", Layer.HIGH_LEF_EAR },
        { "humanEar", Layer.HUMAN_EAR },
        { "lefEar", Layer.LEF_EAR }
    };

        public static Layer LayerByName(string name)
        {
            if (layersByName.ContainsKey(name))
            {
                return layersByName[name];
            }

            if (!string.IsNullOrEmpty(name))
            {
                GD.Print($"[Body::LayerByName] Unknown Layer name: [{name}]");
            }

            return Layer.NONE;
        }

        private Dictionary<int, List<MapleTexture>>[,]? stances;
        private string? name;

        public Body(int skin, BodyDrawInfo drawInfo)
        {
            string skinId = skin.ToString().PadLeft(2, '0');
            int stanceLength = (int)Stance.Id.LENGTH;
            int layerLength = (int)Layer.NUM_LAYERS;
            stances = new Dictionary<int, List<MapleTexture>>[stanceLength, layerLength];

            for (int i = 0; i < (int)Stance.Id.LENGTH; i++)
                for (int j = 0; j < (int)Layer.NUM_LAYERS; j++)
                    stances[i, j] = new Dictionary<int, List<MapleTexture>>();

            Wz_Node bodyNode = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", $"000020{skinId}.img");
            Wz_Node headNode = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", $"000120{skinId}.img");

            foreach (var keyValuePair in Stance.StanceUtils.Names)
            {
                Stance.Id stance = keyValuePair.Key;
                string stanceName = keyValuePair.Value;

                Wz_Node stanceBodyNode = bodyNode.FindNodeByPath(Stance.StanceUtils.Names[stance]);
                Wz_Node stanceHeadNode = headNode.FindNodeByPath(Stance.StanceUtils.Names[stance]);

                if (stanceBodyNode == null)
                {
                    continue;
                }

                Wz_Node frameNode;
                MaplePoint<int> shift;
                for (int frame = 0; (frameNode = stanceBodyNode.FindNodeByPath(frame.ToString())) != null; frame++)
                {
                    foreach (Wz_Node partNode in frameNode.Nodes)
                    {
                        string part = partNode.Text;
                        if (part != "delay" && part != "face")
                        {
                            string zstr;
                            Wz_Node mapNodes;
                            if (partNode.GetValue<Wz_Uol>() == null)
                            {
                                Wz_Node zNode = partNode.FindNodeByPath("z");
                                if (zNode == null)
                                {
                                    continue;
                                }
                                zstr = zNode.GetValue<string>();
                                mapNodes = partNode.FindNodeByPath("map");
                            }
                            else
                            {
                                Wz_Node zNode = partNode.ResolveUol().FindNodeByPath("z");
                                if (zNode == null)
                                {
                                    continue;
                                }
                                zstr = zNode.GetValue<string>();
                                mapNodes = partNode.ResolveUol().FindNodeByPath("map");
                            }
                            Body.Layer z = Body.LayerByName(zstr);

                            switch (z)
                            {
                                case Body.Layer.HAND_BELOW_WEAPON:
                                    Wz_Node handMove = mapNodes.FindNodeByPath("handMove");
                                    shift = drawInfo.GetHandPostionShift(stance, frame) - ((handMove == null) ? new MaplePoint<int>(0, 0) : new MaplePoint<int>(handMove.GetValue<Wz_Vector>().X, handMove.GetValue<Wz_Vector>().Y));
                                    break;
                                default:
                                    Wz_Node navel = mapNodes.FindNodeByPath("navel");
                                    shift = drawInfo.GetBodyPostionShift(stance, frame) - ((navel == null) ? new MaplePoint<int>(0, 0) : new MaplePoint<int>(navel.GetValue<Wz_Vector>().X, navel.GetValue<Wz_Vector>().Y));
                                    break;
                            }

                            MapleTexture texture = new MapleTexture(partNode);
                            texture.Shift(shift);
                            if (!stances[(int)stance, (int)z].ContainsKey(frame))
                            {
                                stances[(int)stance, (int)z][frame] = new List<MapleTexture>();
                            }
                            stances[(int)stance, (int)z][frame].Add(texture);
                        }
                    }

                    if (stanceHeadNode != null)
                    {
                        Wz_Node headPartNode = stanceHeadNode.FindNodeByPath(frame.ToString()).FindNodeByPath("head").ResolveUol();
                        Wz_Node zNode = headPartNode.FindNodeByPath("z");
                        string zstr = zNode.GetValue<string>();
                        Body.Layer z = Body.LayerByName(zstr);
                        shift = drawInfo.GetHeadPostionShift(stance, frame);

                        MapleTexture texture = new MapleTexture(headPartNode);
                        texture.Shift(shift);

                        if (!stances[(int)stance, (int)z].ContainsKey(frame))
                        {
                            stances[(int)stance, (int)z][frame] = new List<MapleTexture>();
                        }
                        stances[(int)stance, (int)z][frame].Add(texture);
                    }
                }
            }

            const int NUM_SKINTYPES = 13;
            string[] skinTypes = new string[]
            {
            "Light",
            "Tan",
            "Dark",
            "Pale",
            "Ashen",
            "Green",
            "",
            "",
            "",
            "Ghostly",
            "Pale Pink",
            "Clay",
            "Alabaster"
            };

            if (skin < NUM_SKINTYPES)
                name = skinTypes[skin];

            if (name == "")
            {
                GD.Print("Skin [" + skin + "] is using the default value.");
                name = WzLib.wzs.WzNode.FindNodeByPath($"String\\Eqp.img\\Eqp\\Skin\\{skin}\\name").GetValue<string>();
            }
        }

        public void Render(CanvasItem canvas, Layer layer, Stance.Id stance, int frame, DrawArgument args)
        {
            if (stances![(int)stance, (int)layer].TryGetValue(frame, out var textureList))
                foreach (MapleTexture texture in textureList)
                    texture.Render(canvas, args);
        }
    }
}