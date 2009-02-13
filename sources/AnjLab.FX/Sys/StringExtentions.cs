using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Sys
{
    public static class StringExtentions
    {
        public static void StartProcess(this string cmd)
        {
            var p = new Process {StartInfo = new ProcessStartInfo(cmd)};
            p.Start();
        }

        public static bool IsNullOrEmtpy(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}
