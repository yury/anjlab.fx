using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.IO
{
    public interface ILog
    {
        void Info(string message, params string[] args);
        void Warning(string message, params string[] args);
        void Error(string message, params string[] args);
        void Fatal(string message, params string[] args);
    }
}
