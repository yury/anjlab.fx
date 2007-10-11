
AnjLab.Finance = {
    calcYield: function(price, earlyDate, laterDate){
        return (100*(100 - price))/(AnjLab.Finance.getTimeInterval(earlyDate, laterDate)*price);
    },
    
    getTimeInterval: function(currentDate, futureDate){
        var currentYearFirstDay = DateTime.firstYearDay(currentDate);
        var futureYearLastDay = DateTime.lastYearDay(futureDate);
        
        var daysInYearsInterval = DateTime.daysBetween(currentYearFirstDay, futureYearLastDay) + 1;
        if (daysInYearsInterval % 365 == 0) // no leap year in interval
        {
            return DateTime.daysBetween(currentDate, futureDate) / 365;
        }
        else
        {
            if (currentDate.getFullYear() == futureDate.getFullYear()) // leap year
                return DateTime.daysBetween(currentDate, futureDate) / 366;
            else // interval contains leap years
            {
                var firstInterval = DateTime.daysBetween(currentDate, DateTime.lastYearDay(currentDate)) / DateTime.daysInYear(currentDate);
                var secondInterval = (DateTime.daysBetween(DateTime.firstYearDay(futureDate), futureDate) + 1) / DateTime.daysInYear(futureDate);
                return firstInterval + secondInterval + (futureDate.getFullYear() - currentDate.getFullYear() - 1);
            }
        }
    },
    
    calcPercent:function(value, maxValue){
        return 100 * value / maxValue;
    },
    
    calcPriceFromYield: function(yield, ddate, rdate){
        return (100/(yield*AnjLab.Finance.getTimeInterval(ddate, rdate) + 100))*100;
    },
    
    calcValue: function(maxValue, percent){
        return maxValue * percent / 100;
    }
};