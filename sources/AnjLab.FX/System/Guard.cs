using System;
using AnjLab.FX.Properties;

namespace AnjLab.FX.System
{
    public static class Guard
    {
        #region ArgumentGreaterThenZero overloads

        public static void ArgumentGreaterThenZero(string argument, TimeSpan value)
        {
            if (value.Ticks <= 0)
                throw new ArgumentOutOfRangeException(argument, value, Resources.ShouldBeGreaterThenZero);
        }
        
        public static void ArgumentGreaterThenZero(string argument, decimal value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(argument, value, Resources.ShouldBeGreaterThenZero);
        }

        public static void ArgumentGreaterThenZero(string argument, int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(argument, value, Resources.ShouldBeGreaterThenZero);
        }

        public static void ArgumentGreaterThenZero(string argument, float value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(argument, value, Resources.ShouldBeGreaterThenZero);
        }

        public static void ArgumentGreaterThenZero(string argument, double value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(argument, value, Resources.ShouldBeGreaterThenZero);
        }

        #endregion

        public static void ArgumentBetweenInclusive(string argument, int value, int a, int b)
        {
            if (value < a || b < value)
                throw new ArgumentOutOfRangeException( argument
                                                     , value
                                                     , string.Format(Resources.ShouldBeBetween_A_B
                                                                    , a
                                                                    , b));
        }

        public static void ArgumentNotNull(string argument, object value)
        {
            if (value == null)
                throw new ArgumentNullException(argument, Resources.CantBeNull);
        }

        public static void ArgumentNotNullOrEmpty(string argument, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(Resources.CantBeNullOrEmpty, argument);
        }
    }
}
