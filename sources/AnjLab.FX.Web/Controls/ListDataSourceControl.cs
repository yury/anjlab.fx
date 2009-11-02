using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing.Design;
using System.Web.UI;
using System.Web.UI.WebControls;
using AnjLab.FX.Sys;
using System.Collections;

namespace AnjLab.FX.Web.Controls
{
    [PersistChildren(false),
    ParseChildren(true)]
    public class ListDataSourceControl : DataTableSourceContol
    {
        private readonly DataTableAdapterConfig _dataTableConfig = new DataTableAdapterConfig();
        private DataTable _dataTable;
        
        public void SetDataSource<T>(IList<T> list)
        {
            IDataTableAdapter<T> adapter = DataTableAdapterFactory.Singleton.New<T>(DataTableConfig.PropertyColumns);
            _dataTable = adapter.Get(list);

            this.RaiseDataSourceChangedEvent(global::System.EventArgs.Empty);
        }

        internal override DataTable GetDataTable()
        {
            return _dataTable;
        }

        [
        Category("Behavior"),
        Description("Extra configuration about how to generate a DataTable from the results"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
        TypeConverter(typeof(ExpandableObjectConverter)),
        DefaultValue((string)null)
        ]
        public DataTableAdapterConfig DataTableConfig
        {
            get { return _dataTableConfig; }
        }
    }

    [PersistChildren(false),
     ParseChildren(true),
     TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataTableAdapterConfig
    {
        private Collection<PropertyColumnElement> _columns = new Collection<PropertyColumnElement>();
        [
        Description("List of columns"),
        PersistenceMode(PersistenceMode.InnerProperty),
        DefaultValue((string[])null)
        ]
        [NotifyParentProperty(true)]
        public Collection<PropertyColumnElement> Columns
        {
            get { return _columns; }
        }

        public IList<PropertyColumn> PropertyColumns
        {
            get
            {
                List<PropertyColumn> columns = new List<PropertyColumn>();
                foreach (PropertyColumnElement columnElement in _columns)
                {
                    columns.Add(columnElement);
                }

                return columns;
            }
        }
    }

    [PersistChildren(false),
     ParseChildren(true),
     TypeConverter(typeof(ExpandableObjectConverter))]
    public class PropertyColumnElement : PropertyColumn
    {
        
    }

}
