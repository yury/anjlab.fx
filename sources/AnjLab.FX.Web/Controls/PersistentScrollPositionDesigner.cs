using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.Design;

namespace AnjLab.FX.Web.Controls.Design
{
	public class PersistentScrollPositionDesigner : ControlDesigner
	{
		public override string GetDesignTimeHtml()
		{
			return string.Format("<table style=\"border:1px solid #CCCCCC;\" cellspacing=\"0\" cellpadding=\"0\">\r\n<tr>\r\n<td nowrap style=\"font:messagebox;background-color:#ffffff;color:#444444;background-position:bottom;background-repeat:repeat-x;padding:4px;\"><strong>{0}</strong> - {1}</td>\r\n</tr></table>", "PersistentScrollPosition", PersistentScrollPositionControl.ID.ToString());
		}

		private PersistentScrollPosition PersistentScrollPositionControl
		{
			get
			{
				return (PersistentScrollPosition)base.Component;
			}
		}
	}
}
