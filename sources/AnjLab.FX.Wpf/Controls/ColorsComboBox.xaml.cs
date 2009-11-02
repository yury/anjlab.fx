using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using AnjLab.FX.Windows.Media;

namespace AnjLab.FX.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for BrushesComboBox.xaml
    /// </summary>
    public partial class ColorsComboBox
    {
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register(
            "SelectedColor",
            typeof(Color),
            typeof(ColorsComboBox),
            new PropertyMetadata(SelectedColorChanged));

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        private static void SelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ColorsComboBox;

            if (control != null && control.ItemsSource != null && control.ItemsSource.GetType().Equals(typeof(ColorData[])))
            {
                var brushData = new List<ColorData>((ColorData[])control.ItemsSource);
                control.SelectedValue =
                    brushData.Find(brush => brush.Color.Equals(e.NewValue) || brush.Color.ToString().Equals(e.NewValue.ToString()));
            }
        }

        public ColorsComboBox()
        {
            InitializeComponent();
        }

        void BrushesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SelectedValue != null)
            {
                var brushData = SelectedValue as ColorData;
                if (brushData != null)
                {
                    SetValue(SelectedColorProperty, brushData.Color);
                }
            }
        }
    }
}
