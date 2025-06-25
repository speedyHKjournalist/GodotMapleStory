namespace MapleStory
{
    public class TimedBool
    {
        private long _last;
        private long _delay;
        private bool _value;

        public bool Value => _value;

        public TimedBool()
        {
            _value = false;
            _delay = 0;
            _last = 0;
        }

        public void SetFor(long millis)
        {
            _last = millis;
            _delay = millis;
            _value = true;
        }

        public void Update(uint timestep)
        {
            if (_value)
            {
                if (timestep >= _delay)
                {
                    _value = false;
                    _delay = 0;
                }
                else
                {
                    _delay -= timestep;
                }
            }
        }

        public void Reset(bool b)
        {
            _value = b;
            _delay = 0;
            _last = 0;
        }

        public float Alpha()
        {
            if (_last == 0) return _value ? 1.0f : 0.0f;
            if (_value == false) return 1.0f;

            return 1.0f - (float)_delay / _last;
        }

        public static bool operator ==(TimedBool tb, bool b)
        {
            if (ReferenceEquals(tb, null))
            {
                return !b;
            }
            return tb._value == b;
        }

        public static bool operator ==(bool b, TimedBool tb)
        {
            return tb == b;
        }

        public static bool operator !=(TimedBool tb, bool b)
        {
            return !(tb == b);
        }

        public static bool operator !=(bool b, TimedBool tb)
        {
            return !(b == tb);
        }

        public override bool Equals(object? obj)
        {
            if (obj is bool b)
            {
                return _value == b;
            }
            if (obj is TimedBool otherTb)
            {
                return _value == otherTb._value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}