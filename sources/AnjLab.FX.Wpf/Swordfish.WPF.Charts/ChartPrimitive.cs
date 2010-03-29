// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish.WPF.Charts\ChartPrimitive.cs                          **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.Generic;

namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
	/// <summary>
	/// Summary description for ChartPrimitive.
	/// </summary>
	public class ChartPrimitive
	{
		// ********************************************************************
		// Private Fields
		// ********************************************************************
		#region Private Fields

		/// <summary>
		/// A list of points that define the shape of the primitive
		/// </summary>
		private readonly List<Point> points;

	    private readonly Dictionary<Point, object> pointsData;
		/// <summary>
		/// The minimum corner of the rectangle that holds all the points
		/// </summary>
		private Point minPoint;
		/// <summary>
		/// The maximum corner of the rectangle that holds all the points
		/// </summary>
		private Point maxPoint;
		/// <summary>
		/// The label for the primitive
		/// </summary>
		private string label;
		/// <summary>
		/// The last point added to the list
		/// </summary>
		private Point lastPoint;
		/// <summary>
		/// Flag indicating if this primitive is filled or not
		/// </summary>
		private bool filled=false;
		/// <summary>
		/// Flag indicating if this primitive is shown in the legend, and also
		/// if it's points are hit tested or not on the cursor
		/// </summary>
		private bool showInLegend=true;
		/// <summary>
		/// The line thickness
		/// </summary>
		private double lineThickness = 1;
		/// <summary>
		/// The paths for this primitive
		/// </summary>
		private readonly PathFigure paths;
		/// <summary>
		/// The color used to draw the primitive
		/// </summary>
		public Color color;
		/// <summary>
		/// Flag indicating whether to hit test the primitive or not
		/// </summary>
		private bool hitTest=true;
		/// <summary>
		/// Flag indicating if the line is dashed or not
		/// </summary>
		private bool dashed=false;

	    private double fillOpacity = 1.0;

	    #endregion Private Fields

		// ********************************************************************
		// Methods
		// ********************************************************************
		#region Methods

		/// <summary>
		/// Constructor. Initializes class fields.
		/// </summary>
		public ChartPrimitive()
		{
			// Initialize min and max points

			// Set the brush to red
			color = Colors.Red;

			// Set a default label
			label = "Unlabled";

			points = new List<Point>();
            pointsData = new Dictionary<Point, object>();

			minPoint = new Point(0.0f, 0.0f);
			maxPoint = new Point(0.0f, 0.0f);
			paths = new PathFigure();

		}

        /// <summary>
        /// Copy constructor. Deep copies the ChartPrimitive passed in.
        /// </summary>
        /// <param name="chartPrimitive"></param>
        protected ChartPrimitive(ChartPrimitive chartPrimitive) : this()
		{
			points.AddRange(chartPrimitive.Points);
            foreach (KeyValuePair<Point, object> dataObject in chartPrimitive.pointsData)
            {
                pointsData[dataObject.Key] = dataObject.Value;
            }
			minPoint = chartPrimitive.MinPoint;
			maxPoint = chartPrimitive.MaxPoint;
			color = chartPrimitive.Color;
			label = chartPrimitive.Label;
			lastPoint = chartPrimitive.lastPoint;
			filled = chartPrimitive.Filled;
			showInLegend = chartPrimitive.ShowInLegend;
			hitTest = chartPrimitive.HitTest;
			lineThickness = chartPrimitive.LineThickness;
			paths = chartPrimitive.paths.Clone();
		}

        public double FillOpacity
        {
            get { return fillOpacity; }
            set { fillOpacity = value; }
        }
        /// <summary>
        /// Does a deep clone of the current ChartPrimitive
        /// </summary>
        /// <returns></returns>
        public ChartPrimitive Clone()
		{
			return new ChartPrimitive(this);
		}

        public void AddPoint(double x, double y)
        {
            AddPoint(new Point(x, y), null);
        }

        public void AddPoint(double x, double y, object data)
		{
            AddPoint(new Point(x, y), data);
		}

        public void AddPoint(Point point, object data)
        {
            AddPoint(point, data, true);
        }

        public void AddPoint(Point point)
        {
            AddPoint(point, null, true);
        }

        public void AddPoint(Point point, bool updateMinMax)
        {
            AddPoint(point, null, updateMinMax);
        }

	    /// <summary>
		/// Adds a point to the primitive
		/// </summary>
        /// <param name="point"></param>
        /// <param name="data"></param>
        /// <param name="updateMinMax"></param>
        public void AddPoint(Point point, object data, bool updateMinMax)
		{
			// For normal lines, the points are added as is.
			if (points.Count>0)
			{
				// If this isn't the first line being added,
				// then test to see if the new line increases
				// the area convered by all the lines
                if (updateMinMax)
                {
                    UpdateMinMax(point);
                }

                paths.Segments.Add(new LineSegment(point, true));
            }
			else
			{
				// First line being drawn so set the min/max points
				// to the points of the line being added
				minPoint = point;
				maxPoint = point;

				paths.StartPoint = point;
			}
			lastPoint = point;
			points.Add(point);
            pointsData[point] = data;
        }

		/// <summary>
		/// Inserts a point into the primitive
		/// </summary>
        public void InsertPoint(Point point, object data, int index)
		{
            InsertPoint(point, data, index, true);
		}

        public void InsertPoint(Point point, int index)
		{
            InsertPoint(point, null, index, true);
		}

        public void InsertPoint(Point point, int index, bool updateMinMax)
        {
            InsertPoint(point, null, index, updateMinMax);
        }

        public void InsertPoint(Point point, object data, int index, bool updateMinMax)
		{
			if (points.Count > 0)
			{
				if (index > 0)
				{
                    paths.Segments.Insert(index - 1, new LineSegment(point, true));
				}
				else
				{
                    paths.Segments.Insert(0, new LineSegment(paths.StartPoint, true));
					paths.StartPoint = point;
				}
                if (updateMinMax)
                {
                    UpdateMinMax(point);
                }
                pointsData[point] = data;
			}
			else
			{
                AddPoint(point, data);
			}
		}

        public object GetPointData(Point point)
        {
            return pointsData.ContainsKey(point) ? pointsData[point] : null;
        }

	    /// <summary>
		/// Adds a bezier curve point where it gives a little plateau at the point
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void AddSmoothHorizontalBar(double x, double y)
		{
			AddSmoothHorizontalBar(new Point(x, y));
		}

		/// <summary>
		/// Adds a bezier curve point where it gives a little plateau at the point
		/// </summary>
		/// <param name="point"></param>
		public void AddSmoothHorizontalBar(Point point)
		{
			if (points.Count != 0)
			{
				double xDiff = (point.X - lastPoint.X) * .5;
				//double yDiff = (point.Y - lastPoint.Y) * .3;

				Point controlPoint1 = new Point(lastPoint.X + xDiff, lastPoint.Y);
				Point controlPoint2 = new Point(point.X - xDiff, point.Y);
				UpdateMinMax(point);
				lastPoint = point;
				paths.Segments.Add(new BezierSegment(controlPoint1, controlPoint2, point, true));
			}
			points.Add(point);
		}

		/// <summary>
		/// Updates the Minimum and Maximum point with the point passed in
		/// </summary>
		/// <param name="point"></param>
		protected void UpdateMinMax(Point point)
		{
			minPoint.X = Math.Min(minPoint.X, point.X);
			minPoint.Y = Math.Min(minPoint.Y, point.Y);
			maxPoint.X = Math.Max(maxPoint.X, point.X);
			maxPoint.Y = Math.Max(maxPoint.Y, point.Y);
		}

		#endregion Methods

		// ********************************************************************
		// Properties
		// ********************************************************************
		#region Properties

		/// <summary>
		/// Gets the list of points added to this primitive
		/// </summary>
		public List<Point> Points
		{
			get
			{
				return points;
			}
		}

		/// <summary>
		/// Gets the path geometry of this primitive
		/// </summary>
		public PathGeometry PathGeometry
		{
			get
			{
				PathGeometry geometry = new PathGeometry();
				geometry.Figures.Add(paths);
				return geometry;
			}
		}

		/// <summary>
		/// Gets the minium x,y values in the point collection
		/// </summary>
		public Point MinPoint
		{
			get
			{
				return minPoint;
			}
		}

		/// <summary>
		/// Gets the maximum x,y values in the point collection
		/// </summary>
		public Point MaxPoint
		{
			get
			{
				return maxPoint;
			}
		}

        /// <summary>
        /// Gets/Sets the color of the primitive
        /// </summary>
        public Color Color
		{
			get
			{
				return color;
			}
			set
			{
				color = value;
			}
		}

		/// <summary>
		/// Gets/Sets the line label
		/// </summary>
		public string Label
		{
			get
			{
				return label;
			}
			set
			{
				label = value;
			}
		}

		/// <summary>
		/// Gets/Sets if the shape is filled or not
		/// </summary>
		public bool Filled
		{
			get
			{
				return filled;
			}
			set
			{
				filled = value;
			}
		}

		/// <summary>
		/// Gets/Sets if the the line is dashed or not
		/// </summary>
		public bool Dashed
		{
			get
			{
				return dashed;
			}
			set
			{
				dashed = value;
			}
		}

        /// <summary>
        /// Gets/Sets if the chart should draw points for individual values or not
        /// </summary>
	    public bool ShowIndividualPoints { get; set; }

		/// <summary>
		/// Gets/Sets if the primitve should be shown in the plot legend.
		/// Note if this is true the points are tested for when the cursor
		/// is near them so that the cursor can show their value.
		/// </summary>
		public bool ShowInLegend
		{
			get
			{
				return showInLegend;
			}
			set
			{
				showInLegend = value;
			}
		}

		/// <summary>
		/// Gets/Sets if the line is to be hit tested or not
		/// </summary>
		public bool HitTest
		{
			get
			{
				return hitTest;
			}
			set
			{
				hitTest = value;
			}
		}


		/// <summary>
		/// Gets/Sets the line thickness
		/// </summary>
		public double LineThickness
		{
			get
			{
				return lineThickness;
			}
			set
			{
				lineThickness = value;
			}
		}

		#endregion Properties
	}
}
