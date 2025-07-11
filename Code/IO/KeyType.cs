namespace MapleStory
{
    public class KeyType
    {
        // Keytypes determine how a keycode is interpreted.
        public enum Id : byte
        {
            NONE = 0,
            SKILL = 1,
            ITEM = 2,
            CASH = 3,
            MENU = 4,
            ACTION = 5,
            FACE = 6,
            MACRO = 8,
            TEXT = 9,
            LENGTH
        }

        public static Id TypeById(int id)
        {
            if (id <= (int)Id.NONE || id >= (int)Id.LENGTH)
            {
                return Id.NONE;
            }

            return (Id)id;
        }
    }
}
