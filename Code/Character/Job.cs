namespace MapleStory
{
    public class Job
    {
        public enum Level : int
        {
            BEGINNER = 0,
            FIRST,
            SECOND,
            THIRD,
            FOURTH
        }

        public static Level GetNextLevel(Level level)
        {
            return level switch
            {
                Level.BEGINNER => Level.FIRST,
                Level.FIRST => Level.SECOND,
                Level.SECOND => Level.THIRD,
                _ => Level.FOURTH
            };
        }

        private string name;
        private int id;
        private Level level;

        public Job(int id)
        {
            ChangeJob(id);
        }

        public Job()
        {
            ChangeJob(0);
        }

        public void ChangeJob(int id)
        {
            this.id = id;
            this.name = GetName(id);

            if (id == 0)
                level = Level.BEGINNER;
            else if (id % 100 == 0)
                level = Level.FIRST;
            else if (id % 10 == 0)
                level = Level.SECOND;
            else if (id % 10 == 1)
                level = Level.THIRD;
            else
                level = Level.FOURTH;
        }

        public bool IsSubJob(int subId)
        {
            for (Level lv = Level.BEGINNER; lv <= Level.FOURTH; lv++)
            {
                if (subId == GetSubJob(lv))
                    return true;
            }
            return false;
        }

        public bool CanUse(int skillId)
        {
            int required = skillId / 10000;
            return IsSubJob(required);
        }

        public int GetId() => id;

        public int GetSubJob(Level lv)
        {
            if (lv <= level)
            {
                return lv switch
                {
                    Level.BEGINNER => 0,
                    Level.FIRST => (id / 100) * 100,
                    Level.SECOND => (id / 10) * 10,
                    Level.THIRD => (level == Level.FOURTH) ? (id - 1) : id,
                    Level.FOURTH => id,
                    _ => 0
                };
            }
            return 0;
        }

        public string GetName() => name;

        public Level GetLevel() => level;

        private string GetName(int jobId)
        {
            return jobId switch
            {
                0 => "Beginner",
                100 => "Swordsman",
                110 => "Fighter",
                111 => "Crusader",
                112 => "Hero",
                120 => "Page",
                121 => "White Knight",
                122 => "Paladin",
                130 => "Spearman",
                131 => "Dragon Knight",
                132 => "Dark Knight",
                200 => "Magician",
                210 => "Wizard (F/P)",
                211 => "Mage (F/P)",
                212 => "Archmage (F/P)",
                220 => "Wizard (I/L)",
                221 => "Mage (I/L)",
                222 => "Archmage (I/L)",
                230 => "Cleric",
                231 => "Priest",
                232 => "Bishop",
                300 => "Archer",
                310 => "Hunter",
                311 => "Ranger",
                312 => "Bowmaster",
                320 => "Crossbowman",
                321 => "Sniper",
                322 => "Marksman",
                400 => "Rogue",
                410 => "Assassin",
                411 => "Hermit",
                412 => "Nightlord",
                420 => "Bandit",
                421 => "Chief Bandit",
                422 => "Shadower",
                500 => "Pirate",
                510 => "Brawler",
                511 => "Marauder",
                512 => "Buccaneer",
                520 => "Gunslinger",
                521 => "Outlaw",
                522 => "Corsair",
                1000 => "Noblesse",
                1100 or 1110 or 1111 or 1112 => "Dawn Warrior",
                1200 or 1210 or 1211 or 1212 => "Blaze Wizard",
                1300 or 1310 or 1311 or 1312 => "Wind Archer",
                1400 or 1410 or 1411 or 1412 => "Night Walker",
                1500 or 1510 or 1511 or 1512 => "Thunder Breaker",
                2000 or 2100 or 2110 or 2111 or 2112 => "Aran",
                900 => "GM",
                910 => "SuperGM",
                _ => ""
            };
        }

        public EquipStat.Id GetPrimary(Weapon.Type weaponType)
        {
            return (id / 100) switch
            {
                2 => EquipStat.Id.INT,
                3 => EquipStat.Id.DEX,
                4 => EquipStat.Id.LUK,
                5 => (weaponType == Weapon.Type.GUN) ? EquipStat.Id.DEX : EquipStat.Id.STR,
                _ => EquipStat.Id.STR
            };
        }

        public EquipStat.Id GetSecondary(Weapon.Type weaponType)
        {
            return (id / 100) switch
            {
                2 => EquipStat.Id.LUK,
                3 => EquipStat.Id.STR,
                4 => EquipStat.Id.DEX,
                5 => (weaponType == Weapon.Type.GUN) ? EquipStat.Id.STR : EquipStat.Id.DEX,
                _ => EquipStat.Id.DEX
            };
        }
    }
}
