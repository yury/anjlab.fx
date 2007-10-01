using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.UI;

namespace AnjLab.FX.Web
{
    public abstract class DataTableSourceContol : DataSourceControl
    {
        internal abstract DataTable GetDataTable();
    }

    internal class DataTableSourceView : DataSourceView
    {
        private DataTableSourceContol _owner;
        private DataView _dataView;

        public DataTableSourceView(DataTableSourceContol owner, string viewName)
            : base(owner, viewName)
        {
            this._owner = owner;
        }

        public override bool CanSort
        {
            get { return true; }
        }

        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            arguments.RaiseUnsupportedCapabilitiesError(this);

            DataTable dt = _owner.GetDataTable();
            if(dt == null) return new DataTable().DefaultView;

            _dataView = dt.DefaultView;

            PerformSorting(arguments.SortExpression);

            return _dataView;
        }

        private void PerformSorting(string sortExpression)
        {
            _dataView.Sort = sortExpression;
        }
    }
}
