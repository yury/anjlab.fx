using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using AnjLab.FX.System;

namespace AnjLab.FX.Collections
{
    public class SerializableList<T> : List<T>, IXmlSerializable
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
            GenerateAliases();

            writer.WriteStartElement("Aliases");
            WriteObject(writer, _aliases, false);
            writer.WriteEndElement();

            writer.WriteStartElement("Items");
            writer.WriteAttributeString("count", "", this.Count.ToString());
            writer.WriteAttributeString("xmlns", "xsi", null, XmlSchemaInstanceNamespace);
            writer.WriteAttributeString("xmlns", "xsd", null, XmlSchemaNamespace);
            foreach (T item in this)
                WriteObject(writer, item, true);
            writer.WriteEndElement();
        }

        private void GenerateAliases()
        {
            foreach (T item in this)
                GetAlias(item.GetType());
        }

        public void ReadXml(XmlReader reader)
        {
            // read aliases
            reader.ReadToFollowing("Aliases");
            reader.MoveToContent();
            reader.Read(); // skip Aliases tag
            _aliases = (List<TypeAlias>)DeserializeObject(reader, typeof(List<TypeAlias>));

            // read items
            reader.ReadToFollowing("Items");
            reader.MoveToFirstAttribute();
            int count = int.Parse(reader.Value);
            reader.MoveToContent();
            reader.Read();// skip Items tag
            for(int i = 0; i < count; i++)
            {
                TypeAlias typeAlias = GetTypeAlias(reader.LocalName);
                Guard.NotNull(typeAlias, "TypeAlias:{0} not found", reader.LocalName);

                this.Add((T) DeserializeObject(reader, typeAlias.Type, typeAlias.Alias));
            }
            
            reader.ReadEndElement();
        }

        void WriteObject(XmlWriter writer, object obj, bool useAlias)
        {
            Type type = obj.GetType();
            XmlSerializer xs = (useAlias) ? new XmlSerializer(type, new XmlRootAttribute(GetAlias(type))) : new XmlSerializer(type);

            xs.Serialize(writer, obj);
        }

        private string GetAlias(Type type)
        {
            TypeAlias alias = new TypeAlias(type.Name, type);
            TypeAlias existAlias = GetTypeAlias(alias.Alias);

            if (existAlias != null)
            {
                if (existAlias.TypeDesc != alias.TypeDesc)
                {
                    int i = 0;
                    while (GetTypeAlias(alias.Alias + i) != null)
                        i++;

                    alias.Alias = alias.Alias + 1;
                    _aliases.Add(alias);
                }
            }
            else
                _aliases.Add(alias);

            return alias.Alias;
        }

        private TypeAlias GetTypeAlias(string alias)
        {
            foreach (TypeAlias a in _aliases)
                if (a.Alias == alias)
                    return a;
            return null;
        }

        object DeserializeObject(XmlReader reader, Type type)
        {
            XmlSerializer xs = new XmlSerializer(type);
            return xs.Deserialize(reader);
        }
        
        object DeserializeObject(XmlReader reader, Type type, string elementName)
        {
            XmlSerializer xs = new XmlSerializer(type, new XmlRootAttribute(elementName));
            return xs.Deserialize(reader);
        }
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
            _typeDesc = type.Assembly.ManifestModule.FullyQualifiedName + "$" + type.FullName;
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
                    string assembly = parts[0];
                    string typeName = parts[1];
                    _type = Assembly.LoadFrom(assembly).GetType(typeName);
                }
                return _type;
            }
        }
    }
}
