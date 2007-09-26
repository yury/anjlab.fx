using System.Web.UI;

namespace AnjLab.FX.Web
{
    public class ControlsSearcher
    {
        public static Control FindControl(Control container, string id)
        {
            Control res = container.FindControl(id);
            if (res != null && res.ID == id)
                return res;

            foreach (Control child in container.Controls)
            {
                if (child.ID == id)
                    return child;

                if (child.HasControls())
                {
                    res = FindControl(child, id);
                    if (res != null)
                        return res;
                }
            }
            return null;
        }
    }
}
