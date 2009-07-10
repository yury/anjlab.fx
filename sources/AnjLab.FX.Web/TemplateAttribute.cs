using System;

namespace AnjLab.FX.Web
{
    /// <summary>Use this class to define control's ascx skin file</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TemplateAttribute : Attribute
    {
        private readonly string _ascxFile;

        public TemplateAttribute(string ascxFileName)
        {
            _ascxFile = ascxFileName;
        }

        public string FileName { get { return _ascxFile; } }
    }
}
