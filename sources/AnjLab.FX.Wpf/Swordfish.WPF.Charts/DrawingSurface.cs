using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Controls;

namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
    public class DrawingSurface : Canvas
    {
        readonly List<Visual> _visuals = new List<Visual>();

        public void AddVisual(Visual visual)
        {
            _visuals.Add(visual);

            AddVisualChild(visual);
            AddLogicalChild(visual);
        }

        public void RemoveVisual(Visual visual)
        {
            _visuals.Remove(visual);

            RemoveLogicalChild(visual);
            RemoveVisualChild(visual);
        }

        protected override int VisualChildrenCount
        {
            get { return _visuals.Count + base.VisualChildrenCount; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < base.VisualChildrenCount)
                return base.GetVisualChild(index);
            else
                return _visuals[base.VisualChildrenCount - index];
        }
    }
}
