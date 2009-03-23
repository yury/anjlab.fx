using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AnjLab.FX.IO
{
    /// <summary>
    /// DataReader which can read comma-separated files
    /// </summary>
    public class CSVReader : IDataReader
    {
        private readonly StreamReader _innerStreamReader;
        private char _delimiter = ';';
        private readonly List<CSVReaderColumnDefinition> _headerColumns = new List<CSVReaderColumnDefinition>();
        private readonly Dictionary<string, int> _ordinals = new Dictionary<string, int>();
        private string[] _innerData;
        private bool _isFirstLineHeader = true;

        public CSVReader(string fileName):this(fileName, Encoding.Default)
        {
        }

        public CSVReader(string fileName, Encoding encoding) 
        {
            _innerStreamReader = new StreamReader(fileName, encoding);

            ReadHeader();
        }

        public CSVReader(Stream stream): this(stream, Encoding.Default)
        {
        }

        public CSVReader(Stream stream, Encoding encoding)
        {
            _innerStreamReader = new StreamReader(stream, encoding);

            ReadHeader();
        }

        /// <summary>
        /// Gets or sets the delimiter of file. By default it is ';'
        /// </summary>
        /// <value>The delimiter.</value>
        public char Delimiter
        {
            get { return _delimiter; }
            set { _delimiter = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the first line is header.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is first line header; otherwise, <c>false</c>.
        /// </value>
        public bool IsFirstLineHeader
        {
            get { return _isFirstLineHeader; }
            set { _isFirstLineHeader = value; }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            if(_innerStreamReader != null)
                _innerStreamReader.Dispose();
        }

        #endregion

        private void ReadHeader()
        {
            foreach(var columnName in _innerStreamReader.ReadLine().Split(_delimiter))
                AddColumnDefinition(new CSVReaderColumnDefinition {Name = columnName, Type = typeof(string)});
        }

        private void AddColumnDefinition(CSVReaderColumnDefinition definition)
        {
            _headerColumns.Add(definition);
            _ordinals.Add(definition.Name, _headerColumns.Count - 1);
        }

        public void AddComputedColumnDefinition(CSVReaderColumnDefinition definition, Func<CSVReader, object> computeFunc)
        {
            definition.ComputeFunc = computeFunc;
            AddColumnDefinition(definition);
        }

        public CSVReaderColumnDefinition GetColumnDefinition(int i)
        {
            return _headerColumns[i];
        }

        #region Implementation of IDataRecord

        public string GetName(int i)
        {
            return _headerColumns[i].Name;
        }

        public string GetDataTypeName(int i)
        {
            return GetFieldType(i).Name;
        }

        public Type GetFieldType(int i)
        {
            return _headerColumns[i].Type;
        }

        public object GetValue(string name)
        {
            return GetValue(GetOrdinal(name));
        }

        public object GetValue(int i)
        {
            if (_headerColumns[i].IsComputed)
                return _headerColumns[i].ComputeFunc(this);

            if (string.IsNullOrEmpty(_innerData[i]))
                return _headerColumns[i].NullValue;

            if (GetFieldType(i) == typeof(decimal))
                return string.IsNullOrEmpty(_headerColumns[i].FormatString)
                           ? decimal.Parse(_innerData[i])
                           : decimal.Parse(_innerData[i], GetDecimalFormatProvider(_headerColumns[i].FormatString));

            if (GetFieldType(i) == typeof(int))
                return string.IsNullOrEmpty(_headerColumns[i].FormatString)
                           ? int.Parse(_innerData[i])
                           : int.Parse(_innerData[i], GetIntegerFormatProvider(_headerColumns[i].FormatString));

            if (GetFieldType(i) == typeof(DateTime))
                return string.IsNullOrEmpty(_headerColumns[i].FormatString)
                           ? DateTime.Parse(_innerData[i])
                           : DateTime.ParseExact(_innerData[i], _headerColumns[i].FormatString,
                                                 CultureInfo.InvariantCulture);

            return _innerData[i];
        }

        private static IFormatProvider GetDecimalFormatProvider(string formatString)
        {
            var provider = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
            provider.NumberDecimalSeparator = formatString;
            return provider;
        }

        private static IFormatProvider GetIntegerFormatProvider(string formatString)
        {
            var provider = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
            provider.NumberGroupSeparator = formatString;
            return provider;
        }

        public int GetValues(object[] values)
        {
            for (int i = 0; i < values.Length; i++)
                values[i] = GetValue(i);

            return values.Length;
        }

        public int GetOrdinal(string name)
        {
            return _ordinals[name];
        }

        public bool GetBoolean(int i)
        {
            return Convert.ToBoolean(GetValue(i));
        }

        public byte GetByte(int i)
        {
            return Convert.ToByte(GetValue(i));
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new System.NotImplementedException();
        }

        public char GetChar(int i)
        {
            return Convert.ToChar(GetValue(i));
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new System.NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            return new Guid(Convert.ToString(GetValue(i)));
        }

        public short GetInt16(int i)
        {
            return Convert.ToInt16(GetValue(i));
        }

        public int GetInt32(int i)
        {
            return Convert.ToInt32(GetValue(i));
        }

        public long GetInt64(int i)
        {
            return Convert.ToInt64(GetValue(i));
        }

        public float GetFloat(int i)
        {
            return Convert.ToSingle(GetValue(i)); ;
        }

        public double GetDouble(int i)
        {
            return Convert.ToDouble(GetValue(i));
        }

        public string GetString(string name)
        {
            return GetString(GetOrdinal(name));
        }

        public string GetString(int i)
        {
            return Convert.ToString(GetValue(i));
        }

        public decimal GetDecimal(string name)
        {
            return GetDecimal(GetOrdinal(name));
        }

        public decimal GetDecimal(int i)
        {
            return Convert.ToDecimal(GetValue(i));
        }

        public DateTime GetDateTime(string name)
        {
            return GetDateTime(GetOrdinal(name));
        }

        public DateTime GetDateTime(int i)
        {
            return Convert.ToDateTime(GetValue(i));
        }

        public IDataReader GetData(int i)
        {
            throw new System.NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return false;
        }

        public int FieldCount
        {
            get { return _headerColumns.Count; }
        }

        object IDataRecord.this[int i]
        {
            get { return GetValue(i); }
        }

        object IDataRecord.this[string name]
        {
            get { return GetValue(GetOrdinal(name)); }
        }

        #endregion

        #region Implementation of IDataReader

        public void Close()
        {
            
        }

        public DataTable GetSchemaTable()
        {
            return null;
        }

        public bool NextResult()
        {
            return _innerStreamReader.ReadLine() != null; 
        }

        public bool Read()
        {
            var line = _innerStreamReader.ReadLine();
            if (line == null) return false;

            _innerData = line.Split(_delimiter);
            return true;
        }

        public int Depth
        {
            get { return 0; }
        }

        public bool IsClosed
        {
            get { return _innerStreamReader == null; }
        }

        public int RecordsAffected
        {
            get { return -1; }
        }

        #endregion
    }

    public class CSVReaderColumnDefinition
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public string FormatString { get; set; }
        public object NullValue { get; set; }

        public bool IsComputed { get { return ComputeFunc != null; } }

        internal Func<CSVReader, object> ComputeFunc { get; set; }
    }
}
