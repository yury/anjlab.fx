using System.Windows;

namespace AnjLab.FX.Wpf.Controls
{
    public class ColorPickerLight : ColorPicker
    {
        static ColorPickerLight()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorPickerLight), new FrameworkPropertyMetadata(typeof(ColorPickerLight)));
        }
    }
}
