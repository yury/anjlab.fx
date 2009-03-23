// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish.WPF.Charts\ChartUtilities.cs                          **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Text;
using System.IO;
using System.Windows.Media.Imaging;
using Color=System.Windows.Media.Color;
using Point=System.Windows.Point;
using Size=System.Windows.Size;

namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
	/// <summary>
	/// Class that contains various utilities for drawing charts
	/// </summary>
	public static class ChartUtilities
	{
		// ********************************************************************
		// Public Methods
		// ********************************************************************
		#region Public Methods

		/// <summary>
		/// Copies the plotToCopy as a bitmap to the clipboard, and copies the
		/// chartControl to the clipboard as tab separated values.
		/// </summary>
		/// <param name="plotToCopy"></param>
		/// <param name="chartControl"></param>
		/// <param name="width">Width of the bitmap to be created</param>
		/// <param name="height">Height of the bitmap to be created</param>
		public static void CopyChartToClipboard(FrameworkElement plotToCopy, XYLineChart chartControl, double width, double height)
		{
            Bitmap bitmap = CopyFrameworkElementToBitmap(plotToCopy, width, height);
			string text = ConvertChartToSpreadsheetText(chartControl, '\t');
			MemoryStream csv = new MemoryStream(Encoding.UTF8.GetBytes(ConvertChartToSpreadsheetText(chartControl, ',')));
			DataObject dataObject = new DataObject();
			dataObject.SetData(DataFormats.Bitmap, bitmap);
			dataObject.SetData(DataFormats.Text, text);
			dataObject.SetData(DataFormats.CommaSeparatedValue, csv);
			Clipboard.SetDataObject(dataObject);
		}

		/// <summary>
		/// Converts a chart to tab separated values
		/// </summary>
		/// <param name="xyLineChart"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		public static string ConvertChartToSpreadsheetText(XYLineChart xyLineChart, char token)
		{
			int maxPrimitiveLength = 0;
			foreach (ChartPrimitive primitive in xyLineChart.Primitives)
			{
				maxPrimitiveLength = Math.Max(maxPrimitiveLength, primitive.Points.Count);
			}
			string[] grid = new string[maxPrimitiveLength + 1];
			foreach (ChartPrimitive primitive in xyLineChart.Primitives)
			{
				if (primitive.ShowInLegend)
				{
					int row = 0;
					grid[row] += primitive.Label + " X" + token + primitive.Label + " Y"+token;
					foreach (Point point in primitive.Points)
					{
						++row;
						grid[row] += point.X.ToString() + token + point.Y + token;
					}
					++row;
					while (row < grid.Length)
					{
						grid[row] += token.ToString()+token;
						++row;
					}
				}
			}

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(xyLineChart.Title);
			foreach (string line in grid)
			{
				sb.AppendLine(line.Substring(0, line.Length - 1));
			}

			return sb.ToString();
		}

		/// <summary>
		/// Calculates the as near to the input as possible, a power of 10 times 1,2, or 5 
		/// </summary>
		/// <param name="optimalValue"> The value to get closest to</param>
		/// <returns>The nearest value to the input value</returns>
		public static double Closest_1_2_5_Pow10(double optimalValue)
		{
			double[] numbersList = { 1.0, 2.0, 5.0 };
			return ClosestValueInListTimesBaseToInteger(optimalValue, numbersList, 10.0);
		}

		/// <summary>
		/// Calculates the closest possible value to the optimalValue passed
		/// in, that can be obtained by multiplying one of the numbers in the
		/// list by the baseValue to the power of any integer.
		/// </summary>
		/// <param name="optimalValue">The number to get closest to</param>
		/// <param name="numbers">List of numbers to mulitply by</param>
		/// <param name="baseValue">The base value</param>
		/// <returns></returns>
		public static double ClosestValueInListTimesBaseToInteger(double optimalValue, double[] numbers, double baseValue)
		{
            double multiplier = Math.Pow(baseValue, Math.Floor(Math.Log(optimalValue) / Math.Log(baseValue)));
			double minimumDifference = baseValue * baseValue * multiplier;
			double closestValue = 0.0;
			double minimumNumber = baseValue * baseValue;

			foreach (double number in numbers)
			{
				double difference = Math.Abs(optimalValue - number * multiplier);
				if (difference < minimumDifference)
				{
					minimumDifference = difference;
					closestValue = number * multiplier;
				}
				if (number < minimumNumber)
				{
					minimumNumber = number;
				}
			}

			if (Math.Abs(optimalValue - minimumNumber * baseValue * multiplier) < Math.Abs(optimalValue - closestValue))
				closestValue = minimumNumber * baseValue * multiplier;

			return closestValue;
		}

		/// <summary>
		/// Gets the plot rectangle that is required to hold all the
		/// lines in the primitive list
		/// </summary>
		/// <param name="primitiveList"></param>
		/// <returns></returns>
		public static Rect GetPlotRectangle(List<ChartPrimitive> primitiveList)
		{
			return GetPlotRectangle(primitiveList,0,0);
		}

		/// <summary>
		/// Gets a nominally oversize rectangle that the plot will be drawn into
		/// </summary>
		/// <param name="primitiveList"></param>
        /// <param name="x_oversize"></param>
        /// <param name="y_oversize"></param>
		/// <returns></returns>
        public static Rect GetPlotRectangle(List<ChartPrimitive> primitiveList, double x_oversize, double y_oversize)
		{
			// Get the extent of the plot region by going through
			// all the lines, and finding the min and max points
			bool firstPass = true;
			Vector minPoint = new Vector(0, 0);
			Vector maxPoint = new Vector(0, 0);
			foreach (ChartPrimitive primitive in primitiveList)
			{
				if (!firstPass)
				{
                    minPoint.X = Math.Min(primitive.MinPoint.X, minPoint.X);
                    minPoint.Y = Math.Min(primitive.MinPoint.Y, minPoint.Y);
                    maxPoint.X = Math.Max(primitive.MaxPoint.X, maxPoint.X);
                    maxPoint.Y = Math.Max(primitive.MaxPoint.Y, maxPoint.Y);
				}
				else
				{
					minPoint.X = primitive.MinPoint.X;
					maxPoint.X = primitive.MaxPoint.X;
					minPoint.Y = primitive.MinPoint.Y;
					maxPoint.Y = primitive.MaxPoint.Y;
					firstPass = false;
				}
			}

			// Make sure that the plot size is greater than zero
			if (maxPoint.Y == minPoint.Y)
			{
				if (maxPoint.Y!=0)
				{
					maxPoint.Y *= 1.05;
					minPoint.Y *= 0.95;
				}
				else
				{
					maxPoint.Y = 1;
					minPoint.Y = 0;
				}
			}

			if (maxPoint.X == minPoint.X)
			{
				if (maxPoint.X!=0.0)
				{
					maxPoint.X *= 1.05;
					minPoint.X *= 0.95;
				}
				else
				{
					maxPoint.X = 1;
					minPoint.X = 0;
				}
			}

			// Add a bit of a border around the plot
			maxPoint.X = maxPoint.X + (maxPoint.X - minPoint.X)*x_oversize*.5;
			maxPoint.Y = maxPoint.Y + (maxPoint.Y - minPoint.Y)*y_oversize*.5;
			minPoint.X = minPoint.X - (maxPoint.X - minPoint.X)*x_oversize*.5;
			minPoint.Y = minPoint.Y - (maxPoint.Y - minPoint.Y)*y_oversize*.5;

			return new Rect(minPoint.X,minPoint.Y,maxPoint.X - minPoint.X,maxPoint.Y - minPoint.Y);
		}//GetPlotRectangle

		/// <summary>
		/// Converts a ChartLine object to a ChartPolygon object that has
		/// one edge along the bottom Horizontal base line in the plot.
		/// </summary>
		/// <param name="chartLine"></param>
		/// <returns></returns>
		public static ChartPrimitive ChartLineToBaseLinedPolygon(ChartPrimitive chartLine)
		{
			ChartPrimitive chartPolygon = chartLine.Clone();

			Point firstPoint = chartPolygon.Points[0];
			firstPoint.Y = 0;
			Point lastPoint = chartPolygon.Points[chartPolygon.Points.Count - 1];
			lastPoint.Y = 0;

			chartPolygon.InsertPoint(firstPoint, null, 0);
			chartPolygon.AddPoint(lastPoint);
			chartPolygon.Filled = true;

			return chartPolygon;
		}

		/// <summary>
		/// Takes two lines and creates a polyon between them
		/// </summary>
		/// <param name="baseLine"></param>
		/// <param name="topLine"></param>
		/// <returns></returns>
		public static ChartPrimitive LineDiffToPolygon(ChartPrimitive baseLine, ChartPrimitive topLine)
		{
			ChartPrimitive polygon = new ChartPrimitive();
			List<Point> baseLinePoints = baseLine.Points;
			List<Point> topLinePoints = topLine.Points;

			for (int pointNo = baseLinePoints.Count - 1; pointNo >= 0; --pointNo)
			{
				polygon.AddPoint(baseLinePoints[pointNo]);
			}
			for (int pointNo = 0; pointNo < topLinePoints.Count; ++pointNo)
			{
				polygon.AddPoint(topLinePoints[pointNo]);
			}

			polygon.Filled = true;

			return polygon;
		}

		/// <summary>
		/// Adds a set of lines to the chart for test purposes
		/// </summary>
		/// <param name="xyLineChart"></param>
		public static void AddTestLines(XYLineChart xyLineChart)
		{
			// Add test Lines to demonstrate the control

			xyLineChart.Primitives.Clear();

			double limit = 5;
			double increment = 1;

			// Create 3 normal lines
			ChartPrimitive[] lines = new ChartPrimitive[3];

            //for (int lineNo = 0; lineNo < 3; ++lineNo)
            //{
            //    ChartPrimitive line = new ChartPrimitive();

            //    // Label the lines
            //    line.Filled = true;
            //    line.Dashed = false;
            //    line.ShowInLegend = false;
            //    line.AddPoint(0, 0);

            //    // Draw 3 sine curves
            //    for (double x = 0; x < limit + increment*.5; x += increment)
            //    {
            //        line.AddPoint(x, Math.Cos(x * Math.PI - lineNo * Math.PI / 1.5));
            //    }
            //    line.AddPoint(limit, 0);

            //    // Add the lines to the chart
            //    xyLineChart.Primitives.Add(line);
            //    lines[lineNo] = line;
            //}

            //// Set the line colors to Red, Green, and Blue
            //lines[0].Color = Color.FromArgb(90,255,0,0);
            //lines[1].Color = Color.FromArgb(90, 0, 180, 0);
            //lines[2].Color = Color.FromArgb(90, 0, 0, 255);

			for (int lineNo = 0; lineNo < 3; ++lineNo)
			{
				ChartPrimitive line = new ChartPrimitive();

				// Label the lines
				line.Label = "Test Line " + (lineNo + 1);
				//line.ShowInLegend = true;
				//line.HitTest = true;

				line.LineThickness = 1.5;
				// Draw 3 sine curves
                DateTime date = DateTime.Now;
			    int i = 0;
				for (double x = 0; x < limit + increment*.5; x += increment)
				{
                    line.AddPoint(date.Add(new TimeSpan(0, i++, 0)).Ticks, -2 * Math.Cos(x * Math.PI - lineNo * Math.PI / 1.5));
				}

				// Add the lines to the chart
				xyLineChart.Primitives.Add(line);
				lines[lineNo] = line;
			}
			// Set the line colors to Red, Green, and Blue
			lines[0].Color = Colors.Red;
			lines[1].Color = Colors.Green;
			lines[2].Color = Colors.Blue;

			xyLineChart.Title = "Test Chart Title";
			xyLineChart.XAxisLabel = "Test Chart X Axis";
			xyLineChart.YAxisLabel = "Test Chart Y Axis";

			xyLineChart.RedrawPlotLines();
		}

		/// <summary>
		/// Creates a 50% hatch patter for filling a polygon
		/// </summary>
		/// <param name="color"></param>
		/// <param name="blockSize"></param>
		/// <returns></returns>
		public static DrawingBrush CreateHatch50(Color color, Size blockSize)
		{
			GeometryGroup group = new GeometryGroup();
			RectangleGeometry rectangle1 = new RectangleGeometry(new Rect(new Point(0,0),blockSize));
			RectangleGeometry rectangle2 = new RectangleGeometry(new Rect((Point)blockSize,blockSize));
			group.Children.Add(rectangle1);
			group.Children.Add(rectangle2);

			GeometryDrawing drawing = new GeometryDrawing(new SolidColorBrush(color), null, group);

			DrawingBrush brush = new DrawingBrush(drawing);
			brush.TileMode = TileMode.Tile;
			brush.ViewportUnits = BrushMappingMode.Absolute;
			brush.Viewport = new Rect(0,0,blockSize.Width*2, blockSize.Height*2);
			return brush;
		}

		/// <summary>
		/// Copies a Framework Element to the clipboard as a bitmap
		/// </summary>
		/// <param name="copyTarget">The Framework Element to be copied</param>
		/// <param name="width">The width of the bitmap</param>
		/// <param name="height">The height of the bitmap</param>
		public static void CopyFrameworkElementToClipboard(FrameworkElement copyTarget, double width, double height)
		{
			if (copyTarget == null)
				return;

			Clipboard.SetDataObject(CopyFrameworkElementToBitmap(copyTarget, width, height));
		}

		/// <summary>
		/// Copies a Framework Element to a bitmap
		/// </summary>
		/// <param name="copyTarget">The Framework Element to be copied</param>
		/// <param name="width">The width of the bitmap</param>
		/// <param name="height">The height of the bitmap</param>
        public static Bitmap CopyFrameworkElementToBitmap(FrameworkElement copyTarget, double width, double height)
		{
			if (copyTarget == null)
                return new Bitmap((int)width, (int)height);

            Bitmap bitmap;
			// Convert from a WPF Bitmap Source to a Win32 Bitamp
			using (MemoryStream outStream = new MemoryStream())
			{
			    CopyFrameworkElementToStream(copyTarget, width, height, new BmpBitmapEncoder(), outStream);
                bitmap = new Bitmap(outStream);
			}

			return bitmap;
		}

		/// <summary>
		/// Copies a framework element to a bitmap stored in a memory stream
		/// </summary>
		/// <param name="copyTarget"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
        /// <param name="enc"></param>
        /// <param name="str"></param>
		/// <returns></returns>
		public static void CopyFrameworkElementToStream(FrameworkElement copyTarget, double width, double height, BitmapEncoder enc, Stream str)
		{
			// Store the Frameworks current layout transform, as this will be restored later
			Transform storedTransform = copyTarget.LayoutTransform;

			// Set the layout transform to unity to get the nominal width and height
			copyTarget.LayoutTransform = new ScaleTransform(1, 1);
			copyTarget.UpdateLayout();

			double baseHeight = copyTarget.ActualHeight;
			double baseWidth = copyTarget.ActualWidth;

			// Now scale the layout to fit the bitmap
			copyTarget.LayoutTransform =
				new ScaleTransform(baseWidth / width, baseHeight / height);
			copyTarget.UpdateLayout();

			// Render to a Bitmap Source, note that the DPI is changed for the 
			// render target as a way of scaling the FrameworkElement
			RenderTargetBitmap rtb = new RenderTargetBitmap(
				(int)width,
				(int)height,
				96d * width / baseWidth,
				96d * height / baseHeight,
				PixelFormats.Default);

			rtb.Render(copyTarget);

			// Convert from a WPF Bitmap Source to a Win32 Bitamp
			enc.Frames.Add(BitmapFrame.Create(rtb));
			enc.Save(str);
			// Restore the Framework Element to it's previous state
			copyTarget.LayoutTransform = storedTransform;
			copyTarget.UpdateLayout();
		}

		#endregion Public Methods

	}//ChartUtilities
}//Swordfish.Charts
