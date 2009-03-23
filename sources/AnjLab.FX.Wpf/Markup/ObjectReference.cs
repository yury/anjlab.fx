using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;

namespace AnjLab.FX.Wpf.Markup
{
        public class ObjectReference : MarkupExtension
        {
            public ObjectReference()
            {
            }

            public ObjectReference(object key)
            {
                _ref = key;
            }

            public ObjectReference(object key, bool isDeclaration)
            {
                _ref = key;
                _isDeclaration = isDeclaration;
            }

            public override object ProvideValue(IServiceProvider serviceProvider)
            {
                object result = _ref;

                if (_ref == null)
                {
                    throw new InvalidOperationException("The Ref has not been specified for the ObjectReference.");
                }

                // determine whether this is a declaration or a reference
                bool isDeclaration = false;
                if (serviceProvider != null)
                {
                    IProvideValueTarget provideValueTarget = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
                    if (provideValueTarget != null)
                    {
                        isDeclaration = _isDeclaration || (provideValueTarget.TargetProperty == ObjectReference.DeclarationProperty);
                        if (isDeclaration)
                            References[_ref] = new WeakReference(provideValueTarget.TargetObject);
                    }
                }

                if (!isDeclaration)
                {
                    WeakReference targetReference;
                    if (References.TryGetValue(_ref, out targetReference))
                        result = targetReference.Target;
                }

                return result;
            }

            #region Declaration Attached Property

            public static readonly DependencyProperty DeclarationProperty =
                DependencyProperty.RegisterAttached("Declaration", typeof(object), typeof(ObjectReference),
                    new FrameworkPropertyMetadata(null));

            public static object GetDeclaration(DependencyObject d)
            {
                return d.GetValue(DeclarationProperty);
            }

            public static void SetDeclaration(DependencyObject d, object value)
            {
                d.SetValue(DeclarationProperty, value);
            }

            #endregion

            [ThreadStatic]
            private static Dictionary<object, WeakReference> _references = null;

            private static Dictionary<object, WeakReference> References
            {
                get
                {
                    if (_references == null)
                    {
                        _references = new Dictionary<object, WeakReference>();
                    }
                    return _references;
                }
            }

            private object _ref = null;
            public object Ref
            {
                get { return _ref; }
                set { _ref = value; }
            }

            private bool _isDeclaration = false;
            public bool IsDeclaration
            {
                get { return _isDeclaration; }
                set { _isDeclaration = value; }
            }
        }
}
