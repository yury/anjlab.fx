// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish.WPF.Charts\AdornerCursorCoordinateDrawer.cs           **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

using System;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
	public class AdornerCursorCoordinateDrawer : Adorner
	{
		// ********************************************************************
		// Private Fields
		// ********************************************************************
		#region Private Fields
		/// <summary>
		/// Flag indicating if the coordinates are locked to the closest point or or not.
		/// </summary>
		private bool locked;
		/// <summary>
		/// The transformed coordinate of the lock point
		/// </summary>
		private Point lockPoint;
		/// <summary>
		/// The cursor position. Used for calculating the relative position of the text.
		/// </summary>
		private Point mousePoint;
		/// <summary>
		/// The transform of the element being adorned
		/// </summary>
		private readonly MatrixTransform elementTransform;

	    private readonly XYLineChart xyLineChart;

		#endregion Private Fields

		// ********************************************************************
		// Public Methods
		// ********************************************************************
		#region Public Methods

		/// <summary>
		/// Constructor. Initializes class fields.
		/// </summary>
        public AdornerCursorCoordinateDrawer(XYLineChart xyLineChart, UIElement adornedElement, MatrixTransform shapeTransform)
			: base(adornedElement)
		{
			elementTransform = shapeTransform;
			IsHitTestVisible = false;
		    this.xyLineChart = xyLineChart;
		}

		/// <summary>
		/// Draws a mouse cursor on the adorened element
		/// </summary>
		/// <param name="drawingContext"></param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			GeneralTransform inverse = elementTransform.Inverse;
			if (inverse == null)
				return;

			Brush blackBrush = new SolidColorBrush(Colors.Black);

			float radius = 15;
			if (locked)
			{
				// Draw the little circle around the lock point

				Point point = elementTransform.Transform(lockPoint);
				drawingContext.DrawEllipse(null, new Pen(blackBrush, 3), point, 2.5, 2.5);
				drawingContext.DrawEllipse(null, new Pen(new SolidColorBrush(Colors.White), 2), point, 2.5, 2.5);

				// Draw the big yellow circle

				Pen yellowPen = new Pen(new SolidColorBrush(Colors.Yellow), 2);
				Pen blackPen = new Pen(blackBrush, 3);
				drawingContext.DrawEllipse(null, blackPen, mousePoint, radius, radius);
				drawingContext.DrawEllipse(null, yellowPen, mousePoint, radius, radius);
			}
			else
			{
				// Draw the target symbol

				Pen blackPen = new Pen(blackBrush, .7);
				drawingContext.DrawEllipse(null, blackPen, mousePoint, radius, radius);
				drawingContext.DrawLine(blackPen, new Point(mousePoint.X - radius * 1.6, mousePoint.Y), new Point(mousePoint.X - 2, mousePoint.Y));
				drawingContext.DrawLine(blackPen, new Point(mousePoint.X + radius * 1.6, mousePoint.Y), new Point(mousePoint.X + 2, mousePoint.Y));
				drawingContext.DrawLine(blackPen, new Point(mousePoint.X, mousePoint.Y - radius * 1.6), new Point(mousePoint.X, mousePoint.Y - 2));
				drawingContext.DrawLine(blackPen, new Point(mousePoint.X, mousePoint.Y + radius * 1.6), new Point(mousePoint.X, mousePoint.Y + 2));
			}

			// Draw the coordinate text

			// Works out the number of decimal places required to show the difference between
			// 2 pixels. E.g if pixels are .1 apart then use 2 places etc
			Rect rect = inverse.TransformBounds(new Rect(0, 0, 1, 1));

			int xFigures = Math.Max(1, (int)(Math.Ceiling(-Math.Log10(rect.Width)) + .1));
			int yFigures = Math.Max(1, (int)(Math.Ceiling(-Math.Log10(rect.Height)) + .1));

			// Number of significant figures for the x coordinate
			string xFormat = "#0." + new string('#', xFigures);
			/// Number of significant figures for the y coordinate
			string yFormat = "#0." + new string('#', yFigures);

			Point coordinate = locked ? lockPoint : inverse.Transform(mousePoint);

		    string coordinateText;
            string xText = coordinate.X.ToString(xFormat);
            string yText = coordinate.Y.ToString(yFormat);
            if (xyLineChart.MinDate != DateTime.MaxValue)
            {
                DateTime dateTime = xyLineChart.MinDate.AddSeconds(coordinate.X);
                xText = ((xyLineChart.CoordinatDateFormat != null)
                                    ? dateTime.ToString(xyLineChart.CoordinatDateFormat)
                                    : dateTime.ToString());
            }
            coordinateText = string.Format(xyLineChart.CoordinatFormatString, xText, yText);

            drawingContext.PushTransform(new ScaleTransform(1, -1));
			FormattedText formattedText = new FormattedText(coordinateText, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Arial"), 10, blackBrush);
			Pen textBoxPen = new Pen(new SolidColorBrush(Color.FromArgb(127, 255, 255, 255)), 1);

			Rect textBoxRect = new Rect(new Point(mousePoint.X + radius * .7, -mousePoint.Y + radius * .7), new Size(formattedText.Width, formattedText.Height));
			double diff = textBoxRect.Right + 3 - ((FrameworkElement)AdornedElement).ActualWidth;

			if (diff > 0)
				textBoxRect.Location = new Point(textBoxRect.Left - diff, textBoxRect.Top);

			drawingContext.DrawRectangle(textBoxPen.Brush, textBoxPen, textBoxRect);
			drawingContext.DrawText(formattedText, textBoxRect.Location);
			drawingContext.Pop();
		}

		#endregion Public Methods
	
		// ********************************************************************
		// Properties
		// ********************************************************************
		#region Properties

		/// <summary>
		/// Gets/Sets if the coordinates are locked to the LockPoint or not
		/// </summary>
		public bool Locked
		{
			get
			{
				return locked;
			}
			set
			{
				if (locked != value)
				{
					locked = value;
					AdornerLayer parent = AdornerLayer.GetAdornerLayer(AdornedElement);
					parent.Update();
				}
			}
		}

		/// <summary>
		/// Gets/Sets the coordinate for the cursor to show when it is "Locked"
		/// </summary>
		public Point LockPoint
		{
			get
			{
				return lockPoint;
			}
			set
			{
				if (lockPoint != value)
				{
					lockPoint = value;
					AdornerLayer parent = AdornerLayer.GetAdornerLayer(AdornedElement);
					parent.Update();
				}
			}
		}

		/// <summary>
		/// Gets/Sets the current mouse position
		/// </summary>
		public Point MousePoint
		{
			get
			{
				return mousePoint;
			}
			set
			{
				if (mousePoint != value)
				{
					mousePoint = value;
                    AdornerLayer parent = AdornerLayer.GetAdornerLayer(AdornedElement);
                    parent.Update();
				}
			}
		}
		#endregion Properties

	}
}
