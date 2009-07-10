using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using AnjLab.FX.Windows.Media;

namespace AnjLab.FX.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for BrushesComboBox.xaml
    /// </summary>
    public partial class BrushesComboBox
    {
        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register(
            "SelectedBrush",
            typeof(Brush),
            typeof(BrushesComboBox),
            new PropertyMetadata(SelectedBrushChanged));

        public Brush SelectedBrush
        {
            get { return (Brush)GetValue(SelectedBrushProperty); }
            set { SetValue(SelectedBrushProperty, value); }
        }

        private static void SelectedBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as BrushesComboBox;

            if (control != null && control.ItemsSource != null && control.ItemsSource.GetType().Equals(typeof(BrushData[])))
            {
                var brushData = new List<BrushData>((BrushData[]) control.ItemsSource);
                control.SelectedValue =
                    brushData.Find(
                        brush =>
                        brush.Brush != null
                            ? brush.Brush.Equals(e.NewValue) || brush.Brush.ToString().Equals(e.NewValue.ToString())
                            : e.NewValue == null
                        );
            }
        }

        public BrushesComboBox()
        {
            InitializeComponent();
        }

        void BrushesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (SelectedValue != null)
            {
                var brushData = SelectedValue as BrushData;
                if (brushData != null)
                {
                    SetValue(SelectedBrushProperty, brushData.Brush);
                }
            }
        }
    }
}
