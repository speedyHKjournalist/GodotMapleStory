using System.Collections.Generic;

namespace MapleStory
{
    public class Nominal<T> where T : struct
    {
        private T _now;
        private T _before;
        private float _threshold;

        public Nominal()
        {
            _now = default;
            _before = default;
            _threshold = 0.0f;
        }

        public T Get() => _now;

        public T Get(float alpha) => alpha >= _threshold ? _now : _before;

        public T Last() => _before;

        public void Set(T value)
        {
            _now = value;
            _before = value;
        }

        public void Normalize() => _before = _now;

        public bool Normalized() => _before.Equals(_now);

        public void Next(T value, float threshold)
        {
            _before = _now;
            _now = value;
            _threshold = threshold;
        }

        public Nominal<T> Clone()
        {
            Nominal<T> copy = new();

            copy._now = _now;
            copy._before = _before;
            copy._threshold = _threshold;

            return copy;
        }

        public static bool operator ==(Nominal<T> nominal, T value) => nominal._now.Equals(value);

        public static bool operator !=(Nominal<T> nominal, T value) => !nominal._now.Equals(value);

        public static bool operator ==(T value, Nominal<T> nominal) => nominal._now.Equals(value);

        public static bool operator !=(T value, Nominal<T> nominal) => !nominal._now.Equals(value);

        public static T operator +(Nominal<T> nominal, T value)
        {
            return (dynamic)nominal._now + value;
        }

        public static T operator -(Nominal<T> nominal, T value)
        {
            return (dynamic)nominal._now - value;
        }

        public static T operator *(Nominal<T> nominal, T value)
        {
            return (dynamic)nominal._now * value;
        }

        public static T operator /(Nominal<T> nominal, T value)
        {
            return (dynamic)nominal._now / value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is Nominal<T> nominal)
            {
                return (dynamic)nominal._now == _now
                && (dynamic)nominal._before == _before
                && (dynamic)nominal._threshold == _threshold;
            }

            return false;
        }

        public override int GetHashCode() => _now.GetHashCode();
    }

    public class Linear<T> where T : struct
    {
        private T _now;
        private T _before;

        public T Get() => _now;

        public T Get(float alpha)
        {
            return MathUtils.Lerp(_before, _now, alpha);
        }

        public T Last() => _before;

        public void SetAllValue(T value)
        {
            _now = value;
            _before = value;
        }

        public void UpdateValue(T value)
        {
            _before = _now;
            _now = value;
        }

        public void Add(T value)
        {
            _before = _now;
            _now = (dynamic)_now + value;
        }

        public void Subtract(T value)
        {
            _before = _now;
            _now = (dynamic)_now - value;
        }

        public void Normalize() => _before = _now;

        public bool Normalized() => _before.Equals(_now);

        public Linear<T> Clone()
        {
            Linear<T> copy = new();

            copy._now = _now;
            copy._before = _before;
            return copy;
        }

        public static bool operator ==(Linear<T> linear, T value)
        {
            return EqualityComparer<T>.Default.Equals(linear._now, value);
        }

        public static bool operator !=(Linear<T> linear, T value)
        {
            return !(linear == value);
        }

        public static bool operator <(Linear<T> linear, T value)
        {
            return (dynamic)linear._now < value;
        }

        public static bool operator <=(Linear<T> linear, T value)
        {
            return (dynamic)linear._now <= value;
        }

        public static bool operator >(Linear<T> linear, T value)
        {
            return (dynamic)linear._now > value;
        }

        public static bool operator >=(Linear<T> linear, T value)
        {
            return (dynamic)linear._now >= value;
        }

        public static T operator +(Linear<T> linear, T value)
        {
            return (dynamic)linear._now + value;
        }

        public static T operator -(Linear<T> linear, T value)
        {
            return (dynamic)linear._now - value;
        }

        public static T operator *(Linear<T> linear, T value)
        {
            return (dynamic)linear._now * value;
        }

        public static T operator /(Linear<T> linear, T value)
        {
            return (dynamic)linear._now / value;
        }

        public static T operator +(Linear<T> linear, Linear<T> value)
        {
            return (dynamic)linear._now + value.Get();
        }

        public static T operator -(Linear<T> linear, Linear<T> value)
        {
            return (dynamic)linear._now - value.Get();
        }

        public static T operator *(Linear<T> linear, Linear<T> value)
        {
            return (dynamic)linear._now * value.Get();
        }

        public static T operator /(Linear<T> linear, Linear<T> value)
        {
            return (dynamic)linear._now / value.Get();
        }

        public override bool Equals(object? obj)
        {
            if (obj is Linear<T> linear)
            {
                return (dynamic)linear._now == _now
                && (dynamic)linear._before == _before;
            }

            return false;
        }

        public override int GetHashCode() => _now.GetHashCode();
    }
}
