using System.Collections.Generic;
using System.Data;

namespace AnjLab.FX.Sys
{
    public interface IDataTableAdapter<T>
    {
        DataTable Get(IList<T> list);
    }
}
