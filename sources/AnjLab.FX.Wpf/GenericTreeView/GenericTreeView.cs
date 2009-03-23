using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using AnjLab.FX.Wpf.GenericTreeView;

namespace AnjLab.FX.Wpf.GenericTreeView
{
    /// <summary>
    /// The generic tree view control. Based on TreeViewBase with object parameter. Can use several datasources.
    /// </summary>
    public class GenericTreeView : TreeViewBase<object>
    {
        readonly Dictionary<object, ObservableCollection<object>> _childRelations = new Dictionary<object, ObservableCollection<object>>();
        readonly Dictionary<object, object> _parentRelations = new Dictionary<object, object>();
        readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();
        readonly Dictionary<object, string> _keys = new Dictionary<object, string>();

        readonly ObservableCollection<object> _bindingList = new ObservableCollection<object>();

        public void ClearSources()
        {
            Items = null;
            _childRelations.Clear();
            _parentRelations.Clear();
            _dictionary.Clear();
            _keys.Clear();
            _bindingList.Clear();
        }

        public void AddSource<T>(IEnumerable<T> source, Func<T, string> getKeyFunc, Func<T, string> getParentKeyFunc)
        {
            if(source == null) return;

            foreach (T element in source)
            {
                AddElement(element, getKeyFunc(element), getParentKeyFunc(element));
            }

            INotifyCollectionChanged ncc = source as INotifyCollectionChanged;
            if(ncc != null)
            {
                ncc.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
                                             {
                                                 if (e.Action == NotifyCollectionChangedAction.Remove)
                                                 {
                                                     foreach (T item in e.OldItems)
                                                     {
                                                         DeleteElement(item, getKeyFunc(item), getParentKeyFunc(item));
                                                     }
                                                 } else if(e.Action == NotifyCollectionChangedAction.Add)
                                                 {
                                                     foreach (T item in e.NewItems)
                                                     {
                                                         AddElement(item, getKeyFunc(item), getParentKeyFunc(item));
                                                     }
                                                 }
                                             };
            }
        }

        public void AddElement(object element, string key, string parentKey)
        {
            if(_dictionary.ContainsKey(key))
                return;

            _dictionary.Add(key, element);
            _keys.Add(element, key);
            _childRelations.Add(element, new ObservableCollection<object>());

            if (!string.IsNullOrEmpty(parentKey))
            {
                if (_dictionary.ContainsKey(parentKey))
                {
                    object parent = _dictionary[parentKey];

                    _parentRelations.Add(element, parent);
                    _childRelations[parent].Insert(0, element);
                }
            }
            else
            {
                _bindingList.Add(element);
            }
        }

        public void DeleteElement(object element, string key, string parentKey)
        {
            if(!string.IsNullOrEmpty(parentKey))
            {
                if (_dictionary.ContainsKey(parentKey))
                {
                    object parent = _dictionary[parentKey];
                    if (_childRelations.ContainsKey(parent))
                        _childRelations[parent].Remove(element);

                    if (_parentRelations.ContainsKey(element))
                        _parentRelations.Remove(element);
                }
            }

            if (_dictionary.ContainsKey(key))
            {
                _dictionary.Remove(key);
                _keys.Remove(element);
                _childRelations.Remove(element);
                _bindingList.Remove(element);
            }
        }

        public void Bind()
        {
            Items = _bindingList;
        }

        public override string GetItemKey(object item)
        {
            if (_keys.ContainsKey(item))
                return _keys[item];

            return item.GetHashCode().ToString();
            //else
            //    throw new ArgumentNullException("item", "UI TreeView contains item that is already deleted from inner dictionaries.");
        }

        public object SelectedValue
        {
            get { return SelectedItem; }
            set { SelectedItem = value;}
        }

        public override ICollection<object> GetChildItems(object parent)
        {
            if (_childRelations.ContainsKey(parent))
                return _childRelations[parent];
            else
                return new ObservableCollection<object>();
        }

        public override object GetParentItem(object item)
        {
            if(_parentRelations.ContainsKey(item))
                return _parentRelations[item];
            else
                return null;
        }

    }
}
