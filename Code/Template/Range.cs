using System;

namespace MapleStory
{
    public struct Range<T> : IEquatable<Range<T>> where T : IComparable<T>
    {
        private T _a;
        private T _b;

        public Range(T first, T second)
        {
            _a = first;
            _b = second;
        }

        public T First => _a;

        public T Second => _b;

        public T Greater => _a.CompareTo(_b) > 0 ? _a : _b;

        public T Smaller => _a.CompareTo(_b) < 0 ? _a : _b;

        public T Delta => (dynamic)_b - (dynamic)_a;

        public T Length => (dynamic)Greater - (dynamic)Smaller;

        public T Center => (dynamic)_a + (dynamic)_b / (dynamic)2;

        public bool Empty => _a.CompareTo(_b) == 0;

        public bool Contains(T v)
        {
            if (_a.CompareTo(_b) > 0)
            {
                return v.CompareTo(_b) >= 0 && v.CompareTo(_a) <= 0;
            }
            return v.CompareTo(_a) >= 0 && v.CompareTo(_b) <= 0;
        }


        public bool Contains(Range<T> v)
        {
            T thisSmaller = Smaller;
            T thisGreater = Greater;
            T vSmaller = v.Smaller;
            T vGreater = v.Greater;

            return vSmaller.CompareTo(thisSmaller) >= 0 && vGreater.CompareTo(thisGreater) <= 0;
        }

        public bool Overlaps(Range<T> v)
        {
            T thisSmaller = Smaller;
            T thisGreater = Greater;
            T vSmaller = v.Smaller;
            T vGreater = v.Greater;

            return !(thisGreater.CompareTo(vSmaller) < 0 || vGreater.CompareTo(thisSmaller) < 0);
        }

        public static bool operator ==(Range<T> left, Range<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Range<T> left, Range<T> right)
        {
            return !(left == right);
        }

        public static Range<T> operator +(Range<T> left, Range<T> right)
        {
            return new Range<T>((dynamic)left._a + (dynamic)right._a, (dynamic)left._b + (dynamic)right._b);
        }

        public static Range<T> operator -(Range<T> left, Range<T> right)
        {
            return new Range<T>((dynamic)left._a - (dynamic)right._a, (dynamic)left._b - (dynamic)right._b);
        }

        public static Range<T> operator -(Range<T> range)
        {
            return new Range<T>((dynamic)range._a * (dynamic)(-1), (dynamic)range._b * (dynamic)(-1));
        }

        public static Range<T> Symmetric(T mid, T tail)
        {
            return new Range<T>((dynamic)mid - (dynamic)tail, (dynamic)mid + (dynamic)tail);
        }

        public bool Equals(Range<T> other)
        {
            return _a.CompareTo(other._a) == 0 && _b.CompareTo(other._b) == 0;
        }

        public override bool Equals(object? obj)
        {
            return obj is Range<T> range && Equals(range);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_a, _b);
        }
    }
}