using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using AnjLab.FX.Sys;

namespace AnjLab.FX.Wpf.Controls
{
    public class ColorPicker : Control
    {
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor",
                                                                                                      typeof (Color),
                                                                                                      typeof (ColorPicker),
                                                                                                      new PropertyMetadata(SelectedColor_Changed));

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register("SelectedBrush",
                                                                                                      typeof(Brush),
                                                                                                      typeof(ColorPicker),
                                                                                                      new PropertyMetadata(
                                                                                                          new SolidColorBrush(),
                                                                                                          SelectedBrush_Changed));

        static ColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPicker), new FrameworkPropertyMetadata(typeof(ColorPicker)));
        }

        public event EventHandler<EventArgs<Color>> SelectedColorChanged;

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value);}
        }

        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var colorSelectorPopup = GetTemplateChild("colorSelectorPopup") as Popup;
            if (colorSelectorPopup != null) colorSelectorPopup.Opened += colorSelectorPopup_Opened;

            var okButton = GetTemplateChild("PART_okButton") as Button;
            if (okButton != null) okButton.Click += okButton_Click;

            var cancelButton = GetTemplateChild("PART_cancelButton") as Button;
            if (cancelButton != null) cancelButton.Click += cancelButton_Click;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClosePopup();
        }

        void ClosePopup()
        {
            var colorSelectorPopup = GetTemplateChild("colorSelectorPopup") as Popup;
            if (colorSelectorPopup != null) colorSelectorPopup.IsOpen = false;
        }

        void okButton_Click(object sender, RoutedEventArgs e)
        {
            var colorSelector = GetTemplateChild("colorSelector") as ColorSelector;
            if (colorSelector != null)
            {
                SelectedColor = colorSelector.SelectedColor;
                if (SelectedColorChanged != null)
                    SelectedColorChanged(this, EventArg.New(SelectedColor));
            }

            ClosePopup();
        }

        void colorSelectorPopup_Opened(object sender, EventArgs e)
        {
            var colorSelector = GetTemplateChild("colorSelector") as ColorSelector;
            if (colorSelector != null)
            {
                colorSelector.SelectedColor = SelectedColor;
                colorSelector.Focus();
            }
        }

        private static void SelectedBrush_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var solidBrush = e.NewValue as SolidColorBrush;
            if (solidBrush != null && ((ColorPicker)d).SelectedColor != solidBrush.Color)
                ((ColorPicker) d).SelectedColor = solidBrush.Color;
        }

        private static void SelectedColor_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ColorPicker) d).SelectedBrush = new SolidColorBrush((Color)e.NewValue);
        }
    }
}
