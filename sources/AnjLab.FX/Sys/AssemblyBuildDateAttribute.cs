using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace AnjLab.FX.Sys
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyBuildDateAttribute: Attribute
    {
        private readonly DateTime _dateTime;

        public DateTime DateTime
        {
            get { return _dateTime; }
        }

        public AssemblyBuildDateAttribute()
        {
            
        }

        public AssemblyBuildDateAttribute(DateTime date)
        {
            _dateTime = date;            
        }

        public AssemblyBuildDateAttribute(string date)
        {
            DateTime.TryParse(date, CultureInfo.InvariantCulture, DateTimeStyles.None, out _dateTime);
        }

        public static DateTime ? GetEntryAssemblyBuildDate()
        {
            foreach (AssemblyBuildDateAttribute attribute in Assembly.GetEntryAssembly().GetCustomAttributes(typeof (AssemblyBuildDateAttribute), true))
            {
                return attribute.DateTime;  
            }
            return null;
        }
    }
}
