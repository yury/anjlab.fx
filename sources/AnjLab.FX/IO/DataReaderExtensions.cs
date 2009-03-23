using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AnjLab.FX.IO
{
    public static class DataReaderExtensions
    {
        public static bool IsDBNull(this IDataReader reader, string name)
        {
            return reader.IsDBNull(reader.GetOrdinal(name));
        }

        public static DateTime? GetDateTime(this IDataReader reader, string name)
        {
            return reader.IsDBNull(name) || string.IsNullOrEmpty(Convert.ToString(reader[name]))
                       ? default(DateTime?)
                       : Convert.ToDateTime(reader[name]);
        }

        public static DateTime? GetDateTime(this IDataReader reader, string name, string format, IFormatProvider provider)
        {
            return reader.IsDBNull(name) || string.IsNullOrEmpty(Convert.ToString(reader[name]))
                       ? default(DateTime?)
                       : DateTime.ParseExact(reader.GetString(name), format, provider);
        }

        public static string GetString(this IDataReader reader, string name)
        {
            return reader.IsDBNull(name) ? null : Convert.ToString(reader[name]);
        }

        public static Decimal? GetDecimal(this IDataReader reader, string name)
        {
            return reader.IsDBNull(name) ? default(decimal?) : Convert.ToDecimal(reader[name]);
        }

        public static Int16? GetInt16(this IDataReader reader, string name)
        {
            return reader.IsDBNull(name) ? default(Int16?) : Convert.ToInt16(reader[name]);
        }

        public static Int32? GetInt32(this IDataReader reader, string name)
        {
            return reader.IsDBNull(name) ? default(Int32?) : Convert.ToInt32(reader[name]);
        }

        public static Int64? GetInt64(this IDataReader reader, string name)
        {
            return reader.IsDBNull(name) ? default(Int64?) : Convert.ToInt64(reader[name]);
        }

        public static Decimal? GetDecimal(this IDataReader reader, string name, string decimalSeparator, params string[] exceptValues)
        {
            if (reader.IsDBNull(name) || string.IsNullOrEmpty(reader.GetString(name))) return default(decimal?);
            foreach(var value in exceptValues)
            {
                if (value == reader.GetString(name)) return default(decimal?);
            }

            var provider = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
            provider.NumberDecimalSeparator = decimalSeparator;
            
            return decimal.Parse(reader.GetString(name), provider);
        }
    }
}
