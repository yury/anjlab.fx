using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using System.Data;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Wpf.Controls
{
    public class ObjectContentPresenter : ContentPresenter
    {
        private bool _isBeingChanged;
        private static readonly Binding BindingToObject;

        static ObjectContentPresenter()
        {
            BindingToObject = new Binding
                                  {
                                      Mode = BindingMode.OneWay,
                                      Path = new PropertyPath(ObjectProperty),
                                      RelativeSource = new RelativeSource(RelativeSourceMode.Self)
                                  };
        }

        public ObjectContentPresenter()
        {
            // Bind the content to the Row property
            this.SetBinding(ContentProperty, BindingToObject);
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(object), typeof(ObjectContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(OnKeyChanged)));

        public object Key
        {
            get
            {
                return this.GetValue(KeyProperty);
            }
            set
            {
                this.SetValue(KeyProperty, value);
            }
        }

        public static readonly DependencyProperty ObjectSourceProperty = DependencyProperty.Register("ObjectSource", typeof(IEnumerable), typeof(ObjectContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(OnObjectSourceChanged)));

        private static void OnObjectSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var presenter = d as ObjectContentPresenter;
            presenter.TryToFindObject();
        }

        public IEnumerable ObjectSource
        {
            get
            {
                return this.GetValue(ObjectSourceProperty) as IEnumerable;
            }
            set
            {
                this.SetValue(ObjectSourceProperty, value);
            }
        }

        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register("Object", typeof(object), typeof(ObjectContentPresenter), new PropertyMetadata(null, new PropertyChangedCallback(OnObjectSourceChanged)));

        public object Object
        {
            get
            {
                return this.GetValue(ObjectProperty);
            }
            set
            {
                this.SetValue(ObjectProperty, value);
            }
        }

        public static readonly DependencyProperty KeyMemberPathProperty = DependencyProperty.Register("KeyMemberPath", typeof(string), typeof(ObjectContentPresenter));

        /// <summary>
        /// Gets or sets the path to key field in object from ObjectSource.
        /// </summary>
        /// <value>The key member path.</value>
        public string KeyMemberPath
        {
            get
            {
                return this.GetValue(KeyMemberPathProperty) as string;
            }
            set
            {
                this.SetValue(KeyMemberPathProperty, value);
            }
        }

        public static readonly DependencyProperty KeyPathProperty = DependencyProperty.Register("KeyPath", typeof(string), typeof(ObjectContentPresenter));

        /// <summary>
        /// Gets or sets the path to key field.
        /// </summary>
        /// <value>The key path.</value>
        public string KeyPath
        {
            get
            {
                return this.GetValue(KeyPathProperty) as string;
            }
            set
            {
                this.SetValue(KeyPathProperty, value);
            }
        }

        private void TryToFindObject()
        {
            if(ObjectSource == null || Key == null) return;

            if (!_isBeingChanged)
            {
                _isBeingChanged = true;

                try
                {
                    var key = Key;
                    if (!string.IsNullOrEmpty(KeyPath))
                        key = Reflector.GetValue(Key, KeyPath);

                    if (key != null)
                    {
                        foreach (var obj in ObjectSource)
                        {
                            if (key.Equals(Reflector.GetValue(obj, KeyMemberPath)))
                            {
                                Object = obj;
                                break;
                            }
                        }
                    }
                }
                finally
                {
                    _isBeingChanged = false;
                }
            }
        }

        private static void OnKeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var presenter = sender as ObjectContentPresenter;
            presenter.TryToFindObject();
        }

    }
}
