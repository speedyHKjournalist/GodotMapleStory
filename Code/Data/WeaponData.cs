using System.Collections.Generic;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class WeaponData : Cache<WeaponData>
    {
        private EquipData? equipData;
        private Weapon.Type type;
        private bool twoHanded;
        private int attackSpeed;
        private int attack;
        /*    BoolPair<Sound> usesounds;*/
        private string afterImage = string.Empty;

        private static Dictionary<int, WeaponData> cache = [];

        public WeaponData() { }

        public WeaponData(int equipId)
        {
            Init(equipId);
        }

        protected override void Init(int equipId)
        {
            equipData = new EquipData(equipId);
            int prefix = equipId / 10000;
            type = Weapon.Utils.ByValue(prefix);
            twoHanded = prefix == (int)Weapon.Type.STAFF || (prefix >= (int)Weapon.Type.SWORD_2H && prefix <= (int)Weapon.Type.POLEARM) || prefix == (int)Weapon.Type.CROSSBOW;

            Wz_Node src = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", "Weapon", $"0{equipId.ToString()}.img", "info");
            attackSpeed = src.FindNodeByPath("attackSpeed").GetValue<int>();
            attack = src.FindNodeByPath("attack").GetValue<int>();

            /*        nl::node soundsrc = nl::nx::Sound["Weapon.img"][src["sfx"]];

                    bool twosounds = soundsrc["Attack2"].data_type() == nl::node::type::audio;

                    if (twosounds)
                    {
                        usesounds[false] = soundsrc["Attack"];
                        usesounds[true] = soundsrc["Attack2"];
                    }
                    else
                    {
                        usesounds[false] = soundsrc["Attack"];
                        usesounds[true] = soundsrc["Attack"];
                    }*/

            afterImage = src.FindNodeByPath("afterImage").GetValue<string>();
        }

        public bool IsTwoHanded()
        {
            return twoHanded;
        }

        public EquipData? GetEquipData()
        {
            return equipData;
        }

        public string GetAfterImage()
        {
            return afterImage;
        }

        public new Weapon.Type GetType()
        {
            return type;
        }

        public int GetSpeed()
        {
            return attackSpeed;
        }

        public int GetAttack()
        {
            return attack;
        }

        public string GetSpeedString()
        {
            switch (attackSpeed)
            {
                case 1:
                    return "FAST (1)";
                case 2:
                    return "FAST (2)";
                case 3:
                    return "FAST (3)";
                case 4:
                    return "FAST (4)";
                case 5:
                    return "NORMAL (5)";
                case 6:
                    return "NORMAL (6)";
                case 7:
                    return "SLOW (7)";
                case 8:
                    return "SLOW (8)";
                case 9:
                    return "SLOW (9)";
                default:
                    return "";
            }
        }
    }
}