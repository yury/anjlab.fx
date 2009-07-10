//
// subset of prototype.js functions
//
Object.extend = function(destination, source) {
  for (var property in source) {
    destination[property] = source[property];
  }
  return destination;
}

Function.prototype.bind = function() {
  var __method = this, args = $A(arguments), object = args.shift();
  return function() {
    return __method.apply(object, args.concat($A(arguments)));
  }
}

Function.prototype.bindAsEventListener = function(object) {
  var __method = this, args = $A(arguments), object = args.shift();
  return function(event) {
    return __method.apply(object, [( event || window.event)].concat(args).concat($A(arguments)));
  }
}

var $A = Array.from = function(iterable) {
  if (!iterable) return [];
  if (iterable.toArray) {
    return iterable.toArray();
  } else {
    var results = [];
    for (var i = 0, length = iterable.length; i < length; i++)
      results.push(iterable[i]);
    return results;
  }
}

//
// our methods
//
AnjLab = {}

DateTime = {
    parse:function(dateStr){
        try{
            var parts = dateStr.split('.');
            return new Date(parts[2], parts[1] - 1, parts[0]);
        }
        catch(e){
            return null;
        }
    },
    
    firstYearDay : function(date){
        return new Date(date.getFullYear(), 0, 1);
    },
    
    lastYearDay:function(date){
        return new Date(date.getFullYear(), 11, 31);
    },
    
    daysBetween:function(earlyDate, laterDate){
        return DateTime._millisecsInDays(laterDate - earlyDate);
    },
    
    daysInYear : function(date){
        return DateTime.isLeapYear(date) ? 366 : 365;
    },
    
    isLeapYear:function(year){
        if (year.getFullYear) year = year.getFullYear();
        return (year % 4 == 0 && year % 400 != 0);
    },
    
    equals:function(date1, date2){
        return date1.toDateString() == date2.toDateString();
    },
    
    // private
    _millisecsInDays:function(msecs){
        return Math.round(msecs/1000/60/60/24);
    }
};

Math.roundTo = function(value, toSign)
{
    if (!value || value == 0)
        return value;
       
    var negative = (value < 0);
    var k = Math.pow(10, toSign);
    var round = Math.round(value * k);
    if (round == 0) round = round + (negative)?-1:1;
    return round / k;
}
