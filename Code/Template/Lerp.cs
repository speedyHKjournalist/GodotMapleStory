using System.Collections.Generic;

namespace MapleStory
{
    public static class MathUtils
    {
        // Linear interpolation function
        public static T Lerp<T>(T first, T second, float alpha) where T : struct
        {
            if (alpha <= 0.0f) return first;
            if (alpha >= 1.0f) return second;
            if (EqualityComparer<T>.Default.Equals(first, second)) return first;

            return (T)(dynamic)((1.0f - alpha) * (dynamic)first + alpha * (dynamic)second);
        }
    }
}