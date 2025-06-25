using Godot;
using System.Collections.Generic;

namespace MapleStory
{
    // A characters equipment (The visual part)
    public class CharEquips
    {
        // Cap types (vslot)
        public enum CapType
        {
            NONE,
            HEADBAND,
            HAIRPIN,
            HALFCOVER,
            FULLCOVER
        };

        private EnumMap<EquipSlot.Id, Clothing> clothes = new();
        private static Dictionary<int, Clothing> clothCache = [];

        public CharEquips() { }

        // Add an equip, if not in cache, the equip is created from the files.
        public void AddEquip(int itemId, BodyDrawInfo drawinfo)
        {
            if (itemId <= 0)
                return;

            if (!clothCache.TryGetValue(itemId, out var cloth))
            {
                cloth = new(itemId, drawinfo);
                clothCache[itemId] = cloth;
            }

            EquipSlot.Id slot = cloth.GetEquipSlot();
            clothes[slot] = cloth;
        }

        // Remove an equip
        public void RemoveEquip(EquipSlot.Id slot)
        {
            clothes.Erase(slot);
        }

        // Return if there is an overall equipped
        public bool HasOverAll()
        {
            return GetEquip(EquipSlot.Id.TOP) / 10000 == 105;
        }

        // Return the item id of the equip at the specified slot
        public int GetEquip(EquipSlot.Id slot)
        {
            if (clothes.TryGetValue(slot, out var cloth) && cloth != null)
                return cloth.GetId();
            else
                return 0;
        }

        // Return the cap type (vslot)
        public CapType GetCapType()
        {
            if (clothes.TryGetValue(EquipSlot.Id.HAT, out var cap) && cap != null)
            {
                string vslot = cap.GetVslot();
                if (vslot == "CpH1H5")
                    return CharEquips.CapType.HALFCOVER;
                else if (vslot == "CpH1H5AyAs")
                    return CharEquips.CapType.FULLCOVER;
                else if (vslot == "CpH5")
                    return CharEquips.CapType.HEADBAND;
                else
                    return CharEquips.CapType.NONE;
            }
            else
                return CharEquips.CapType.NONE;
        }

        // Draw an equip
        public void Render(CanvasItem canvas, EquipSlot.Id equipSlotLayer, Stance.Id interStance, Clothing.Layer clothingLayer, int interFrame, DrawArgument args)
        {
            if (clothes.TryGetValue(equipSlotLayer, out var clothing) && clothing != null)
                clothing.Render(canvas, interStance, clothingLayer, interFrame, args);
        }

        // Return a stance which has been adjusted to the equipped weapon type
        public Stance.Id AdjustStance(Stance.Id stance)
        {
            if (clothes.TryGetValue(EquipSlot.Id.WEAPON, out var weapon) && weapon != null)
                switch (stance)
                {
                    case Stance.Id.STAND1:
                    case Stance.Id.STAND2:
                        return weapon.GetStand();

                    case Stance.Id.WALK1:
                    case Stance.Id.WALK2:
                        return weapon.GetWalk();

                    default:
                        return stance;
                }
            else
                return stance;
        }

        // Return whether the equipped weapon is twohanded
        public bool IsTwoHanded()
        {
            if (clothes.TryGetValue(EquipSlot.Id.WEAPON, out var weapon) && weapon != null)
                return weapon.IsTwoHanded();

            return false;
        }

        // Return the item id of the equipped weapon
        public int GetWeapon()
        {
            return GetEquip(EquipSlot.Id.WEAPON);
        }

        // Return if there is a weapon equipped
        public bool HasWeapon()
        {
            return GetWeapon() != 0;
        }
    }
}