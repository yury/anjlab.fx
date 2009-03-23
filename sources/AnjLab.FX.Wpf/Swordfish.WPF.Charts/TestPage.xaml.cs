// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish.WPF.Charts\TestPage.xaml.cs                           **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

using System.Windows;
using System.Windows.Controls;


namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class TestPage : Grid
    {
        public TestPage()
        {
            InitializeComponent();
			ChartUtilities.AddTestLines(xyLineChart);
			xyLineChart.SubNotes = new string[] { "Right Mouse Button To Zoom, Left Mouse Button To Pan, Double-Click To Reset" };
			copyToClipboard.CopyToClipboardDelegate = this.CopyChartToClipboard;
		}

		public XYLineChart XYLineChart
		{
			get
			{
				return xyLineChart;
			}
		}

		/// <summary>
		/// Copies the chart to the clipboard
		/// </summary>
		/// <param name="element"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		protected void CopyChartToClipboard(FrameworkElement element, double width, double height)
		{
			ChartUtilities.CopyChartToClipboard(plotToCopy, xyLineChart, width, height);
		}

    }
}