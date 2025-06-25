using WzComparerR2.WzLib;

namespace MapleStory
{
    public class EquipData : Cache<EquipData>
    {
        private ItemData? itemData;
        private string type = string.Empty;
        private bool cash;
        private bool tradeBlock;
        private int slots;
        private EquipSlot.Id eqslot;

        private EnumMap<MapleStat.Id, int> reqStats = new();
        private EnumMap<EquipStat.Id, int> defStats = new();

        public EquipData() { }

        public EquipData(int id)
        {
            Init(id);
        }

        protected override void Init(int id)
        {
            itemData = new ItemData(id);
            string strId = "0" + id.ToString();
            string category = itemData.GetCategory();
            Wz_Node src = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", $"{category}", $"{strId}.img", "info");
            cash = src.FindNodeByPath("cash").GetValue<int>() != 0;
            int tradeBlockValue = src.FindNodeByPath("tradeBlock")?.GetValue<int>() ?? 0;
            tradeBlock = tradeBlockValue != 0;
            slots = src.FindNodeByPath("tuc")?.GetValue<int>() ?? 0;

            reqStats[MapleStat.Id.LEVEL] = src.FindNodeByPath("reqLevel").GetValue<int>();
            reqStats[MapleStat.Id.JOB] = src.FindNodeByPath("reqJob").GetValue<int>();
            reqStats[MapleStat.Id.STR] = src.FindNodeByPath("reqSTR").GetValue<int>();
            reqStats[MapleStat.Id.DEX] = src.FindNodeByPath("reqDEX").GetValue<int>();
            reqStats[MapleStat.Id.INT] = src.FindNodeByPath("reqINT").GetValue<int>();
            reqStats[MapleStat.Id.LUK] = src.FindNodeByPath("reqLUK").GetValue<int>();
            defStats[EquipStat.Id.STR] = src.FindNodeByPath("incSTR")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.DEX] = src.FindNodeByPath("incDEX")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.INT] = src.FindNodeByPath("incINT")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.LUK] = src.FindNodeByPath("incLUK")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.WATK] = src.FindNodeByPath("incPAD")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.WDEF] = src.FindNodeByPath("incPDD")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.MAGIC] = src.FindNodeByPath("incMAD")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.MDEF] = src.FindNodeByPath("incMDD")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.HP] = src.FindNodeByPath("incMHP")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.MP] = src.FindNodeByPath("incMMP")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.ACC] = src.FindNodeByPath("incACC")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.AVOID] = src.FindNodeByPath("incEVA")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.HANDS] = src.FindNodeByPath("incHANDS")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.SPEED] = src.FindNodeByPath("incSPEED")?.GetValue<int>() ?? 0;
            defStats[EquipStat.Id.JUMP] = src.FindNodeByPath("incJUMP")?.GetValue<int>() ?? 0;

            const int NonWeaponTypes = 15;
            const int WeaponOffset = NonWeaponTypes + 15;
            const int WeaponTypes = 20;

            int index = (id / 10000) - 100;

            if (index >= 0 && index < NonWeaponTypes)
            {
                string[] types =
                [
                    "HAT",
                    "FACE ACCESSORY",
                    "EYE ACCESSORY",
                    "EARRINGS",
                    "TOP",
                    "OVERALL",
                    "BOTTOM",
                    "SHOES",
                    "GLOVES",
                    "SHIELD",
                    "CAPE",
                    "RING",
                    "PENDANT",
                    "BELT",
                    "MEDAL"
                ];

                EquipSlot.Id[] equipSlots = new EquipSlot.Id[]
                {
                    EquipSlot.Id.HAT,
                    EquipSlot.Id.FACE,
                    EquipSlot.Id.EYEACC,
                    EquipSlot.Id.EARACC,
                    EquipSlot.Id.TOP,
                    EquipSlot.Id.TOP,
                    EquipSlot.Id.BOTTOM,
                    EquipSlot.Id.SHOES,
                    EquipSlot.Id.GLOVES,
                    EquipSlot.Id.SHIELD,
                    EquipSlot.Id.CAPE,
                    EquipSlot.Id.RING1,
                    EquipSlot.Id.PENDANT1,
                    EquipSlot.Id.BELT,
                    EquipSlot.Id.MEDAL
                };

                type = types[index];
                eqslot = equipSlots[index];
            }
            else if (index >= WeaponOffset && index < WeaponOffset + WeaponTypes)
            {
                string[] types =
                [
                    "ONE-HANDED SWORD",
                    "ONE-HANDED AXE",
                    "ONE-HANDED MACE",
                    "DAGGER",
                    "", "", "",
                    "WAND",
                    "STAFF",
                    "",
                    "TWO-HANDED SWORD",
                    "TWO-HANDED AXE",
                    "TWO-HANDED MACE",
                    "SPEAR",
                    "POLEARM",
                    "BOW",
                    "CROSSBOW",
                    "CLAW",
                    "KNUCKLE",
                    "GUN"
                ];

                int weaponIndex = index - WeaponOffset;
                type = types[weaponIndex];
                eqslot = EquipSlot.Id.WEAPON;
            }
            else
            {
                type = "CASH";
                eqslot = EquipSlot.Id.NONE;
            }
        }
        public EquipSlot.Id GetEquipSlot()
        {
            return eqslot;
        }
        public ItemData? GetItemData()
        {
            return itemData;
        }
        public int GetReqStat(MapleStat.Id stat)
        {
            return reqStats[stat];
        }
        public bool IsValid()
        {
            if (itemData == null) return false;
            return itemData.IsValid();
        }
        public bool IsWeapon()
        {
            return eqslot == EquipSlot.Id.WEAPON;
        }

        public int GetDefStat(EquipStat.Id stat)
        {
            return defStats[stat];
        }

        public EquipSlot.Id GetEqslot()
        {
            return eqslot;
        }

        public new string GetType()
        {
            return type;
        }
    }
}