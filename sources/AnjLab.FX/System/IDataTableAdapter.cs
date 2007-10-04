using System.Collections.Generic;
using System.Data;

namespace AnjLab.FX.System
{
    public interface IDataTableAdapter<T>
    {
        DataTable Get(IList<T> list);
    }
}
