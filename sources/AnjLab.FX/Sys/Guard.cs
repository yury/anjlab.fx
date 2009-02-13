using System;
using System.Threading;
using AnjLab.FX.Properties;

namespace AnjLab.FX.Sys
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
                throw new ArgumentOutOfRangeException(argument
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

        public static void NotNull(object value, string msg, params object[] args)
        {
            if (value == null)
                throw new NullReferenceException(String.Format(msg, args));
        }

        public static void NotNull(object value)
        {
            NotNull(value, Resources.CantBeNull);
        }

        public static void GreaterThan(int value, int than)
        {
            GreaterThan(value, than, Resources.ShouldBeGreater_A_B, value, than);
        }

        public static void GreaterThan(int value, int then, string msg, params object[] args)
        {
            if (value <= then)
                throw new ArgumentOutOfRangeException(String.Format(msg, args));
        }

        public static void ArgumentNotNullNorEmpty(string argument, string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException(Resources.CantBeNullOrEmpty, argument);
        }

        public static void NotNullNorEmpty(string value, string message, params object[] args)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentOutOfRangeException(String.Format(message, args));
        }

        public static void IsTrue(bool value)
        {
            IsTrue(value, Resources.CantBeFalse);
        }

        public static void IsTrue(bool value, string msg, params object[] args)
        {
            if (!value)
                throw new InvalidOperationException(String.Format(msg, args));
        }

        public static void IsNull(object value, string message)
        {
            if (value != null)
                throw new InvalidOperationException(message);
        }
    }
}
