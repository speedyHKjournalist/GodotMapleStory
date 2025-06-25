using System;
using System.Collections.Generic;

namespace MapleStory
{
    public struct Account
    {
        public int accountId;
        public int female;
        public bool admin;
        public string name;
        public bool muted;
        public bool pin;
        public int pic;
    };

    public struct World
    {
        public string name;
        public string eventMessage;
        public List<int> channelCapacities;
        public int channelCount;
        public int flag;
        public int id;
    };

    public struct StatsEntry
    {
        public string name;
        public bool female;
        public List<int> petIds;
        public EnumMap<MapleStat.Id, int> stats;
        public long exp;
        public int mapId;
        public int portal;
        public ValueTuple<int, int> rank;
        public ValueTuple<int, int> jobRank;
    };

    public struct LookEntry
    {
        public bool female;
        public int skin;
        public int faceId;
        public int hairId;
        public Dictionary<int, int> equips;
        public Dictionary<int, int> maskedEquipments;
        public List<int> petIds;
    };

    public struct CharEntry
    {
        public StatsEntry stats;
        public LookEntry look;
        public int id;
    };
}