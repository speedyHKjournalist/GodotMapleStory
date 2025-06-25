using System;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public class MapleRectangle<T> where T : struct, IComparable<T>
    {
        private MaplePoint<T> leftTop;
        private MaplePoint<T> rightBottom;

        public MapleRectangle(Wz_Node source)
        {
            leftTop = new MaplePoint<T>(source.FindNodeByPath("lt"));
            rightBottom = new MaplePoint<T>(source.FindNodeByPath("rb"));
        }

        public MapleRectangle(Wz_Node sourceLeftTop, Wz_Node sourceRightBottom)
        {
            leftTop = new MaplePoint<T>(sourceLeftTop);
            rightBottom = new MaplePoint<T>(sourceRightBottom);
        }
        public MapleRectangle(MaplePoint<T> leftTop, MaplePoint<T> rightBottom)
        {
            this.leftTop = leftTop;
            this.rightBottom = rightBottom;
        }

        public MapleRectangle(T left, T right, T top, T bottom)
        {
            leftTop = new MaplePoint<T>(left, top);
            rightBottom = new MaplePoint<T>(right, bottom);
        }

        public MapleRectangle()
        {
            leftTop = new MaplePoint<T>();
            rightBottom = new MaplePoint<T>();
        }

        public T Width()
        {
            dynamic l = Left();
            dynamic r = Right();
            return Math.Abs(l - r);
        }

        public T Height()
        {
            dynamic t = Top();
            dynamic b = Bottom();
            return Math.Abs(t - b);
        }

        public T Left() => leftTop.X;
        public T Top() => leftTop.Y;
        public T Right() => rightBottom.X;
        public T Bottom() => rightBottom.Y;

        public bool Contains(MaplePoint<T> v)
        {
            return !Straight() &&
                   ((dynamic)v.X >= (dynamic)Left()) &&
                   ((dynamic)v.X <= (dynamic)Right()) &&
                   ((dynamic)v.Y >= (dynamic)Top()) &&
                   ((dynamic)v.Y <= (dynamic)Bottom());
        }

        public bool Overlaps(MapleRectangle<T> other)
        {
            return GetHorizontal().Overlaps(new Range<T>(other.Left(), other.Right())) &&
                   GetVertical().Overlaps(new Range<T>(other.Top(), other.Bottom()));
        }

        public bool Straight() => leftTop.Equals(rightBottom);

        public bool Empty() => leftTop.Straight() && rightBottom.Straight() && Straight();

        public MaplePoint<T> GetLeftTop() => leftTop.Clone();
        public MaplePoint<T> GetRightBottom() => rightBottom.Clone();

        public Range<T> GetHorizontal() => new Range<T>(Left(), Right());
        public Range<T> GetVertical() => new Range<T>(Top(), Bottom());

        public void Shift(MaplePoint<T> v)
        {
            leftTop += v;
            rightBottom += v;
        }
    }
}