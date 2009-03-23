// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish.WPF.Charts\CopyToClipboard.xaml.cs                    **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
	/// <summary>
	/// Interaction logic for UIElementToClipboard.xaml
	/// </summary>

	public partial class CopyToClipboard : UserControl
	{
		public delegate void CopyToClipboardDelegateType(FrameworkElement target, double width, double height);

		public CopyToClipboardDelegateType CopyToClipboardDelegate;

		public CopyToClipboard()
		{
			InitializeComponent();
			copyOptions.Visibility = Visibility.Collapsed;
			MouseEnter += UIElementToClipboard_MouseEnter;
			MouseLeave += UIElementToClipboard_MouseLeave;
			CopyToClipboardDelegate = ChartUtilities.CopyFrameworkElementToClipboard;
		}

		// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty CopyTargetProperty =
			DependencyProperty.Register("CopyTarget", typeof(FrameworkElement), typeof(CopyToClipboard), new UIPropertyMetadata(null));

		public FrameworkElement CopyTarget
		{
			get
			{
				return (FrameworkElement)GetValue(CopyTargetProperty);
			}
			set
			{
				SetValue(CopyTargetProperty, value);
			}
		}

		void UIElementToClipboard_MouseLeave(object sender, MouseEventArgs e)
		{
			copyOptions.Visibility = Visibility.Collapsed;
			Margin = new Thickness(0,0,0,0);
		}

		void UIElementToClipboard_MouseEnter(object sender, MouseEventArgs e)
		{
			copyOptions.Visibility = Visibility.Visible;
			Margin = new Thickness(0,0,0,8);
		}

		void bCopy640x480_Click(object sender, RoutedEventArgs e)
		{
			CopyToClipboardDelegate(CopyTarget, 640, 480);
		}
		void bCopy800x600_Click(object sender, RoutedEventArgs e)
		{
			CopyToClipboardDelegate(CopyTarget, 800, 600);
		}
		void bCopy1024x768_Click(object sender, RoutedEventArgs e)
		{
			CopyToClipboardDelegate(CopyTarget, 1024, 768);
		}
		void bCopy1280x1024_Click(object sender, RoutedEventArgs e)
		{
			CopyToClipboardDelegate(CopyTarget, 1280, 1024);
		}
		void bCopyCustom_Click(object sender, RoutedEventArgs e)
		{
			double width;
			double height;
			Double.TryParse(tbWidth.Text, out width);
			Double.TryParse(tbHeight.Text, out height);
			CopyToClipboardDelegate(CopyTarget, width, height);
		}
	}
}