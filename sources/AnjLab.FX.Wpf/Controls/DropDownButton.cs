using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace AnjLab.FX.Wpf.Controls
{
    public class DropDownButton : ToggleButton
    {
        // *** Dependency Properties ***

        public static readonly DependencyProperty DropDownProperty = DependencyProperty.Register("DropDown", typeof(ContextMenu), typeof(DropDownButton), new UIPropertyMetadata(null));

        public static readonly DependencyProperty MenuPlacementProperty = DependencyProperty.Register("MenuPlacement", typeof(PlacementMode), typeof(DropDownButton), new UIPropertyMetadata(null));

        // *** Constructors ***

        public DropDownButton()
        {
            // Bind the ToogleButton.IsChecked property to the drop-down's IsOpen property

            var binding = new Binding("DropDown.IsOpen") {Source = this};
            this.SetBinding(IsCheckedProperty, binding);
        }

        // *** Properties ***

        public ContextMenu DropDown
        {
            get
            {
                return (ContextMenu)GetValue(DropDownProperty);
            }
            set
            {
                SetValue(DropDownProperty, value);
            }
        }

        public PlacementMode MenuPlacement
        {
            get
            {
                return (PlacementMode)GetValue(MenuPlacementProperty);
            }
            set
            {
                SetValue(MenuPlacementProperty, value);
            }
        }

        // *** Overridden Methods ***

        protected override void OnClick()
        {
            if (DropDown != null)
            {
                // If there is a drop-down assigned to this button, then position and display it

                DropDown.PlacementTarget = this;
                DropDown.Placement = MenuPlacement;

                DropDown.IsOpen = true;
            }
        }
    }
}
