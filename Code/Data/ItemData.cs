using WzComparerR2.WzLib;

namespace MapleStory
{
    public class ItemData : Cache<ItemData>
    {
        private BoolPair<MapleTexture> icons = new();
        private int itemId;
        private int price;
        private string name = string.Empty;
        private string desc = string.Empty; //description
        private string category = string.Empty;
        private bool valid;
        private bool untradable;
        private bool unique;
        private bool unsellable;
        private bool cashItem;
        private int gender;

        public ItemData() { }

        public ItemData(int id)
        {
            Init(id);
        }

        protected override void Init(int id)
        {
            itemId = id;
            untradable = false;
            unique = false;
            unsellable = false;
            cashItem = false;
            gender = 0;

            string strPrefix = "0" + GetItemPrefix(id).ToString();
            string strId = "0" + itemId.ToString();
            int prefix = GetPrefix(itemId);

            Wz_Node? src = null;
            Wz_Node? strsrc = null;

            switch (prefix)
            {
                case 1:
                    category = GetEqCategory(itemId);
                    src = WzLib.wzs.WzNode.FindNodeByPath(true, "Character", $"{category}", $"{strId}.img", "info");
                    strsrc = WzLib.wzs.WzNode.FindNodeByPath(true, "String", "Eqp.img", "Eqp", $"{category}", itemId.ToString());
                    break;
                case 2:
                    category = "Consume";
                    src = WzLib.wzs.WzNode.FindNodeByPath(true, "Consume", $"{strPrefix}.img", $"{strId}", "info");
                    strsrc = WzLib.wzs.WzNode.FindNodeByPath(true, "String", "Consume.img", itemId.ToString());
                    break;
                case 3:
                    category = "Install";
                    src = WzLib.wzs.WzNode.FindNodeByPath(true, "Install", $"{strPrefix}.img", $"{strId}", "info");
                    strsrc = WzLib.wzs.WzNode.FindNodeByPath(true, "String", "Ins.img", itemId.ToString());
                    break;
                case 4:
                    category = "Etc";
                    src = WzLib.wzs.WzNode.FindNodeByPath(true, "Etc", $"{strPrefix}.img", $"strId", "info");
                    strsrc = WzLib.wzs.WzNode.FindNodeByPath(true, "String", "Etc.img", itemId.ToString());
                    break;
                case 5:
                    category = "Cash";
                    src = WzLib.wzs.WzNode.FindNodeByPath(true, "Cash", $"{strPrefix}.img", $"{strId}", "info");
                    strsrc = WzLib.wzs.WzNode.FindNodeByPath(true, "String", "Cash.img", itemId.ToString());
                    break;
            }

            if (src != null)
            {
                icons[false] = new MapleTexture(src.FindNodeByPath("icon"));
                icons[true] = new MapleTexture(src.FindNodeByPath("iconRaw"));
                price = src.FindNodeByPath("price")?.GetValue<int>() ?? 0;
                untradable = (src.FindNodeByPath("tradeBlock")?.GetValue<int>() ?? 0) == 1;
                unique = (src.FindNodeByPath("only")?.GetValue<int>() ?? 0) == 1;
                unsellable = (src.FindNodeByPath("notSale")?.GetValue<int>() ?? 0) == 1;
                cashItem = (src.FindNodeByPath("cash")?.GetValue<int>() ?? 0) == 1;
                gender = GetItemGender(itemId);

                name = strsrc!.FindNodeByPath("name").GetValueEx<string>(string.Empty);
                desc = strsrc!.FindNodeByPath("desc")?.GetValueEx<string>(string.Empty) ?? string.Empty;
                valid = true;
            }
            else
            {
                valid = false;
            }
        }

        public int GetPrefix(int id)
        {
            return id / 1000000;
        }

        public int GetItemPrefix(int id)
        {
            return id / 10000;
        }

        public int GetItemGender(int id)
        {
            int itemPrefix = GetItemPrefix(id);

            if ((GetPrefix(id) != 1 && itemPrefix != 254) || itemPrefix == 119 || itemPrefix == 168)
                return 2;

            int genderDigit = (id / 1000) % 10;

            return (genderDigit > 1) ? 2 : genderDigit;
        }

        public string GetCategory()
        {
            return category;
        }

        public bool IsValid()
        {
            return valid;
        }

        public bool IsUntradable()
        {
            return untradable;
        }

        public bool IsUnique()
        {
            return unique;
        }

        public bool IsUnsellable()
        {
            return unsellable;
        }

        public bool IsCashItem()
        {
            return cashItem;
        }

        public int GetId()
        {
            return itemId;
        }

        public int GetPrice()
        {
            return price;
        }

        public int GetGender()
        {
            return gender;
        }

        public string GetName()
        {
            return name;
        }

        public string GetDesc()
        {
            return desc;
        }

        public MapleTexture GetIcon(bool raw)
        {
            return icons[raw].Clone();
        }
        private string GetEqCategory(int id)
        {
            string[] categoryNames = new string[]
            {
                "Cap",
                "Accessory",
                "Accessory",
                "Accessory",
                "Coat",
                "Longcoat",
                "Pants",
                "Shoes",
                "Glove",
                "Shield",
                "Cape",
                "Ring",
                "Accessory",
                "Accessory",
                "Accessory"
            };

            int index = GetItemPrefix(id) - 100;
            if (index >= 0 && index < 15)
                return categoryNames[index];
            else if (index >= 30 && index <= 70)
                return "Weapon";
            else
                return string.Empty;
        }
    }
}