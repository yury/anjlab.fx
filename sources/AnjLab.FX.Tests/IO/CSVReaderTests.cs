using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using AnjLab.FX.IO;
using System.IO;

namespace AnjLab.FX.Tests.IO
{
    [TestFixture]
    public class CSVReaderTests
    {
        [Test]
        public void TestCSVReader()
        {
            using (var reader = new CSVReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("AnjLab.FX.Tests.IO.CSVReaderData.txt")))
            {
                reader.GetColumnDefinition(1).Type = typeof (decimal);
                reader.GetColumnDefinition(1).FormatString = ".";
                reader.GetColumnDefinition(1).NullValue = 10m;

                reader.GetColumnDefinition(2).Type = typeof(int);
                reader.GetColumnDefinition(2).NullValue = 5;

                reader.GetColumnDefinition(3).Type = typeof(DateTime);
                reader.GetColumnDefinition(3).FormatString = "dd.MM.yyyy";

                reader.GetColumnDefinition(4).Type = typeof(DateTime);
                reader.GetColumnDefinition(4).FormatString = "HH:mm:ss";

                reader.AddComputedColumnDefinition(new CSVReaderColumnDefinition { Name = "Полное время", Type=typeof(DateTime)},
                    r => r.GetDateTime(3).Add(r.GetDateTime(4).TimeOfDay));
    
                Assert.AreEqual("Column 1", reader.GetColumnDefinition(0).Name);
                Assert.AreEqual(false, reader.GetColumnDefinition(0).IsComputed);
                Assert.AreEqual(typeof(string), reader.GetColumnDefinition(0).Type);

                Assert.AreEqual("Column 2", reader.GetColumnDefinition(1).Name);
                Assert.AreEqual(false, reader.GetColumnDefinition(1).IsComputed);
                Assert.AreEqual(typeof(decimal), reader.GetColumnDefinition(1).Type);

                Assert.AreEqual("Column 3", reader.GetColumnDefinition(2).Name);
                Assert.AreEqual(false, reader.GetColumnDefinition(2).IsComputed);
                Assert.AreEqual(typeof(int), reader.GetColumnDefinition(2).Type);

                var table = new DataTable();
                table.Load(reader);

                Assert.AreEqual(typeof(decimal), table.Columns["Column 2"].DataType);
                Assert.AreEqual(typeof(int), table.Columns["Column 3"].DataType);
                Assert.AreEqual(typeof(DateTime), table.Columns["День"].DataType);
                Assert.AreEqual(typeof(DateTime), table.Columns["Время"].DataType);
                Assert.AreEqual(typeof(DateTime), table.Columns["Полное время"].DataType);

                Assert.AreEqual(144.34m, table.Rows[0]["Column 2"]);
                Assert.AreEqual(10m, table.Rows[1]["Column 2"]);
                Assert.AreEqual(5, table.Rows[1]["Column 3"]);
                Assert.AreEqual(123, table.Rows[0]["Column 3"]);
                Assert.AreEqual(new DateTime(2008, 3, 2), table.Rows[0]["День"]);
                Assert.AreEqual(new DateTime(2008, 3, 2, 14, 55, 35), table.Rows[0]["Полное время"]);
            }
        }
    }
}
