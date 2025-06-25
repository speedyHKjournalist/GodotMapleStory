namespace MapleStory
{
    public class BoolPair<T>
    {
        public T? first { get; private set; }
        public T? second { get; private set; }

        public BoolPair(T? first, T? second)
        {
            this.first = first;
            this.second = second;
        }

        public BoolPair() { }

        public void Set(bool b, T? value)
        {
            if (b)
                first = value;
            else
                second = value;
        }

        public T? this[bool b]
        {
            get => b ? first : second;
            set
            {
                if (b)
                    first = value;
                else
                    second = value;
            }
        }
    }
}