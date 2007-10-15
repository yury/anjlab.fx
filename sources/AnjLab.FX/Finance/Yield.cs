using System;
using AnjLab.FX.System;

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
    }
}
