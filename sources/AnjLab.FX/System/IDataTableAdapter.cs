using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace AnjLab.FX.System
{
    public interface IDataTableAdapter<T>
    {
        DataTable Get(IList<T> list);
    }
}
