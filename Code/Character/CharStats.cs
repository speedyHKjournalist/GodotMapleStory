using System.Collections.Generic;

namespace MapleStory
{
    public class CharStats
    {
        private string name = string.Empty;
        private List<int> petId = [];
        private Job? job;
        private long exp;
        private int mapId;
        private int portal;
        private (int, int) rank;
        private (int, int) jobRank;
        private EnumMap<MapleStat.Id, int> baseStats = new();
        private EnumMap<EquipStat.Id, int> totalStats = new();
        private EnumMap<EquipStat.Id, int> buffDeltas = new();
        private EnumMap<EquipStat.Id, float> percentages = new();
        private int maxDamage;
        private int minDamage;
        private int honor;
        private int attackSpeed;
        private int projectileRange;
        private Weapon.Type weaponType;
        private float mastery;
        private float critical;
        private float minCritical;
        private float maxCritical;
        private float damagePercent;
        private float bossDamage;
        private float ignoreDef;
        private float stance;
        private float resistStatus;
        private float reduceDamage;
        private bool female;

        public CharStats(StatsEntry state)
        {
            name = state.name;
            petId = state.petIds;
            exp = state.exp;
            mapId = state.mapId;
            portal = state.portal;
            rank = state.rank;
            jobRank = state.jobRank;
            baseStats = state.stats;
            female = state.female;
            job = new (baseStats[MapleStat.Id.JOB]);

            InitTotalStats();
        }
        public void InitTotalStats()
        {
            totalStats[EquipStat.Id.HP] = GetStat(MapleStat.Id.MAXHP);
            totalStats[EquipStat.Id.MP] = GetStat(MapleStat.Id.MAXMP);
            totalStats[EquipStat.Id.STR] = GetStat(MapleStat.Id.STR);
            totalStats[EquipStat.Id.DEX] = GetStat(MapleStat.Id.DEX);
            totalStats[EquipStat.Id.INT] = GetStat(MapleStat.Id.INT);
            totalStats[EquipStat.Id.LUK] = GetStat(MapleStat.Id.LUK);
            totalStats[EquipStat.Id.SPEED] = 100;
            totalStats[EquipStat.Id.JUMP] = 100;

            maxDamage = 0;
            minDamage = 0;
            honor = 0;
            attackSpeed = 0;
            projectileRange = 400;
            mastery = 0.0f;
            critical = 0.05f;
            minCritical = 0.5f;
            maxCritical = 0.75f;
            damagePercent = 0.0f;
            bossDamage = 0.0f;
            ignoreDef = 0.0f;
            stance = 0.0f;
            resistStatus = 0.0f;
            reduceDamage = 0.0f;
        }
        public void CloseTotalStats()
        {
            totalStats[EquipStat.Id.ACC] += CalculateAccuracy();

            foreach (var (Key, Value) in percentages.Entries)
            {
                EquipStat.Id stat = Key;
                int total = totalStats[stat];
                total += (int)(total * Value);
                SetTotal(stat, total);
            }

            int primary = GetPrimaryStat();
            int secondary = GetSecondaryStat();
            int attack = GetTotal(EquipStat.Id.WATK);
            float multiplier = damagePercent + (float)(attack) / 100;
            maxDamage = (int)((primary + secondary) * multiplier);
            minDamage = (int)(((primary * 0.9f * mastery) + secondary) * multiplier);
        }

        public int CalculateAccuracy()
        {

            int totaldex = GetTotal(EquipStat.Id.DEX);
            int totalluk = GetTotal(EquipStat.Id.LUK);

            return (int)(totaldex * 0.8f + totalluk * 0.5f);
        }

        public int GetPrimaryStat()
        {
            EquipStat.Id primary = job!.GetPrimary(weaponType);

            return (int)(GetMultiplier() * GetTotal(primary));
        }

        public int GetSecondaryStat()
        {
            EquipStat.Id secondary = job!.GetSecondary(weaponType);

            return GetTotal(secondary);
        }

        public float GetMultiplier()
        {
            switch (weaponType)
            {
                case Weapon.Type.SWORD_1H:
                    return 4.0f;

                case Weapon.Type.AXE_1H:
                case Weapon.Type.MACE_1H:
                case Weapon.Type.WAND:
                case Weapon.Type.STAFF:
                    return 4.4f;
                case Weapon.Type.DAGGER:
                case Weapon.Type.CROSSBOW:
                case Weapon.Type.CLAW:
                case Weapon.Type.GUN:
                    return 3.6f;
                case Weapon.Type.SWORD_2H:
                    return 4.6f;
                case Weapon.Type.AXE_2H:
                case Weapon.Type.MACE_2H:
                case Weapon.Type.KNUCKLE:
                    return 4.8f;
                case Weapon.Type.SPEAR:
                case Weapon.Type.POLEARM:
                    return 5.0f;
                case Weapon.Type.BOW:
                    return 3.4f;
                default:
                    return 0.0f;
            }
        }

        public void SetStat(MapleStat.Id stat, int value)
        {
            baseStats[stat] = value;
        }

        public void SetTotal(EquipStat.Id stat, int value)
        {
            if (EquipStatCaps.STATS.TryGetValue(stat, out int capValue))
            {
                if (value > capValue)
                    value = capValue;
            }

            totalStats[stat] = value;
        }

        public void AddBuff(EquipStat.Id stat, int value)
        {
            int current = GetTotal(stat);
            SetTotal(stat, current + value);
            buffDeltas[stat] += value;
        }

        public void AddValue(EquipStat.Id stat, int value)
        {
            int current = GetTotal(stat);
            SetTotal(stat, current + value);
        }

        public void AddPercent(EquipStat.Id stat, float percent)
        {
            percentages[stat] += percent;
        }

        public void SetWeaponType(Weapon.Type weaponType)
        {
            this.weaponType = weaponType;
        }

        public void SetExp(long exp)
        {
            this.exp = exp;
        }

        public void SetPortal(int portal)
        {
            this.portal = portal;
        }

        public void SetMastery(float mastery)
        {
            this.mastery = 0.5f + mastery;
        }

        public void SetDamagePercent(float damagePercent)
        {
            this.damagePercent = damagePercent;
        }

        public void SetReduceDamage(float reduceDamage)
        {
            this.reduceDamage = reduceDamage;
        }

        public void ChangeJob(int id)
        {
            baseStats[MapleStat.Id.JOB] = id;
            job.ChangeJob(id);
        }

        public int CalculateDamage(int mobAttack)
        {
            // TODO: Random stuff, need to find the actual formula somewhere.
            int weaponDef = GetTotal(EquipStat.Id.WDEF);

            if (weaponDef == 0)
                return mobAttack;

            int reduceAttack = mobAttack / 2 + mobAttack / weaponDef;

            return reduceAttack - (int)(reduceAttack * reduceDamage);
        }

        public bool IsDamageBuffed()
        {
            return GetBuffDelta(EquipStat.Id.WATK) > 0 || GetBuffDelta(EquipStat.Id.MAGIC) > 0;
        }

        public int GetStat(MapleStat.Id stat)
        {
            return baseStats[stat];
        }

        public int GetTotal(EquipStat.Id stat)
        {
            return totalStats[stat];
        }

        public int GetBuffDelta(EquipStat.Id stat)
        {
            return buffDeltas[stat];
        }

        public MapleRectangle<int> GetRange()
        {
            return new MapleRectangle<int>(-projectileRange, -5, -50, -50);
        }

        public void SetMapId(int mapId)
        {
            this.mapId = mapId;
        }

        public int GetMapId()
        {
            return mapId;
        }

        public int GetPortal()
        {
            return portal;
        }

        public long GetExp()
        {
            return exp;
        }

        public string GetName()
        {
            return name;
        }

        public string GetJobName()
        {
            return job.GetName();
        }

        public Weapon.Type GetWeaponType()
        {
            return weaponType;
        }

        public float GetMastery()
        {
            return mastery;
        }

        public float GetCritical()
        {
            return critical;
        }

        public float GetMinCritical()
        {
            return minCritical;
        }

        public float GetMaxCritical()
        {
            return maxCritical;
        }

        public float GetReduceDamage()
        {
            return reduceDamage;
        }

        public float GetBossDamage()
        {
            return bossDamage;
        }

        public float GetIgnoreDef()
        {
            return ignoreDef;
        }

        public void SetStance(float stance)
        {
            this.stance = stance;
        }

        public float GetStance()
        {
            return stance;
        }

        public float GetResistance()
        {
            return resistStatus;
        }

        public int GetMaxDamage()
        {
            return maxDamage;
        }

        public int GetMinDamage()
        {
            return minDamage;
        }

        public int GetHonor()
        {
            return honor;
        }

        public void SetAttackSpeed(int attackSpeed)
        {
            this.attackSpeed = attackSpeed;
        }

        public int GetAttackSpeed()
        {
            return attackSpeed;
        }

        public Job GetJob()
        {
            return job;
        }

        public bool GetFemale()
        {
            return female;
        }
    }
}