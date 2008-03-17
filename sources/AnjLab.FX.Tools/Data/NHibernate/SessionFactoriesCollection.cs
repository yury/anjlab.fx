using System.Configuration;

namespace AnjLab.FX.Tools.Data.NHibernate
{
    [ConfigurationCollection(typeof(SessionFactoryElement))]
    public sealed class SessionFactoriesCollection : ConfigurationElementCollection
    {
        public SessionFactoriesCollection() {
            SessionFactoryElement sessionFactory = (SessionFactoryElement)CreateNewElement();
            Add(sessionFactory);
        }

        public override ConfigurationElementCollectionType CollectionType {
            get {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement() {
            return new SessionFactoryElement();
        }

        protected override object GetElementKey(ConfigurationElement element) {
            return ((SessionFactoryElement)element).Name;
        }

        public SessionFactoryElement this[int index] {
            get {
                return (SessionFactoryElement)BaseGet(index);
            }
            set {
                if (BaseGet(index) != null) {
                    BaseRemoveAt(index);
                }

                BaseAdd(index, value);
            }
        }

        new public SessionFactoryElement this[string name] {
            get {
                return (SessionFactoryElement)BaseGet(name);
            }
        }

        public int IndexOf(SessionFactoryElement sessionFactory) {
            return BaseIndexOf(sessionFactory);
        }

        public void Add(SessionFactoryElement sessionFactory) {
            BaseAdd(sessionFactory);
        }

        protected override void BaseAdd(ConfigurationElement element) {
            BaseAdd(element, false);
        }

        public void Remove(SessionFactoryElement sessionFactory) {
            if (BaseIndexOf(sessionFactory) >= 0) {
                BaseRemove(sessionFactory.Name);
            }
        }

        public void RemoveAt(int index) {
            BaseRemoveAt(index);
        }

        public void Remove(string name) {
            BaseRemove(name);
        }

        public void Clear() {
            BaseClear();
        }
    }
}
