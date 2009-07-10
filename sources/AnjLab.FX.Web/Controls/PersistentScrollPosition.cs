using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Reflection;
using System.Web.UI.WebControls;

namespace AnjLab.FX.Web.Controls
{
    /// <summary>
    /// This code was taken from http://www.codeproject.com/KB/ajax/AJAXPersistScrollPosition.aspx
    /// The original author of this code is Steven Berkovitz
    /// </summary>
	[DefaultProperty("ControlToPersist")]
	[ToolboxData("<{0}:PersistentScrollPosition runat=\"server\" ControlToPersist=\"\" />")]
	[NonVisualControl()]
	[Designer(typeof(AnjLab.FX.Web.Controls.Design.PersistentScrollPositionDesigner))]
	public class PersistentScrollPosition : Control, IScriptControl, INamingContainer
	{
		#region Internal Members
		private ScriptManager _sm = null;
		private Control _control  = null;
		private HiddenField storage;
		#endregion

		#region Ctor(s)
		/// <summary>
		/// Initializes a new instance of the <see cref="PersistentScrollPosition"/> class.
		/// </summary>
		public PersistentScrollPosition()
		{
		}
		#endregion

		#region Base Class Overrides
		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

			// Create hidden control for storage
			storage = new HiddenField();
			storage.ID = "storage";			
			Controls.Add(storage);
		}

		protected override void OnPreRender(EventArgs e)
		{
            if (!this.DesignMode)
                ScriptManager.RegisterScriptControl(this);
						
			base.OnPreRender(e);
		}

		protected override void Render(HtmlTextWriter writer)
		{	
			if(!this.DesignMode)
				ScriptManager.RegisterScriptDescriptors(this);

			base.Render(writer);
		}

		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region Properties		
		/// <summary>
		/// Gets or sets the control to persist.
		/// </summary>
		/// <value>The control to persist.</value>
		public string ControlToPersist
		{
			get
			{
				object o = ViewState["ControlToPersist"];
				return (o != null) ? (string)o : string.Empty;
			}
			set
			{
				ViewState["ControlToPersist"] = value;
			}
		}

		/// <summary>
		/// Gets the script manager
		/// </summary>
		/// <value>The script manager</value>
		internal ScriptManager ScriptManager
		{
			get
			{
				if(_sm == null)
				{
					Page page = Page;
					if(page == null)
						throw new InvalidOperationException("Page cannot be null");

					_sm = ScriptManager.GetCurrent(page);
					if(_sm == null)
						throw new InvalidOperationException("A ScriptManager is required");
				}

				return _sm;
			}
		}

		/// <summary>
		/// Gets the control.
		/// </summary>
		/// <value>The control.</value>
		protected internal Control Control
		{
			get
			{
				if(_control == null)
				{
					Page page = Page;
					if(page == null)
						throw new InvalidOperationException("Page cannot be null");

					_control = (Control)FindControlRecursive(Page, ControlToPersist);
					if(_control == null)
						throw new InvalidOperationException("Could not located the specified control");
				}

				return _control;
			}
		}
		#endregion

		#region IScriptControl Members

		public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
		{
			ScriptComponentDescriptor scd = new ScriptBehaviorDescriptor("AnjLab.FX.Web.Controls.PersistentScrollPosition", Control.ClientID);			
			scd.AddElementProperty("storage", storage.ClientID);
			yield return scd;
		}

		public IEnumerable<ScriptReference> GetScriptReferences()
		{
#if DEBUG
			yield return new ScriptReference("AnjLab.FX.Web.Controls.PersistentScrollPosition.debug.js", Assembly.GetAssembly(typeof(PersistentScrollPosition)).FullName);
#else
			yield return new ScriptReference("AnjLab.FX.Web.Controls.PersistentScrollPosition.release.js", Assembly.GetAssembly(typeof(PersistentScrollPosition)).FullName);
#endif
		}

		#endregion

		#region Helper
		/// <summary>
		/// Finds the control
		/// </summary>
		/// <param name="root">The root control to search.</param>
		/// <param name="id">The id.</param>
		/// <returns></returns>
		protected virtual Control FindControlRecursive(Control root, string id)
		{
			if(root.ID == id)
			{
				return root;
			}

			foreach(Control c in root.Controls)
			{
				Control t = FindControlRecursive(c, id);
				if(t != null)
				{
					return t;
				}
			}

			return null;
		}
		#endregion
	}
}
