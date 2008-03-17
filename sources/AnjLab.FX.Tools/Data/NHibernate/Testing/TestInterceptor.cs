using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NHibernate.Type;

namespace AnjLab.FX.Tools.Data.NHibernate.Testing
{
    [Serializable]
    public class TestInterceptor : EmptyInterceptor
    {
        private ArrayList _savedObjects = new ArrayList();
        
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            _savedObjects.Add(entity);
            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public ArrayList SavedObjects
        {
            get { return _savedObjects; }
        }
    }
}
