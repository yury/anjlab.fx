using System.Collections.Generic;
using System.ComponentModel;

namespace AnjLab.FX.Sys
{
    public class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private KeyValuePair<string,  ListSortDirection> _sortPair = new KeyValuePair<string, ListSortDirection>();
        
        public SortableBindingList()
        {
            
        }

        public SortableBindingList(IEnumerable<T> list)
        {
            foreach(T element in list)
                Add(element);
        }
        
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            // Get list to sort
            var items = Items as List<T>;
            
            if(_sortPair.Key == property.Name)
            {
                direction = _sortPair.Value == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
            }

            _sortPair = new KeyValuePair<string, ListSortDirection>(property.Name, direction);
                
            if (items != null)
            {
                var pc = new PropertyComparer<T>(property, direction);
                items.Sort(pc);
                _isSorted = true;
            }
            else
            {
                _isSorted = false;
            }

            //this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
        }
    }
}