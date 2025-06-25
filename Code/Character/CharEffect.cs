namespace MapleStory
{
    namespace CharEffect
    {
        public enum Id
        {
            LEVELUP = 0,
            JOBCHANGE,
            SCROLL_SUCCESS,
            SCROLL_FAILURE,
            MONSTER_CARD
        }

        public static class Paths
        {
            public static readonly EnumMap<Id, string> PATHS;

            // Static constructor to initialize the static readonly field
            static Paths()
            {
                PATHS = new EnumMap<Id, string>();
                PATHS.Emplace(Id.LEVELUP, "LevelUp");
                PATHS.Emplace(Id.JOBCHANGE, "JobChanged");
                PATHS.Emplace(Id.SCROLL_SUCCESS, "Enchant\\Success");
                PATHS.Emplace(Id.SCROLL_FAILURE, "Enchant\\Failure");
                PATHS.Emplace(Id.MONSTER_CARD, "MonsterBook\\cardGet");
            }
        }
    }
}