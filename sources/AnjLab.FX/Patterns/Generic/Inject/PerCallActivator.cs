using System;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    class PerCallActivator: Activator
    {
        public PerCallActivator(Definition definition, IObjectFactory factory) : base(definition, factory)
        {
        }

        public override Activator Create(Definition definition, IObjectFactory factory)
        {
            return new PerCallActivator(definition, factory);
        }

        public override object GetInstance()
        {
            return GetInstance(_definition.Args.ToArray());
        }


        public override object GetInstance(object[] args)
        {
            object o = _objectFactory.New(args);
            _objectFactory.Init(_definition.PostInitData);
            return o;
        }
    }
}
