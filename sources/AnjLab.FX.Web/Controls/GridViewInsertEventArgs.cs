using System.Collections.Specialized;
using System.ComponentModel;

namespace AnjLab.FX.Web.Controls
{
    public class GridViewInsertEventArgs : CancelEventArgs
    {
        private readonly int _rowIndex;
        private IOrderedDictionary _values;

        public GridViewInsertEventArgs(int rowIndex)
            : base(false)
        {
            _rowIndex = rowIndex;
        }

        /// <summary>
        /// Gets a dictionary containing the revised values of the non-key field name/value
        /// pairs in the row to update.
        /// </summary>
        public IOrderedDictionary NewValues
        {
            get
            {
                if (_values == null)
                {
                    _values = new OrderedDictionary();
                }
                return _values;
            }
        }

        /// <summary>
        /// Gets the index of the row being updated.
        /// </summary>
        public int RowIndex { get { return _rowIndex; } }
    }
}