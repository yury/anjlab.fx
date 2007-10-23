using System;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    class SingletonActivator: Activator
    {
        readonly object _syncObject = new object();
        private object _value;

        public SingletonActivator(Definition definition, IObjectFactory factory) : base(definition, factory)
        {
        }

        public override Activator Create(Definition definition, IObjectFactory factory)
        {
            return new SingletonActivator(definition, factory);
        }

        public override object GetInstance(object [] args)
        {
            if (_value != null)
                return _value;

            lock(_syncObject)
            {
                if (_value != null)
                    return _value;
                object v = _objectFactory.New(args);
                _objectFactory.Init(_definition.PostInitData);
                return _value = v;
            }
        }


        public override object GetInstance()
        {
            return GetInstance(_definition.Args.ToArray());
        }
    }
}
