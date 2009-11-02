//
// ColorSelector.cs 
// An HSB (hue, saturation, brightness) based
// color picker.
//
// taken from Microsoft.Samples.CustomControls
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AnjLab.FX.Wpf.Controls
{
    public class ColorSelector : Control
    {
        static ColorSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ColorSelector),
                                                     new FrameworkPropertyMetadata(typeof (ColorSelector)));
        }

        public ColorSelector()
        {
            templateApplied = false;
            m_color = Colors.White;
            shouldFindPoint = true;
            SetValue(AProperty, m_color.A);
            SetValue(RProperty, m_color.R);
            SetValue(GProperty, m_color.G);
            SetValue(BProperty, m_color.B);
            SetValue(SelectedColorProperty, m_color);
        }

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_ColorDetail = GetTemplateChild(ColorDetailName) as FrameworkElement;
            m_ColorMarker = GetTemplateChild(ColorMarkerName) as Path;
            m_ColorSlider = GetTemplateChild(ColorSliderName) as SpectrumSlider;
            m_ColorSlider.ValueChanged += BaseColorChanged;


            m_ColorMarker.RenderTransform = markerTransform;
            m_ColorMarker.RenderTransformOrigin = new Point(0.5, 0.5);
            m_ColorDetail.MouseLeftButtonDown += OnMouseLeftButtonDown;
            m_ColorDetail.PreviewMouseMove += OnMouseMove;
            m_ColorDetail.SizeChanged += ColorDetailSizeChanged;

            templateApplied = true;
            shouldFindPoint = true;
            isAlphaChange = false;
        }

        #endregion

        #region Public Properties

        // Gets or sets the selected color.
        public Color SelectedColor
        {
            get { return (Color) GetValue(SelectedColorProperty); }
            set
            {
                SetValue(SelectedColorProperty, value);
                setColor(value);
            }
        }

        // Gets or sets the the selected color in hexadecimal notation.
        public string HexadecimalString
        {
            get { return (string) GetValue(HexadecimalStringProperty); }
            set { SetValue(HexadecimalStringProperty, value); }
        }

        #region RGB Properties

        // Gets or sets the ARGB alpha value of the selected color.
        public byte A
        {
            get { return (byte) GetValue(AProperty); }
            set { SetValue(AProperty, value); }
        }

        // Gets or sets the ARGB red value of the selected color.
        public byte R
        {
            get { return (byte) GetValue(RProperty); }
            set { SetValue(RProperty, value); }
        }

        // Gets or sets the ARGB green value of the selected color.
        public byte G
        {
            get { return (byte) GetValue(GProperty); }
            set { SetValue(GProperty, value); }
        }

        // Gets or sets the ARGB blue value of the selected color.
        public byte B
        {
            get { return (byte) GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }

        #endregion RGB Properties

        #region ScRGB Properties

        // Gets or sets the ScRGB alpha value of the selected color.
        public double ScA
        {
            get { return (double) GetValue(ScAProperty); }
            set { SetValue(ScAProperty, value); }
        }

        // Gets or sets the ScRGB red value of the selected color.
        public double ScR
        {
            get { return (double) GetValue(ScRProperty); }
            set { SetValue(RProperty, value); }
        }

        // Gets or sets the ScRGB green value of the selected color.
        public double ScG
        {
            get { return (double) GetValue(ScGProperty); }
            set { SetValue(GProperty, value); }
        }

        // Gets or sets the ScRGB blue value of the selected color.
        public double ScB
        {
            get { return (double) GetValue(BProperty); }
            set { SetValue(BProperty, value); }
        }

        #endregion ScRGB Properties

        #endregion

        #region Public Events

        public event RoutedPropertyChangedEventHandler<Color> SelectedColorChanged
        {
            add { AddHandler(SelectedColorChangedEvent, value); }

            remove { RemoveHandler(SelectedColorChangedEvent, value); }
        }

        #endregion

        #region Dependency Property Fields

        public static readonly DependencyProperty AProperty =
            DependencyProperty.Register
                ("A", typeof (byte), typeof (ColorSelector),
                 new PropertyMetadata((byte) 255,
                                      AChanged
                     ));

        public static readonly DependencyProperty BProperty =
            DependencyProperty.Register
                ("B", typeof (byte), typeof (ColorSelector),
                 new PropertyMetadata((byte) 255,
                                      BChanged
                     ));

        public static readonly DependencyProperty GProperty =
            DependencyProperty.Register
                ("G", typeof (byte), typeof (ColorSelector),
                 new PropertyMetadata((byte) 255,
                                      GChanged
                     ));

        public static readonly DependencyProperty HexadecimalStringProperty =
            DependencyProperty.Register
                ("HexadecimalString", typeof (string), typeof (ColorSelector),
                 new PropertyMetadata("#FFFFFFFF",
                                      HexadecimalStringChanged
                     ));

        public static readonly DependencyProperty RProperty =
            DependencyProperty.Register
                ("R", typeof (byte), typeof (ColorSelector),
                 new PropertyMetadata((byte) 255,
                                      RChanged
                     ));

        public static readonly DependencyProperty ScAProperty =
            DependencyProperty.Register
                ("ScA", typeof (float), typeof (ColorSelector),
                 new PropertyMetadata((float) 1,
                                      ScAChanged
                     ));

        public static readonly DependencyProperty ScBProperty =
            DependencyProperty.Register
                ("ScB", typeof (float), typeof (ColorSelector),
                 new PropertyMetadata((float) 1,
                                      ScBChanged
                     ));

        public static readonly DependencyProperty ScGProperty =
            DependencyProperty.Register
                ("ScG", typeof (float), typeof (ColorSelector),
                 new PropertyMetadata((float) 1,
                                      ScGChanged
                     ));

        public static readonly DependencyProperty ScRProperty =
            DependencyProperty.Register
                ("ScR", typeof (float), typeof (ColorSelector),
                 new PropertyMetadata((float) 1,
                                      ScRChanged
                     ));

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register
                ("SelectedColor", typeof (Color), typeof (ColorSelector),
                 new PropertyMetadata(Colors.Transparent,
                                      selectedColor_changed
                     ));

        #endregion

        #region RoutedEvent Fields

        public static readonly RoutedEvent SelectedColorChangedEvent = EventManager.RegisterRoutedEvent(
            "SelectedColorChanged",
            RoutingStrategy.Bubble,
            typeof (RoutedPropertyChangedEventHandler<Color>),
            typeof (ColorSelector)
            );

        #endregion

        #region Property Changed Callbacks

        private static void AChanged(DependencyObject d,
                                     DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorSelector) d;
            c.OnAChanged((byte) e.NewValue);
        }

        protected virtual void OnAChanged(byte newValue)
        {
            m_color.A = newValue;
            SetValue(ScAProperty, m_color.ScA);
            SetValue(SelectedColorProperty, m_color);
        }

        private static void RChanged(DependencyObject d,
                                     DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorSelector) d;
            c.OnRChanged((byte) e.NewValue);
        }

        protected virtual void OnRChanged(byte newValue)
        {
            m_color.R = newValue;
            SetValue(ScRProperty, m_color.ScR);
            SetValue(SelectedColorProperty, m_color);
        }


        private static void GChanged(DependencyObject d,
                                     DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorSelector) d;
            c.OnGChanged((byte) e.NewValue);
        }

        protected virtual void OnGChanged(byte newValue)
        {
            m_color.G = newValue;
            SetValue(ScGProperty, m_color.ScG);
            SetValue(SelectedColorProperty, m_color);
        }


        private static void BChanged(DependencyObject d,
                                     DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorSelector) d;
            c.OnBChanged((byte) e.NewValue);
        }

        protected virtual void OnBChanged(byte newValue)
        {
            m_color.B = newValue;
            SetValue(ScBProperty, m_color.ScB);
            SetValue(SelectedColorProperty, m_color);
        }


        private static void ScAChanged(DependencyObject d,
                                       DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorSelector) d;
            c.OnScAChanged((float) e.NewValue);
        }

        protected virtual void OnScAChanged(float newValue)
        {
            isAlphaChange = true;
            if (shouldFindPoint)
            {
                m_color.ScA = newValue;
                SetValue(AProperty, m_color.A);
                SetValue(SelectedColorProperty, m_color);
                SetValue(HexadecimalStringProperty, m_color.ToString());
            }
            isAlphaChange = false;
        }


        private static void ScRChanged(DependencyObject d,
                                       DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorSelector) d;
            c.OnScRChanged((float) e.NewValue);
        }

        protected virtual void OnScRChanged(float newValue)
        {
            if (shouldFindPoint)
            {
                m_color.ScR = newValue;
                SetValue(RProperty, m_color.R);
                SetValue(SelectedColorProperty, m_color);
                SetValue(HexadecimalStringProperty, m_color.ToString());
            }
        }


        private static void ScGChanged(DependencyObject d,
                                       DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorSelector) d;
            c.OnScGChanged((float) e.NewValue);
        }

        protected virtual void OnScGChanged(float newValue)
        {
            if (shouldFindPoint)
            {
                m_color.ScG = newValue;
                SetValue(GProperty, m_color.G);
                SetValue(SelectedColorProperty, m_color);
                SetValue(HexadecimalStringProperty, m_color.ToString());
            }
        }


        private static void ScBChanged(DependencyObject d,
                                       DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorSelector) d;
            c.OnScBChanged((float) e.NewValue);
        }

        protected virtual void OnScBChanged(float newValue)
        {
            if (shouldFindPoint)
            {
                m_color.ScB = newValue;
                SetValue(BProperty, m_color.B);
                SetValue(SelectedColorProperty, m_color);
                SetValue(HexadecimalStringProperty, m_color.ToString());
            }
        }

        private static void HexadecimalStringChanged(DependencyObject d,
                                                     DependencyPropertyChangedEventArgs e)
        {
            var c = (ColorSelector) d;
            c.OnHexadecimalStringChanged((string) e.OldValue, (string) e.NewValue);
        }

        protected virtual void OnHexadecimalStringChanged(string oldValue, string newValue)
        {
            try
            {
                if (shouldFindPoint)
                {
                    m_color = (Color) ColorConverter.ConvertFromString(newValue);
                }

                SetValue(AProperty, m_color.A);
                SetValue(RProperty, m_color.R);
                SetValue(GProperty, m_color.G);
                SetValue(BProperty, m_color.B);


                if (shouldFindPoint && !isAlphaChange && templateApplied)
                {
                    updateMarkerPosition(m_color);
                }
            }
            catch (FormatException)
            {
                SetValue(HexadecimalStringProperty, oldValue);
            }
        }

        private static void selectedColor_changed(DependencyObject d,
                                                  DependencyPropertyChangedEventArgs e)
        {
            var cPicker = (ColorSelector) d;
            cPicker.OnSelectedColorChanged((Color) e.OldValue, (Color) e.NewValue);
        }

        protected virtual void OnSelectedColorChanged(Color oldColor, Color newColor)
        {
            var newEventArgs =
                new RoutedPropertyChangedEventArgs<Color>(oldColor, newColor);
            newEventArgs.RoutedEvent = SelectedColorChangedEvent;
            RaiseEvent(newEventArgs);
        }

        #endregion

        #region Template Part Event Handlers

        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            templateApplied = false;
            if (oldTemplate != null)
            {
                m_ColorSlider.ValueChanged -= BaseColorChanged;
                m_ColorDetail.MouseLeftButtonDown -= OnMouseLeftButtonDown;
                m_ColorDetail.PreviewMouseMove -= OnMouseMove;
                m_ColorDetail.SizeChanged -= ColorDetailSizeChanged;
                m_ColorDetail = null;
                m_ColorMarker = null;
                m_ColorSlider = null;
            }
            base.OnTemplateChanged(oldTemplate, newTemplate);
        }


        private void BaseColorChanged(
            object sender,
            RoutedPropertyChangedEventArgs<Double> e)
        {
            if (m_ColorPosition != null)
            {
                determineColor((Point) m_ColorPosition);
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(m_ColorDetail);
            updateMarkerPosition(p);
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(m_ColorDetail);
                updateMarkerPosition(p);
                Mouse.Synchronize();
            }
        }

        private void ColorDetailSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (args.PreviousSize != Size.Empty &&
                args.PreviousSize.Width != 0 && args.PreviousSize.Height != 0)
            {
                double widthDifference = args.NewSize.Width/args.PreviousSize.Width;
                double heightDifference = args.NewSize.Height/args.PreviousSize.Height;
                markerTransform.X = markerTransform.X*widthDifference;
                markerTransform.Y = markerTransform.Y*heightDifference;
            }
            else if (m_ColorPosition != null)
            {
                markerTransform.X = ((Point) m_ColorPosition).X*args.NewSize.Width;
                markerTransform.Y = ((Point) m_ColorPosition).Y*args.NewSize.Height;
            }
        }

        #endregion

        #region Color Resolution Helpers

        private void setColor(Color theColor)
        {
            m_color = theColor;

            if (templateApplied)
            {
                SetValue(AProperty, m_color.A);
                SetValue(RProperty, m_color.R);
                SetValue(GProperty, m_color.G);
                SetValue(BProperty, m_color.B);
                updateMarkerPosition(theColor);
            }
        }

        private void updateMarkerPosition(Point p)
        {
            markerTransform.X = p.X;
            markerTransform.Y = p.Y;
            p.X = p.X/m_ColorDetail.ActualWidth;
            p.Y = p.Y/m_ColorDetail.ActualHeight;
            m_ColorPosition = p;
            determineColor(p);
        }

        private void updateMarkerPosition(Color theColor)
        {
            m_ColorPosition = null;


            HsvColor hsv = ColorUtilities.ConvertRgbToHsv(theColor.R, theColor.G, theColor.B);

            m_ColorSlider.Value = hsv.H;

            var p = new Point(hsv.S, 1 - hsv.V);

            m_ColorPosition = p;
            p.X = p.X*m_ColorDetail.ActualWidth;
            p.Y = p.Y*m_ColorDetail.ActualHeight;
            markerTransform.X = p.X;
            markerTransform.Y = p.Y;
        }

        private void determineColor(Point p)
        {
            var hsv = new HsvColor(360 - m_ColorSlider.Value, 1, 1);
            hsv.S = p.X;
            hsv.V = 1 - p.Y;
            m_color = ColorUtilities.ConvertHsvToRgb(hsv.H, hsv.S, hsv.V);
            shouldFindPoint = false;
            m_color.ScA = (float) GetValue(ScAProperty);
            SetValue(HexadecimalStringProperty, m_color.ToString());
            shouldFindPoint = true;
        }

        #endregion

        #region Private Fields

        private static readonly string ColorDetailName = "PART_ColorDetail";
        private static readonly string ColorMarkerName = "PART_ColorMarker";
        private static readonly string ColorSliderName = "PART_ColorSlider";
        private readonly TranslateTransform markerTransform = new TranslateTransform();
        private bool isAlphaChange;
        private Color m_color;
        private FrameworkElement m_ColorDetail;
        private Path m_ColorMarker;
        private Point? m_ColorPosition;
        private SpectrumSlider m_ColorSlider;
        private bool shouldFindPoint;
        private bool templateApplied;

        #endregion
    }

    #region SpectrumSlider

    public class SpectrumSlider : Slider
    {
        static SpectrumSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (SpectrumSlider),
                                                     new FrameworkPropertyMetadata(typeof (SpectrumSlider)));
        }

        #region Public Properties

        public Color SelectedColor
        {
            get { return (Color) GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        #endregion

        #region Dependency Property Fields

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register
                ("SelectedColor", typeof (Color), typeof (SpectrumSlider),
                 new PropertyMetadata(Colors.Transparent));

        #endregion

        #region Public Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            m_spectrumDisplay = GetTemplateChild(SpectrumDisplayName) as Rectangle;
            updateColorSpectrum();
            OnValueChanged(Double.NaN, Value);
        }

        #endregion

        #region Protected Methods

        protected override void OnValueChanged(double oldValue, double newValue)
        {
            base.OnValueChanged(oldValue, newValue);
            Color theColor = ColorUtilities.ConvertHsvToRgb(360 - newValue, 1, 1);
            SetValue(SelectedColorProperty, theColor);
        }

        #endregion

        #region Private Methods

        private void updateColorSpectrum()
        {
            if (m_spectrumDisplay != null)
            {
                createSpectrum();
            }
        }


        private void createSpectrum()
        {
            pickerBrush = new LinearGradientBrush();
            pickerBrush.StartPoint = new Point(0.5, 0);
            pickerBrush.EndPoint = new Point(0.5, 1);
            pickerBrush.ColorInterpolationMode = ColorInterpolationMode.SRgbLinearInterpolation;


            List<Color> colorsList = ColorUtilities.GenerateHsvSpectrum();
            double stopIncrement = (double) 1/colorsList.Count;

            int i;
            for (i = 0; i < colorsList.Count; i++)
            {
                pickerBrush.GradientStops.Add(new GradientStop(colorsList[i], i*stopIncrement));
            }

            pickerBrush.GradientStops[i - 1].Offset = 1.0;
            m_spectrumDisplay.Fill = pickerBrush;
        }

        #endregion

        #region Private Fields

        private static string SpectrumDisplayName = "PART_SpectrumDisplay";
        private Rectangle m_spectrumDisplay;
        private LinearGradientBrush pickerBrush;

        #endregion
    }

    #endregion SpectrumSlider

    #region ColorUtilities

    internal static class ColorUtilities
    {
        // Converts an RGB color to an HSV color.
        public static HsvColor ConvertRgbToHsv(int r, int b, int g)
        {
            double delta, min;
            double h = 0, s, v;

            min = Math.Min(Math.Min(r, g), b);
            v = Math.Max(Math.Max(r, g), b);
            delta = v - min;

            if (v == 0.0)
            {
                s = 0;
            }
            else
                s = delta/v;

            if (s == 0)
                h = 0.0;

            else
            {
                if (r == v)
                    h = (g - b)/delta;
                else if (g == v)
                    h = 2 + (b - r)/delta;
                else if (b == v)
                    h = 4 + (r - g)/delta;

                h *= 60;
                if (h < 0.0)
                    h = h + 360;
            }

            var hsvColor = new HsvColor();
            hsvColor.H = h;
            hsvColor.S = s;
            hsvColor.V = v/255;

            return hsvColor;
        }

        // Converts an HSV color to an RGB color.
        public static Color ConvertHsvToRgb(double h, double s, double v)
        {
            double r = 0, g = 0, b = 0;

            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                int i;
                double f, p, q, t;


                if (h == 360)
                    h = 0;
                else
                    h = h/60;

                i = (int) Math.Truncate(h);
                f = h - i;

                p = v*(1.0 - s);
                q = v*(1.0 - (s*f));
                t = v*(1.0 - (s*(1.0 - f)));

                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }


            return Color.FromArgb(255, (byte) (r*255), (byte) (g*255), (byte) (b*255));
        }

        // Generates a list of colors with hues ranging from 0 360
        // and a saturation and value of 1. 
        public static List<Color> GenerateHsvSpectrum()
        {
            var colorsList = new List<Color>(8);


            for (int i = 0; i < 29; i++)
            {
                colorsList.Add(
                    ConvertHsvToRgb(i*12, 1, 1)
                    );
            }
            colorsList.Add(ConvertHsvToRgb(0, 1, 1));


            return colorsList;
        }
    }

    #endregion ColorUtilities

    // Describes a color in terms of
    // Hue, Saturation, and Value (brightness)

    #region HsvColor

    internal struct HsvColor
    {
        public double H;
        public double S;
        public double V;

        public HsvColor(double h, double s, double v)
        {
            H = h;
            S = s;
            V = v;
        }
    }

    #endregion HsvColor

    #region ColorThumb

    public class ColorThumb : Thumb
    {
        public static readonly DependencyProperty PointerOutlineBrushProperty =
            DependencyProperty.Register
                ("PointerOutlineBrush", typeof (Brush), typeof (ColorThumb),
                 new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty PointerOutlineThicknessProperty =
            DependencyProperty.Register
                ("PointerOutlineThickness", typeof (double), typeof (ColorThumb),
                 new FrameworkPropertyMetadata(1.0));

        public static readonly DependencyProperty ThumbColorProperty =
            DependencyProperty.Register
                ("ThumbColor", typeof (Color), typeof (ColorThumb),
                 new FrameworkPropertyMetadata(Colors.Transparent));

        static ColorThumb()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (ColorThumb),
                                                     new FrameworkPropertyMetadata(typeof (ColorThumb)));
        }


        public Color ThumbColor
        {
            get { return (Color) GetValue(ThumbColorProperty); }
            set { SetValue(ThumbColorProperty, value); }
        }

        public double PointerOutlineThickness
        {
            get { return (double) GetValue(PointerOutlineThicknessProperty); }
            set { SetValue(PointerOutlineThicknessProperty, value); }
        }

        public Brush PointerOutlineBrush
        {
            get { return (Brush) GetValue(PointerOutlineBrushProperty); }
            set { SetValue(PointerOutlineBrushProperty, value); }
        }
    }

    #endregion ColorThumb
}