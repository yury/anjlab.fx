using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;

namespace AnjLab.FX.Web
{
    public abstract class DataTableSourceContol : DataSourceControl
    {
        protected const string DefaultViewName = "DefaultView";
        internal DataTableSourceView _view;

        public DataTableSourceContol()
        {
            _view = new DataTableSourceView(this, DefaultViewName);
        }

        protected override DataSourceView GetView(string viewName)
        {
            return this._view;
        }

        protected override ICollection GetViewNames()
        {
            return new string[] { DefaultViewName };
        }

        internal abstract DataTable GetDataTable();

        [Category("Data"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue((string)null),
        Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false)]
        public ParameterCollection SelectParameters
        {
            get { return this._view.SelectParameters; }
        }
    }

    internal class DataTableSourceView : DataSourceView
    {
        private DataTableSourceContol _owner;
        private DataView _dataView;
        private ParameterCollection _selectParameters;

        public DataTableSourceView(DataTableSourceContol owner, string viewName)
            : base(owner, viewName)
        {
            this._owner = owner;
        }

        public override bool CanSort
        {
            get { return true; }
        }
        
        public ParameterCollection SelectParameters
        {
            get
            {
                if (this._selectParameters == null)
                {
                    this._selectParameters = new ParameterCollection();
                }
                return this._selectParameters;
            }
        }

        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            arguments.RaiseUnsupportedCapabilitiesError(this);

            DataTable dt = _owner.GetDataTable();
            if(dt == null) return new DataTable().DefaultView;

            _dataView = new DataView(dt);

            PerformFiltering();

            PerformSorting(arguments.SortExpression);

            return _dataView;
        }

        private void PerformFiltering()
        {
            foreach (ControlParameter parameter in SelectParameters)
            {
                Control control = ControlsSearcher.FindControl(_owner.Page, parameter.ControlID);
                if (control != null)
                {
                    string name = parameter.Name;
                    object value = control.GetType().GetProperty(parameter.PropertyName).GetValue(control, null);
                    if(value == null) continue;

                    if (string.IsNullOrEmpty(_dataView.RowFilter))
                        _dataView.RowFilter = "1=1";
                    if (value.ToString() != string.Empty)
                        _dataView.RowFilter += string.Format(" AND {0} = '{1}'", name, value);
                    else
                        _dataView.RowFilter += string.Format(" AND ({0} IS NULL OR {0} = '{1}')", name, value);
                }
            }
        }

        private void PerformSorting(string sortExpression)
        {
            _dataView.Sort = sortExpression;
        }
    }
}
