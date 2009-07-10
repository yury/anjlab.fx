using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AnjLab.FX.Web.Controls
{
    [ToolboxData("<{0}:ScrollSaver runat=server></{0}:ScrollSaver>")]
    public class ScrollSaver : WebControl
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this.Page.MaintainScrollPositionOnPostBack = true;
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
        }
    }
}
