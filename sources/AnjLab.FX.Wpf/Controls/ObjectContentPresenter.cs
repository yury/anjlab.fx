using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using System.Data;

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

        private static void OnKeyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // When the Key changes, try to find the data row that has the new key.
            // If it is not found, return null.
            var presenter = sender as ObjectContentPresenter;

            if (presenter.ObjectSource == null)
                throw new InvalidOperationException("There is no ObjectSource associated with this ObjectSourcePresneter. The Initialize method has, most likely, not been called.");

            if (!presenter._isBeingChanged)
            {
                presenter._isBeingChanged = true;

                try
                {
                    foreach (var obj in presenter.ObjectSource)
                    {
                        //if (presenter.Key.Equals(fx.Reflector.GetValue(obj, presenter.KeyMemberPath)))
                        //{
                        //    presenter.Object = obj;
                        //    break;
                        //}
                    }
                }
                finally
                {
                    presenter._isBeingChanged = false;
                }
            }
        }

    }
}
