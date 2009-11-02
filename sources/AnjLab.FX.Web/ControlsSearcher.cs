using System;
using System.Web.UI;
using System.Collections.Generic;

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

        public static IList<T> FindControls<T>(Control container) where T : Control
        {
            List<T> controls = new List<T>();
            foreach (Control child in container.Controls)
            {
                if (child.GetType() == typeof(T))
                    controls.Add((T)child);

                if(child.HasControls())
                    controls.AddRange(FindControls<T>(child));
            }

            return controls;
        }
    }
}
