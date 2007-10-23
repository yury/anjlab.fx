using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnjLab.FX.Patterns.Generic.Inject
{
    public abstract class Activator
    {
        protected IObjectFactory _objectFactory;
        protected Definition _definition;

        protected Activator(Definition definition, IObjectFactory factory)
        {
            _definition = definition;
            _objectFactory = factory;
        }

        public static Activator PerCall
        {
            get { return new PerCallActivator(null, null); }
        }

        public static Activator Singleton
        {
            get { return new SingletonActivator(null, null); }
        }

        public abstract Activator Create(Definition definition, IObjectFactory factory);

        public abstract object GetInstance();
        public abstract object GetInstance(object[] args);
    }
}
