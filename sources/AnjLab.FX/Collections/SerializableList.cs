using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using AnjLab.FX.Sys;

#if NET_3_5
namespace AnjLab.FX.Collections
{
    [Serializable]

    public class SerializableList<T> : List<T>, INotifyCollectionChanged, IXmlSerializable
    {
        List<TypeAlias> _aliases = new List<TypeAlias>();
        private const string XmlSchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";
        private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void WriteXml(XmlWriter writer)
        {
            List<T> serializable = GetSerializableItems();
            GenerateAliases(serializable);

            writer.WriteStartElement("Aliases");
            WriteObject(writer, _aliases, false);
            writer.WriteEndElement();

            writer.WriteStartElement("Items");
            writer.WriteAttributeString("count", "", serializable.Count.ToString());
            writer.WriteAttributeString("xmlns", "xsi", null, XmlSchemaInstanceNamespace);
            writer.WriteAttributeString("xmlns", "xsd", null, XmlSchemaNamespace);
            foreach (T item in serializable)
                WriteObject(writer, item, true);
            
            writer.WriteEndElement();
            writer.WriteElementString("End", "");
        }

        private List<T> GetSerializableItems()
        {
            List<T> serializable = new List<T>();
            foreach (T item in this)
                if (!(item is INonSerializableItem))
                    serializable.Add(item);
            return serializable;
        }

        public void ReadXml(XmlReader reader)
        {
            // read aliases
            reader.ReadToFollowing("Aliases");
            reader.MoveToContent();
            reader.Read(); // skip Aliases tag
            _aliases = (List<TypeAlias>)ReadObject(reader, typeof(List<TypeAlias>));

            // read items
            reader.ReadToFollowing("Items");
            reader.MoveToFirstAttribute();
            int count = int.Parse(reader.Value);
            if (count > 0)
            {
                reader.MoveToContent();
                reader.Read(); // skip Items tag
                for (int i = 0; i < count; i++)
                {
                    TypeAlias typeAlias = GetAliasByName(reader.LocalName);
                    Guard.NotNull(typeAlias, "TypeAlias:{0} not found", reader.LocalName);

                    Add((T) ReadObject(reader, typeAlias.Type, typeAlias.Alias));
                }
            }

            reader.ReadToFollowing("End");
            reader.Read();
            reader.ReadEndElement();
        }

        void WriteObject(XmlWriter writer, object obj, bool useAlias)
        {
            Type type = obj.GetType();
            XmlSerializer xs;
            if (useAlias)
                xs = GetSerializer(type, GetAliasByType(type));
            else
                xs = GetSerializer(type, "");

            xs.Serialize(writer, obj);
        }

        object ReadObject(XmlReader reader, Type type)
        {
            return GetSerializer(type, "").Deserialize(reader);
        }

        object ReadObject(XmlReader reader, Type type, string elementName)
        {
            return GetSerializer(type, elementName).Deserialize(reader);
        }

        private void GenerateAliases(IEnumerable<T> items)
        {
            _aliases = new List<TypeAlias>();
            foreach (T item in items)
                GetAliasByType(item.GetType());
        }

        private string GetAliasByType(Type type)
        {
            TypeAlias alias = new TypeAlias(type.Name, type);
            TypeAlias existAlias = GetAliasByName(alias.Alias);

            if (existAlias != null)
            {
                if (existAlias.TypeDesc != alias.TypeDesc)
                {
                    int i = 0;
                    while (GetAliasByName(alias.Alias + i) != null)
                        i++;

                    alias.Alias = alias.Alias + 1;
                    _aliases.Add(alias);
                }
            }
            else
                _aliases.Add(alias);

            return alias.Alias;
        }

        private TypeAlias GetAliasByName(string alias)
        {
            foreach (TypeAlias a in _aliases)
                if (a.Alias == alias)
                    return a;
            return null;
        }

#region XmlSerializer cache

        static readonly Dictionary<string, XmlSerializer> _serializers = new Dictionary<string, XmlSerializer>();

        public XmlSerializer GetSerializer(Type type, string elementName)
        {
            string hash = type.GetHashCode() + elementName;
            if (_serializers.ContainsKey(hash))
                return _serializers[hash];

            XmlSerializer xs = (String.IsNullOrEmpty(elementName)) ? new XmlSerializer(type) : new XmlSerializer(type, new XmlRootAttribute(elementName));
            lock (_serializers)
            {
                if (_serializers.ContainsKey(hash)) // double check
                    return _serializers[hash];

                _serializers[hash] = xs;
                return xs;
            }
        }

        #endregion

#region INotifyCollectionChanged

        public new void Add(T item)
        {
            base.Add(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }

        public void AddRange(IList<T> items)
        {
            base.AddRange(items);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, items);
        }

        public new void AddRange(IEnumerable<T> items)
        {
            throw new NotSupportedException("Please use AddRange(IList<T> items)");
        }

        public new void Remove(T item)
        {
            base.Remove(item);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        }

        public new void RemoveAt(int index)
        {
            T item = this[index];
            base.RemoveAt(index);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, item);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        void OnCollectionChanged(NotifyCollectionChangedAction action, T item)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, item));
        }

        void OnCollectionChanged(NotifyCollectionChangedAction action, IList<T> items)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(action, items));
        }

        #endregion
    }

    [XmlType("TypeAlias")]
    public class TypeAlias
    {
        private string _alias;
        private string _typeDesc;

        public TypeAlias()
        {
        }

        public TypeAlias(string alias, Type type)
        {
            _alias = alias;
            _typeDesc = type.Assembly.ManifestModule.Name + "$" + type.FullName;
        }

        [XmlAttribute]
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        [XmlAttribute]
        public string TypeDesc
        {
            get { return _typeDesc; }
            set { _typeDesc = value; }
        }

        private Type _type = null;
        public Type Type
        {
            get
            {
                if (_type  == null)
                {
                    string [] parts = _typeDesc.Split('$');
                    string assemblyName = parts[0];
                    string typeName = parts[1];
                    Assembly asm = FindAssembly(assemblyName);
                    Guard.NotNull(asm, "Assembly {0} was not found", assemblyName);
                    _type = asm.GetType(typeName);
                }
                return _type;
            }
        }

        private Assembly FindAssembly(string name)
        {
            string asmPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
            return Assembly.LoadFrom(asmPath);
        }
    }
}
#endif