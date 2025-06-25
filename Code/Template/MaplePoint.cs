using System;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public struct MaplePoint<T> : IEquatable<MaplePoint<T>> where T : struct, IComparable<T>
    {
        private readonly T _x;
        private readonly T _y;

        // Return the x-coordinate
        public T X => _x;

        // Return the y-coordinate
        public T Y => _y;

        // Construct a point with coordinates (0, 0)
        public MaplePoint()
        {
            _x = ConvertValue(0);
            _y = ConvertValue(0);
        }

        // Construct a point from a vector property
        public MaplePoint(Wz_Node src)
        {
            Wz_Vector vector = src?.GetValue<Wz_Vector>() ?? new Wz_Vector(0, 0);
            _x = ConvertValue(vector.X);
            _y = ConvertValue(vector.Y);
        }

        public MaplePoint(Godot.Vector2 position)
        {
            // Convert from float for Vector2 components
            _x = (T)Convert.ChangeType(position.X, typeof(T));
            _y = (T)Convert.ChangeType(position.Y, typeof(T));
        }

        // Construct a point from the specified coordinates
        public MaplePoint(T x, T y)
        {
            _x = x;
            _y = y;
        }

        public MaplePoint<T> Clone()
        {
            return new MaplePoint<T>(_x, _y); // More explicit clone for clarity
        }

        private static T ConvertValue(int value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        // Return the inner product (Length / Magnitude)
        public T Length()
        {
            dynamic x = _x, y = _y;
            double result = Math.Sqrt((double)x * (double)x + (double)y * (double)y);

            if (typeof(T) == typeof(int))
                return (T)(object)(int)result;
            if (typeof(T) == typeof(float))
                return (T)(object)(float)result;
            if (typeof(T) == typeof(double))
                return (T)(object)result;

            throw new InvalidOperationException($"Unsupported type {typeof(T)} for Length()");
        }

        public bool Straight()
        {
            return X.Equals(Y);
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }

        public T Distance(MaplePoint<T> v)
        {
            return (this - v).Length();
        }

        public MaplePoint<T> SetX(T val) => new MaplePoint<T>(val, Y);
        public MaplePoint<T> SetY(T val) => new MaplePoint<T>(X, val);
        public MaplePoint<T> ShiftX(T val) => new MaplePoint<T>(Add(X, val), Y);
        public MaplePoint<T> ShiftY(T val) => new MaplePoint<T>(X, Add(Y, val));
        public MaplePoint<T> Shift(T x, T y) => new MaplePoint<T>(Add(X, x), Add(Y, y));
        public MaplePoint<T> Shift(MaplePoint<T> v) => new MaplePoint<T>(Add(X, v.X), Add(Y, v.Y));

        public MaplePoint<T> Abs()
        {
            return new MaplePoint<T>(Math.Abs((dynamic)X), Math.Abs((dynamic)Y));
        }

        public Godot.Vector2 ToVector2()
        {
            return new Godot.Vector2(Convert.ToSingle(X), Convert.ToSingle(Y));
        }

        // Operators
        public static bool operator ==(MaplePoint<T> p1, MaplePoint<T> p2) => p1.Equals(p2);
        public static bool operator !=(MaplePoint<T> p1, MaplePoint<T> p2) => !p1.Equals(p2);
        public static MaplePoint<T> operator +(MaplePoint<T> p, MaplePoint<T> v) => new MaplePoint<T>(Add(p.X, v.X), Add(p.Y, v.Y));
        public static MaplePoint<T> operator -(MaplePoint<T> p, MaplePoint<T> v) => new MaplePoint<T>(Subtract(p.X, v.X), Subtract(p.Y, v.Y));
        public static MaplePoint<T> operator *(MaplePoint<T> p, T v) => new MaplePoint<T>(Multiply(p.X, v), Multiply(p.Y, v));
        public static MaplePoint<T> operator /(MaplePoint<T> p, T v) => new MaplePoint<T>(Divide(p.X, v), Divide(p.Y, v));
        public static MaplePoint<T> operator -(MaplePoint<T> p) => new MaplePoint<T>(Negate(p.X), Negate(p.Y));

        private static T Add(T a, T b) => (dynamic)a + b;
        private static T Subtract(T a, T b) => (dynamic)a - b;
        private static T Multiply(T a, T b) => (dynamic)a * b;
        private static T Divide(T a, T b) => (dynamic)a / b;
        private static T Negate(T a) => -(dynamic)a;

        public bool Equals(MaplePoint<T> other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override bool Equals(object? obj)
        {
            return obj is MaplePoint<T> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}