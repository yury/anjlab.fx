using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using AnjLab.FX.System;

namespace AnjLab.FX.Web.Controls
{
    [
    DefaultEvent("SelectedIndexChanged"),
    Designer("System.Web.UI.Design.WebControls.GridViewDesigner, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
    ControlValueProperty("SelectedValue"),
    ]
    public class BulkEditGridView : GridView
    {
        //key for the RowInserting event handler list
        public static readonly object RowInsertingEvent = new object();

        private readonly List<int> _dirtyRows = new List<int>();
        private readonly List<int> _newRows = new List<int>();

        private TableItemStyle insertRowStyle;

        /// <summary>
        /// Modifies the creation of the row to set all rows as editable.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="dataSourceIndex"></param>
        /// <param name="rowType"></param>
        /// <param name="rowState"></param>
        /// <returns></returns>
        protected override GridViewRow CreateRow(int rowIndex, int dataSourceIndex, DataControlRowType rowType, DataControlRowState rowState)
        {
            return base.CreateRow(rowIndex, dataSourceIndex, rowType, rowState | DataControlRowState.Edit);
        }

        public List<GridViewRow> DirtyRows
        {
            get
            {
                List<GridViewRow> drs = new List<GridViewRow>();
                foreach (int rowIndex in _dirtyRows)
                {
                    drs.Add(Rows[rowIndex]);
                }

                return drs;
            }
        }

        /// <summary>
        /// Adds event handlers to controls in all the editable cells.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="fields"></param>
        protected override void InitializeRow(GridViewRow row, DataControlField[] fields)
        {
            base.InitializeRow(row, fields);
            foreach (TableCell cell in row.Cells)
            {
                if (cell.Controls.Count > 0)
                {
                    AddChangedHandlers(cell.Controls);
                }
            }
        }

        /// <summary>
        /// Adds an event handler to editable controls.
        /// </summary>
        /// <param name="controls"></param>
        private void AddChangedHandlers(ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                if (ctrl is TextBox)
                {
                    ((TextBox)ctrl).TextChanged += this.HandleRowChanged;
                }
                else if (ctrl is CheckBox)
                {
                    ((CheckBox)ctrl).CheckedChanged += this.HandleRowChanged;
                }
                else if (ctrl is DropDownList)
                {
                    ((DropDownList)ctrl).SelectedIndexChanged += this.HandleRowChanged;
                }
                else if (ctrl is HtmlInputText)
                {
                    //Added for BUG#69
                    ((HtmlInputText)ctrl).ServerChange += new EventHandler(this.HandleRowChanged);
                }
                ////could add recursion if we are missing some controls.
                //else if (ctrl.Controls.Count > 0 && !(ctrl is INamingContainer) )
                //{
                //    AddChangedHandlers(ctrl.Controls);
                //}
            }
        }


        /// <summary>
        /// This gets called when a row is changed.  Store the id of the row and wait to update
        /// until save is called.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void HandleRowChanged(object sender, EventArgs args)
        {
            GridViewRow row = ((Control)sender).NamingContainer as GridViewRow;
            if (null != row)
            {
                if (0 != (row.RowState & DataControlRowState.Insert))
                {
                    int altRowIndex = InnerTable.Rows.GetRowIndex(row);
                    if (false == _newRows.Contains(altRowIndex))
                        _newRows.Add(altRowIndex);
                }
                else
                {
                    if (false == _dirtyRows.Contains(row.RowIndex))
                        _dirtyRows.Add(row.RowIndex);
                }
            }

        }

        /// <summary>
        /// Setting this property will cause the grid to update all modified records when 
        /// this button is clicked.  It currently supports Button, ImageButton, and LinkButton.
        /// If you set this property, you do not need to call save programatically.
        /// </summary>
        [IDReferenceProperty(typeof(Control))]
        public string SaveButtonID
        {
            get
            {
                return (string)(ViewState["SaveButtonID"] ?? String.Empty);
            }
            set
            {
                ViewState["SaveButtonID"] = value;
            }
        }

        /// <summary>
        /// Attaches an eventhandler to the onclick method of the save button.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Attach an event handler to the save button.
            if (false == string.IsNullOrEmpty(SaveButtonID))
            {
                Control btn = RecursiveFindControl(NamingContainer, SaveButtonID);
                if (null != btn)
                {
                    if (btn is Button)
                    {
                        ((Button)btn).Click += SaveClicked;
                    }
                    else if (btn is LinkButton)
                    {
                        ((LinkButton)btn).Click += SaveClicked;
                    }
                    else if (btn is ImageButton)
                    {
                        ((ImageButton)btn).Click += SaveClicked;
                    }
                }
            }
        }

        /// <summary>
        /// Looks for a control recursively up the control tree.  We need this because Page.FindControl
        /// does not find the control if we are inside a masterpage content section.
        /// </summary>
        /// <param name="namingcontainer"></param>
        /// <param name="controlName"></param>
        /// <returns></returns>
        private static Control RecursiveFindControl(Control namingcontainer, string controlName)
        {
            Control c = namingcontainer.FindControl(controlName);

            if (c != null)
                return c;

            if (namingcontainer.NamingContainer != null)
                return RecursiveFindControl(namingcontainer.NamingContainer, controlName);

            return null;
        }

        /// <summary>
        /// Handles the save event, and calls the save method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveClicked(object sender, EventArgs e)
        {
            Save();
            DataBind();
        }

        /// <summary>
        /// Saves any modified rows.  This is called automatically if the SaveButtonId is set.
        /// </summary>
        public void Save()
        {
            try
            {
                foreach (int row in _dirtyRows)
                {
                    //TODO: need to check if we really want false here.  Probably want to pull this
                    //fron the save button.
                    UpdateRow(row, false);
                }

                foreach (int row in _newRows)
                {
                    //Make the datasource save a new row.
                    InsertRow(row, false);
                }
            }
            finally
            {
                _dirtyRows.Clear();
                _newRows.Clear();
            }
        }

        /// <summary>
        /// Prepares the <see cref="RowInserting"/> event and calls insert on the DataSource.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="causesValidation"></param>
        private void InsertRow(int rowIndex, bool causesValidation)
        {
            GridViewRow row = null;

            if ((!causesValidation || (Page == null)) || Page.IsValid)
            {
                DataSourceView dsv = null;
                bool useDataSource = IsBoundUsingDataSourceID;
                if (useDataSource)
                {
                    dsv = GetData();
                    if (dsv == null)
                    {
                        throw new HttpException("DataSource Returned Null View");
                    }
                }
                GridViewInsertEventArgs args = new GridViewInsertEventArgs(rowIndex);
                if (useDataSource)
                {
                    if ((row == null) && (InnerTable.Rows.Count > rowIndex))
                    {
                        row = InnerTable.Rows[rowIndex] as GridViewRow;
                    }
                    if (row != null)
                    {
                        ExtractRowValues(args.NewValues, row, true, true);
                    }
                }

                OnRowInserting(args);

                if (!args.Cancel && useDataSource)
                {
                    dsv.Insert(args.NewValues, DataSourceViewInsertCallback);
                }
            }
        }

        /// <summary>
        /// Callback for the datasource's insert command.
        /// </summary>
        /// <param name="i"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static bool DataSourceViewInsertCallback(int i, Exception ex)
        {
            if (null != ex)
            {
                throw ex;
            }

            return true;
        }


        /// <summary>
        /// Fires the <see cref="RowInserting"/> event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnRowInserting(GridViewInsertEventArgs args)
        {
            Delegate handler = Events[RowInsertingEvent];
            if (null != handler)
                handler.DynamicInvoke(this, args);
        }

        /// <summary>
        /// Event fires when new row has been edited, and save is clicked.
        /// </summary>
        public event GridViewInsertEventHandler RowInserting
        {
            add
            {
                Events.AddHandler(RowInsertingEvent, value);
            }
            remove
            {
                Events.RemoveHandler(RowInsertingEvent, value);
            }
        }

        /// <summary>
        /// Access to the GridView's inner table.
        /// </summary>
        protected Table InnerTable
        {
            get
            {
                if (false == HasControls())
                    return null;

                return (Table)Controls[0];
            }
        }

        /// <summary>
        /// Enables inline inserting.  Off by default.
        /// </summary>
        [Category("Extended")]
        public bool EnableInsert
        {
            get
            {
                return (bool)(ViewState["EnableInsert"] ?? false);
            }
            set
            {
                ViewState["EnableInsert"] = value;
            }
        }


        [Category("Extended")]
        public int InsertRowCount
        {
            get
            {
                return (int)(ViewState["InsertRowCount"] ?? 1);
            }
            set
            {
                Guard.ArgumentGreaterThan("value", value, 1);

                ViewState["InsertRowCount"] = value;
            }
        }


        /// <summary>
        /// We have to recreate our insert row so we can load the postback info from it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnPagePreLoad(object sender, EventArgs e)
        {
            base.OnPagePreLoad(sender, e);

            if (EnableInsert && Page.IsPostBack)
            {
                for (int i = 0; i < InsertRowCount; i++)
                    CreateInsertRow();
            }
        }

        /// <summary>
        /// After the controls are databound, add a row to the end.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDataBound(EventArgs e)
        {
            if (EnableInsert)
            {
                for (int i = 0; i < InsertRowCount; i++)
                    CreateInsertRow();
            }

            base.OnDataBound(e);
        }

        /// <summary>
        /// Creates the insert row and adds it to the inner table.
        /// </summary>
        protected virtual void CreateInsertRow()
        {
            GridViewRow row = CreateRow(Rows.Count, -1, DataControlRowType.DataRow, DataControlRowState.Insert);

            DataControlField[] fields = new DataControlField[Columns.Count];
            Columns.CopyTo(fields, 0);

            row.ApplyStyle(insertRowStyle);

            InitializeRow(row, fields);

            //Creates header row for empty data.
            if (Rows.Count == 0)
            {
                Controls.Add(new Table());
                GridViewRow header = CreateRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);
                InitializeRow(header, fields);
                //this.CreateChildTable();
                InnerTable.Rows.Add(header);

            }

            int index = InnerTable.Rows.Count - (ShowFooter ? 1 : 0);
            InnerTable.Rows.AddAt(index, row);
        }


        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Category("Styles"),
        PersistenceMode(PersistenceMode.InnerProperty),
        NotifyParentProperty(true),
        Description("GridView_InsertRowStyle")
        ]
        public TableItemStyle InsertRowStyle
        {
            get
            {
                if (insertRowStyle == null)
                {
                    insertRowStyle = new TableItemStyle();
                    if (IsTrackingViewState)
                    {
                        ((IStateManager)insertRowStyle).TrackViewState();
                    }
                }
                return insertRowStyle;
            }
        }
    }
}
