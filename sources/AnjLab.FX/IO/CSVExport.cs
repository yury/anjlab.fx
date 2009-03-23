using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;

namespace AnjLab.FX.IO
{
    public class CSVExport
    {
        private static readonly string ColumnSeparator = ";";

        public static void Export(DataTable table, string fileName, Encoding encoding)
        {
            using(FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                Export(table, fs, encoding);
            }
        }

        public static void Export(DataTable table, Stream stream, Encoding encoding)
        {
            using(StreamWriter sw = new StreamWriter(stream, encoding))
            {
                //write headers
                for(int i=0; i<table.Columns.Count; i++)
                {
                    sw.Write(table.Columns[i]);
                    
                    if(i<table.Columns.Count - 1)
                        sw.Write(ColumnSeparator);
                }

                sw.Write(sw.NewLine);

                //write rows
                foreach(DataRow row in table.Rows)
                {
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        if (!Convert.IsDBNull(row[i]) && row[i] != null)
                        {
                            sw.Write(row[i].ToString());
                        }

                        if (i < table.Columns.Count - 1)
                            sw.Write(ColumnSeparator);
                    }

                    sw.Write(sw.NewLine);
                }
            }
        }
    }
}
