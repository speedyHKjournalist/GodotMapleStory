namespace MapleStory
{
    public struct Mapping
    {
        public KeyType.Id type;
        public KeyAction.Id action;
        public Mapping()
        {
            type = KeyType.Id.NONE;
            action = 0;
        }

        public Mapping(KeyType.Id type, KeyAction.Id action)
        {
            this.type = type;
            this.action = action;
        }

        public static bool operator ==(Mapping a, Mapping b) => a.type == b.type && a.action == b.action;
        public static bool operator !=(Mapping a, Mapping b) => !(a == b);

        public override bool Equals(object? obj) => obj is Mapping other && this == other;

        public override int GetHashCode() => (type, action).GetHashCode();
    }
}