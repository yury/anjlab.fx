using System.Configuration;
using System.Xml;

namespace AnjLab.FX.Tasks.Scheduling
{
    public class SchedulerConfigSection : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        object IConfigurationSectionHandler.Create(
          object parent, object configContext, XmlNode section)
        {
            return Trigger.ReadTriggers(section.OuterXml);
        }

        #endregion
    }

}
