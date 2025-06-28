using System.Collections.Generic;

namespace MapleStory
{
    // Externalizing this for easier configurability
    public class EquipStatCaps
    {
        public static readonly Dictionary<EquipStat.Id, int> STATS = new()
        {
            { EquipStat.Id.STR, 999 },
            { EquipStat.Id.DEX, 999 },
            { EquipStat.Id.INT, 999 },
            { EquipStat.Id.LUK, 999 },
            { EquipStat.Id.HP, 30000 },
            { EquipStat.Id.MP, 30000 },
            { EquipStat.Id.WATK, 999 },
            { EquipStat.Id.MAGIC, 2000 },
            { EquipStat.Id.WDEF, 999 },
            { EquipStat.Id.MDEF, 999 },
            { EquipStat.Id.ACC, 999 },
            { EquipStat.Id.AVOID, 999 },
            { EquipStat.Id.HANDS, 999 },
            { EquipStat.Id.SPEED, 140 },
            { EquipStat.Id.JUMP, 123 }
        };
    }
}