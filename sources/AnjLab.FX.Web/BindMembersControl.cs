using System;
using System.Web.UI;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Web.Controls
{
    public class BindMembersControl : Control
    {
        public BindMembersControl(Control c)
        {
            BindMemberControls(c);
        }

        protected void BindMemberControls(Control container)
        {
            foreach (TemplateElementAttribute element in AttributeSearcher.GetMemberAttributes<TemplateElementAttribute>(this.GetType(), false))
            {
                Control control = ControlsSearcher.FindControl(container, element.ID);
                if (control == null && !element.CanBeNull)
                    throw new InvalidOperationException(String.Format("Element {0} not found in parent control {1}", element.ID, control.ID));
                else
                    element[this] = control;
            }
        }
    }
}
