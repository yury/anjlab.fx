using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AnjLab.FX.IO
{
    public class TraceLog: ILog
    {
        public void Info(string message, params string[] args)
        {
            Trace.WriteLine(string.Format(message, args), "Info");
        }

        public void Warning(string message, params string[] args)
        {
            Trace.WriteLine(string.Format(message, args), "Warning");
        }

        public void Error(string message, params string[] args)
        {
            Trace.WriteLine(string.Format(message, args), "Error");
        }

        public void Fatal(string message, params string[] args)
        {
            Trace.WriteLine(string.Format(message, args), "Fatal");
        }
    }
}
