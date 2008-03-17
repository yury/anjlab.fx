using System.Configuration;
using Configuration_StringValidator = System.Configuration.StringValidator;

namespace AnjLab.FX.Tools.Data.NHibernate
{
    public class SessionFactoryElement : ConfigurationElement
    {
        public SessionFactoryElement() { }

        public SessionFactoryElement(string name, string configPath) {
            Name = name;
            FactoryConfigPath = configPath;
        }

        [ConfigurationProperty("name", IsRequired = true, IsKey = true, DefaultValue = "Not Supplied")]
        public string Name {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("factoryConfigPath", IsRequired = true, DefaultValue = "Not Supplied")]
        public string FactoryConfigPath {
            get { return (string)this["factoryConfigPath"]; }
            set { this["factoryConfigPath"] = value; }
        }

        [ConfigurationProperty("isTransactional", IsRequired = false, DefaultValue = false)]
        public bool IsTransactional {
            get { return (bool)this["isTransactional"]; }
            set { this["isTransactional"] = value; }
        }
    }
}
