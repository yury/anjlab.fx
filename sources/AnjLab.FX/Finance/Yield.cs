using System;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Finance
{
    public class Yield
    {
        public static decimal Calculate(DateTime currentDate, DateTime futureDate, decimal pricePercent)
        {
            Guard.ArgumentNotNull("currentDate", currentDate);
            Guard.ArgumentNotNull("futureDate", futureDate);
            Guard.ArgumentGreaterThenZero("pricePercent", pricePercent);

            decimal time = TimeInterval.Get(currentDate, futureDate);
            return (time != 0) ? ((100*(100 - pricePercent))/(time*pricePercent)) : 0;
        }

        public static decimal CalculatePrice(decimal yield, DateTime currentDate, DateTime futureDate)
        {
            Guard.ArgumentNotNull("currentDate", currentDate);
            Guard.ArgumentNotNull("futureDate", futureDate);

            decimal time = TimeInterval.Get(currentDate, futureDate);
            return (time != 0) ? (100/(yield*time + 100))*100 : 0;
        }

        public static decimal CalculatePrice(decimal nominal, decimal amount)
        {
            return 100 * amount / nominal;
        }

        public static decimal CalculateAmount(decimal nominal, decimal price)
        {
            return nominal * price / 100;
        }
    }
}
