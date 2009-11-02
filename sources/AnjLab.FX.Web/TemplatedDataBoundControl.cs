using System;
using System.Collections.Generic;
using System.Web.Hosting;
using System.Web.UI;
using AnjLab.FX.Sys;
using System.Web.UI.WebControls;

namespace AnjLab.FX.Web.Controls
{
    /// <summary>Base class for all controls which use ascx templates </summary>
    [ParseChildren(true)]
    public class TemplatedDataBoundControl : DataBoundControl, INamingContainer
    {
        private IList<TemplateElementAttribute> _templateElements;
        private string _templateFile = String.Empty;

        public TemplatedDataBoundControl()
        {
            TemplateAttribute templateAttribute = AttributeSearcher.GetAttribute<TemplateAttribute>(GetType());
            if (templateAttribute != null)
                LoadTemplate(templateAttribute.FileName);
        }

        public TemplatedDataBoundControl(string templateFile)
        {
            LoadTemplate(templateFile);
        }

        private void LoadTemplate(string templateFile)
        {
            _templateFile = templateFile;
            ITemplate ascx = GetTemplate(_templateFile);

            Controls.Clear();
            ascx.InstantiateIn(this);
            Controls[0].ID = "_";
            
            _templateElements = AttributeSearcher.GetMemberAttributes<TemplateElementAttribute>(GetType(), true);
            BindTemplateElements(false);
        }

        protected static ITemplate GetTemplate(string path)
        {
            if (!HostingEnvironment.VirtualPathProvider.FileExists(path))
                throw new InvalidOperationException(String.Format("Ascx template does not found {0}", path));

            return new Page().LoadTemplate(path);
        }

        private void BindTemplateElements(bool onLoadElements)
        {
            foreach (TemplateElementAttribute templateElement in _templateElements)
            {
                if (templateElement.BindOnLoad == onLoadElements)
                {
                    Control element = LookForControl(templateElement.ID);
                    if (element == null)
                        throw new InvalidOperationException(String.Format("Element {0} not found in template {1}", templateElement.ID, _templateFile));
                    templateElement[this] = element;
                }
            }
        }

        public Control LookForControl(string id)
        {
            if (Controls.Count == 0)
                return null;

            return ControlsSearcher.FindControl(Controls[0], id);
        }

        protected override void OnLoad(EventArgs e)
        {
            BindTemplateElements(true);
            base.OnLoad(e);
        }

        #region Properties

        public string TemplateFile
        {
            get { return _templateFile; }
            set { LoadTemplate(value); }
        }

        #endregion
    }
}
