using System;

namespace AnjLab.FX.System
{
    public static class Guard
    {
        public static void ArgumentGreaterThenZero(string argument, TimeSpan value)
        {
            if (value.Ticks <= 0)
                throw new ArgumentOutOfRangeException(argument, value, "Should be greater then zero");
        }

        public static void ArgumentBetweenInclusive(string argument, int value, int a, int b)
        {
            if (value < a || b < value)
                throw new ArgumentOutOfRangeException( argument
                                                     , value
                                                     , string.Format("Should be between [{0},{1}]"
                                                                    , a
                                                                    , b));
        }

        public static void ArgumentNotNull(string argument, object value)
        {
            if (value == null)
                throw new ArgumentNullException(argument, "Can't be null");
        }

        public static void ArgumentNotNullOrEmpty(string argument, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Can't be null or empty", argument);
        }
    }
}
