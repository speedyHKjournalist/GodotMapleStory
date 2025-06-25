using Godot;
using System;
using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class Clothing
    {
        // Defaults when no clothes are equipped
        public readonly static int TOP_DEFAULT_ID = 1040006;
        public readonly static int BOTTOM_DEFAULT_ID = 1060026;
        private static readonly HashSet<int> transparents = new HashSet<int>{ 1002186 };

        public enum Layer
        {
            CAPE,
            SHOES,
            PANTS,
            TOP,
            MAIL,
            MAILARM,
            EARRINGS,
            FACEACC,
            EYEACC,
            PENDANT,
            BELT,
            MEDAL,
            RING,
            CAP,
            CAP_BELOW_BODY,
            CAP_OVER_HAIR,
            GLOVE,
            WRIST,
            GLOVE_OVER_HAIR,
            WRIST_OVER_HAIR,
            GLOVE_OVER_BODY,
            WRIST_OVER_BODY,
            SHIELD,
            BACKSHIELD,
            SHIELD_BELOW_BODY,
            SHIELD_OVER_HAIR,
            WEAPON,
            BACKWEAPON,
            WEAPON_BELOW_ARM,
            WEAPON_BELOW_BODY,
            WEAPON_OVER_HAND,
            WEAPON_OVER_BODY,
            WEAPON_OVER_GLOVE,
            NUM_LAYERS
        };

        public static readonly Dictionary<string, Layer> sublayerNames = new Dictionary<string, Layer>
        {
            // WEAPON
            { "weaponOverHand", Layer.WEAPON_OVER_HAND },
            { "weaponOverGlove", Layer.WEAPON_OVER_GLOVE },
            { "weaponOverBody", Layer.WEAPON_OVER_BODY },
            { "weaponBelowArm", Layer.WEAPON_BELOW_ARM },
            { "weaponBelowBody", Layer.WEAPON_BELOW_BODY },
            { "backWeaponOverShield", Layer.BACKWEAPON },
            // SHIELD
            { "shieldOverHair", Layer.SHIELD_OVER_HAIR },
            { "shieldBelowBody", Layer.SHIELD_BELOW_BODY },
            { "backShield", Layer.BACKSHIELD },
            // GLOVE
            { "gloveWrist", Layer.WRIST },
            { "gloveOverHair", Layer.GLOVE_OVER_HAIR },
            { "gloveOverBody", Layer.GLOVE_OVER_BODY },
            { "gloveWristOverHair", Layer.WRIST_OVER_HAIR },
            { "gloveWristOverBody", Layer.WRIST_OVER_BODY },
            // CAP
            { "capOverHair", Layer.CAP_OVER_HAIR },
            { "capBelowBody", Layer.CAP_BELOW_BODY }
        };

        private EnumMap<Stance.Id, EnumMap<Layer, Dictionary<int, List<MapleTexture>>>> stances = new();
        private int itemId;
        private EquipSlot.Id equipSlot;
        private Stance.Id walk;
        private Stance.Id stand;
        private string vslot;
        private bool twoHanded;
        private bool transparent;

        private static Dictionary<string, Layer> subLayerNames = [];

        public Clothing(int itemId, BodyDrawInfo drawInfo)
        {
            foreach (Stance.Id stance in Enum.GetValues(typeof(Stance.Id)))
            {
                stances.Emplace(stance, new());
                foreach (Layer layer in Enum.GetValues(typeof(Layer)))
                    if (stances.TryGetValue(stance, out var map))
                        map?.Emplace(layer, []);
            }

            this.itemId = itemId;
            EquipData equipData = new EquipData(itemId);
            equipSlot = equipData.GetEquipSlot();

            int NON_WEAPON_TYPES = 15;
            int WEAPON_OFFSET = NON_WEAPON_TYPES + 15;
            int WEAPON_TYPES = 20;

            if (equipSlot == EquipSlot.Id.WEAPON)
                twoHanded = new WeaponData(itemId).IsTwoHanded();
            else
                twoHanded = false;

            Layer[] layers =
            [
                Layer.CAP,
                Layer.FACEACC,
                Layer.EYEACC,
                Layer.EARRINGS,
                Layer.TOP,
                Layer.MAIL,
                Layer.PANTS,
                Layer.SHOES,
                Layer.GLOVE,
                Layer.SHIELD,
                Layer.CAPE,
                Layer.RING,
                Layer.PENDANT,
                Layer.BELT,
                Layer.MEDAL
            ];

            Layer chlayer;
            int index = (itemId / 10000) - 100;

            if (index < NON_WEAPON_TYPES)
                chlayer = layers[index];
            else if (index >= WEAPON_OFFSET && index < WEAPON_OFFSET + WEAPON_TYPES)
                chlayer = Layer.WEAPON;
            else
                chlayer = Layer.CAPE;

            string strid = "0" + itemId.ToString();
            string category = equipData.GetItemData()!.GetCategory();
            Wz_Node src = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", $"{category}", $"{strid}.img");
            Wz_Node info = src.FindNodeByPath("info");
            vslot = info.FindNodeByPath("vslot").GetValueEx<string>("");

            int standno = info.FindNodeByPath("stand").GetValueEx<int>(0);
            stand = standno switch
            {
                1 => Stance.Id.STAND1,
                2 => Stance.Id.STAND2,
                _ => twoHanded ? Stance.Id.STAND2 : Stance.Id.STAND1,
            };

            int walkno = info.FindNodeByPath("walk").GetValueEx<int>(0);
            walk = walkno switch
            {
                1 => Stance.Id.WALK1,
                2 => Stance.Id.WALK2,
                _ => twoHanded ? Stance.Id.WALK2 : Stance.Id.WALK1,
            };

            List<Byte[]> resourceList = [];
            foreach (var keyValuePair in Stance.StanceUtils.Names)
            {
                Stance.Id stance = keyValuePair.Key;
                string stanceName = keyValuePair.Value;

                if (stance == Stance.Id.NONE || stance == Stance.Id.LENGTH)
                    continue;

                Wz_Node stanceNode = src.FindNodeByPath(stanceName);
                if (stanceNode == null)
                    continue;

                Wz_Node frameNode;
                for (int frame = 0; (frameNode = stanceNode.FindNodeByPath(frame.ToString())) != null; frame++)
                {
                    foreach (Wz_Node node in frameNode.Nodes)
                    {
                        string part = node.Text;
                        Wz_Node partNode;
                        if (node.GetValue<Wz_Uol>() != null)
                            partNode = node.ResolveUol();
                        else
                            partNode = node;

                        if (partNode.GetValue<Wz_Png>() == null)
                            continue;

                        Layer z = chlayer;
                        string zstr = partNode.FindNodeByPath("z").GetValue<string>();

                        if (part == "mailArm")
                            z = Layer.MAILARM;
                        else if (sublayerNames.TryGetValue(zstr, out Layer sublayer))
                            z = sublayer;

                        string parent = string.Empty;
                        MaplePoint<int> parentpos = new();

                        foreach (Wz_Node mapNode in partNode.FindNodeByPath("map").Nodes)
                        {
                            Wz_Vector vec = mapNode.GetValue<Wz_Vector>();
                            if (vec != null)
                            {
                                parent = mapNode.Text;
                                parentpos = new MaplePoint<int>(vec.X, vec.Y);
                            }
                        }

                        MaplePoint<int> shift = new MaplePoint<int>();

                        switch (equipSlot)
                        {
                            case EquipSlot.Id.FACE:
                                {
                                    shift -= parentpos;
                                    break;
                                }
                            case EquipSlot.Id.SHOES:
                            case EquipSlot.Id.GLOVES:
                            case EquipSlot.Id.TOP:
                            case EquipSlot.Id.BOTTOM:
                            case EquipSlot.Id.CAPE:
                                {
                                    shift = drawInfo.GetBodyPostionShift(stance, frame) - parentpos;
                                    break;
                                }
                            case EquipSlot.Id.HAT:
                            case EquipSlot.Id.EARACC:
                            case EquipSlot.Id.EYEACC:
                                {
                                    shift = drawInfo.GetFacePostionShift(stance, frame) - parentpos;
                                    break;
                                }
                            case EquipSlot.Id.SHIELD:
                            case EquipSlot.Id.WEAPON:
                                {
                                    if (parent == "handMove")
                                        shift += drawInfo.GetHandPostionShift(stance, frame);
                                    else if (parent == "hand")
                                        shift += drawInfo.GetArmPostionShift(stance, frame);
                                    else if (parent == "navel")
                                        shift += drawInfo.GetBodyPostionShift(stance, frame);

                                    shift -= parentpos;
                                    break;
                                }
                        }

                        MapleTexture texture = new(partNode);
                        texture.Shift(shift);

                        if (stances.TryGetValue(stance, out var layerMap) && layerMap != null)
                            if (layerMap.TryGetValue(z, out var frameDict) && frameDict != null)
                            {
                                var spriteList = frameDict.GetValueOrDefault(frame) ?? (frameDict[frame] = []);
                                spriteList.Add(texture);
                            }
                    }
                }
            }
            transparent = transparents.Contains(itemId);
        }

        public bool ContainsLayer(Stance.Id stance, Layer layer)
        {
            if (stances.TryGetValue(stance, out var layerMap) && layerMap != null)
                if (layerMap.TryGetValue(layer, out var frameDict) && frameDict != null)
                    return frameDict.Count > 0;

            return false;
        }
        public bool IsTransparent()
        {
            return transparent;
        }

        public bool IsTwoHanded()
        {
            return twoHanded;
        }

        public int GetId()
        {
            return itemId;
        }

        public Stance.Id GetStand()
        {
            return stand;
        }

        public Stance.Id GetWalk()
        {
            return walk;
        }

        public EquipSlot.Id GetEquipSlot()
        {
            return equipSlot;
        }

        public string GetVslot()
        {
            return vslot;
        }

        public void Render(CanvasItem canvas, Stance.Id interStance, Layer clothingLayer, int interFrame, DrawArgument args)
        {
            if (stances.TryGetValue(interStance, out var layerMap) && layerMap != null)
                if (layerMap.TryGetValue(clothingLayer, out var frameDict) && frameDict != null)
                    if (frameDict.TryGetValue(interFrame, out var textureList) && textureList != null)
                        foreach (MapleTexture texture in textureList)
                            texture.Render(canvas, args);
        }
    }
}