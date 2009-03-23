using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Data
{
    public enum RepositoryFilterType
    {
        None      = 0,
        ByMinute  = 1,
        ByHour    = 2,
        ByDay     = 3,
        ByWeek    = 4,
        ByMonth   = 5,
        ByQuarter = 6,
        ByYear    = 7
    }
}
