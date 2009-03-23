// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish\WPF\Controls\ColorLabel.xaml.cs                       **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
	/// <summary>
	/// Interaction logic for ColorLabel.xaml
	/// </summary>
	public partial class ColorLabel : UserControl
	{
		// ********************************************************************
		// Constructors
		// ********************************************************************
		#region Constructors

		/// <summary>
		/// Constructor. Initializes class fields.
		/// </summary>
		public ColorLabel()
		{
			InitializeComponent();
			Background = new SolidColorBrush(Colors.Transparent);
		}

		/// <summary>
		/// Initializes the text and color properties.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="color"></param>
		public ColorLabel(string text, Color color) : this()
		{
			Text = text;
			Color = color;
		}

		#endregion Constructors

		// ********************************************************************
		// DependencyProperties
		// ********************************************************************
		#region DependencyProperties

		// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(ColorLabel), new UIPropertyMetadata("Blank", UpdateText));

		// Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty ColorProperty =
			DependencyProperty.Register("Color", typeof(Color), typeof(ColorLabel), new UIPropertyMetadata(Colors.Violet, UpdateColor));

		#endregion DependencyProperties

		// ********************************************************************
		// Properties
		// ********************************************************************
		#region Properties

		/// <summary>
		/// Gets/Sets the text of the color label
		/// </summary>
		public string Text
		{
			get
			{
				return (string)GetValue(TextProperty);
			}
			set
			{
				SetValue(TextProperty, value);
			}
		}

		/// <summary>
		/// Gets/Sets the color to be displayed
		/// </summary>
		public Color Color
		{
			get
			{
				return (Color)GetValue(ColorProperty);
			}
			set
			{
				SetValue(ColorProperty, value);
			}
		}

		#endregion Properties

		// ********************************************************************
		// Event Handlers
		// ********************************************************************
		#region Event Handlers

		/// <summary>
		/// Handles when the Color property is changed
		/// </summary>
		/// <param name="dependency"></param>
		/// <param name="e"></param>
		protected static void UpdateColor(DependencyObject dependency, DependencyPropertyChangedEventArgs e)
		{
			ColorLabel colorLabel = (ColorLabel)dependency;
			Color newColor = (Color)e.NewValue;
			colorLabel.color.Color = newColor;
			/*
			if (newColor == Colors.Transparent)
				colorLabel.colorSize.Visibility = Visibility.Collapsed;
			else
				colorLabel.colorSize.Visibility = Visibility.Visible;
			*/
		}

		/// <summary>
		/// Handles when the Text property is changed
		/// </summary>
		/// <param name="dependency"></param>
		/// <param name="e"></param>
		protected static void UpdateText(DependencyObject dependency, DependencyPropertyChangedEventArgs e)
		{
			ColorLabel colorLabel = (ColorLabel)dependency;
			colorLabel.textBlock.Text = (string)e.NewValue;
			colorLabel.textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity)); 
			colorLabel.colorSize.Width = colorLabel.textBlock.DesiredSize.Height * 1.5;
		}

		#endregion Event Handlers
	}
}