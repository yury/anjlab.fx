using System.Configuration;

namespace AnjLab.FX.Tools.Data.NHibernate
{
    /// <summary>
    /// Encapsulates a section of Web/App.config to declare which session factories are to be created.
    /// Huge kudos go out to http://msdn2.microsoft.com/en-us/library/system.configuration.configurationcollectionattribute.aspx
    /// for this technique - it was by far the best overview of the subject.
    /// </summary>
    public class OpenSessionInViewSection : ConfigurationSection
    {
        [ConfigurationProperty("sessionFactories", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(SessionFactoriesCollection), AddItemName = "sessionFactory",
            ClearItemsName = "clearFactories")]
        public SessionFactoriesCollection SessionFactories {
            get {
                SessionFactoriesCollection sessionFactoriesCollection =
                    (SessionFactoriesCollection)base["sessionFactories"];
                return sessionFactoriesCollection;
            }
        }
    }
}
