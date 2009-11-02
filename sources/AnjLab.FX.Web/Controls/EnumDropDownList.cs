using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AnjLab.FX.Web.Controls
{
    [DefaultProperty("EnumNames")]
    [ToolboxData("<{0}:EnumDropDownList runat=server></{0}:EnumDropDownList>")]
    public class EnumDropDownList : DropDownList
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string EnumNames
        {
            get
            {
                String s = (String)ViewState["EnumNames"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["EnumNames"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string EnumType
        {
            get
            {
                String s = (String)ViewState["EnumType"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["EnumType"] = value;
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string SelectedEnum
        {
            get
            {
                String s = (String)ViewState["SelectedEnum"];
                return (s ?? String.Empty);
            }

            set
            {
                ViewState["SelectedEnum"] = value;
            }
        }

        public new int SelectedValue
        {
            get
            {
                return Convert.ToInt32(base.SelectedValue);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if(string.IsNullOrEmpty(EnumType) || string.IsNullOrEmpty(EnumNames) || this.Items.Count > 0)
                return;

            Type type = Type.GetType(EnumType, true);
            Array values = Enum.GetValues(type);
            string[] names = EnumNames.Split(',');
            for (int i = 0; i < names.Length; i++)
            {
                if(names[i] != "!")
                    Items.Add(new ListItem(names[i], ((int) values.GetValue(i)).ToString()));

                if(values.GetValue(i).ToString() == SelectedEnum)
                    Items[Items.Count - 1].Selected = true;
            }
        }
    }
}
