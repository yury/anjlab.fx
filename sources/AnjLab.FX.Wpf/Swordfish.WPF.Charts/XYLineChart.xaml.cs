// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish.WPF.Charts\XYLineChart.cs                             **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
    public class PoinSelectedEventArgs : EventArgs
    {
        private readonly object item;

        public PoinSelectedEventArgs()
        {
        }

        public PoinSelectedEventArgs(object item)
        {
            this.item = item;
        }

        public object Item
        {
            get { return item; }
        }
    }

    public partial class XYLineChart : UserControl
    {
        

        #region Private Fields

        // user data

        private readonly AdornerCursorCoordinateDrawer adorner;
        private readonly PathGeometry chartClip;

        // Helper classes

        private readonly ClosestPointPicker closestPointPicker;

        // Appearance

        private readonly Color gridLineColor = Colors.Silver;
        private readonly Color gridLineLabelColor = Colors.Black;
        private readonly PanZoomCalculator panZoomCalculator;

        /// <summary>
        /// A list of lines to draw on the chart
        /// </summary>
        private readonly List<ChartPrimitive> primitiveList;

        // internal settings

        private readonly MatrixTransform shapeTransform;

        private AdornerLayer adornerLayer = null;

        private string axisDateFormat = null;
        private string coordinatDateFormat = null;
        private string coordinatFormatString = "{0},{1}";
        private DateTime maxDate = DateTime.MinValue;
        private Point maxPoint;
        private DateTime minDate = DateTime.MaxValue;
        private Point minPoint;
        private Point optimalGridLineSpacing;
        private bool yTextForDigitalView = true;

        #endregion Private Fields

        #region Public Methods

        private readonly Brush selectionSquareBrush = new SolidColorBrush(Colors.LightBlue);
        private readonly Pen selectionSquarePen = new Pen(Brushes.Gray, 2);
        private bool isMultiSelecting = false;
        private DrawingVisual selectionSquare;
        private Point selectionSquareTopLeft;

        /// <summary>
        /// Constructor. Initializes all the class fields.
        /// </summary>
        public XYLineChart()
        {
            // This assumes that you are navigating to this scene.
            // If you will normally instantiate it via code and display it
            // manually, you either have to call InitializeComponent by hand or
            // uncomment the following line.

            InitializeComponent();

            primitiveList = new List<ChartPrimitive>();

            // Set the Chart Geometry Clip region
            chartClip = new PathGeometry();
            chartClip.AddGeometry(
                new RectangleGeometry(new Rect(0, 0, clippedPlotCanvas.ActualWidth, clippedPlotCanvas.ActualHeight)));
            shapeTransform = new MatrixTransform();
            adorner = new AdornerCursorCoordinateDrawer(this, clippedPlotCanvas, shapeTransform);

            optimalGridLineSpacing = new Point(150, 75);

            panZoomCalculator =
                new PanZoomCalculator(new Rect(0, 0, clippedPlotCanvas.ActualWidth, clippedPlotCanvas.ActualHeight));
            panZoomCalculator.Window = new Rect(0, 0, clippedPlotCanvas.ActualWidth, clippedPlotCanvas.ActualHeight);
            panZoomCalculator.PanZoomChanged += panZoomCalculator_PanZoomChanged;

            closestPointPicker = new ClosestPointPicker(new Size(13, 13));
            closestPointPicker.ClosestPointChanged += closestPointPicker_ClosestPointChanged;

            Cursor = Cursors.None;

            // Set up all the message handlers for the clipped plot canvas
            AttachEventsToCanvas(this);
            clippedPlotCanvas.IsVisibleChanged += clippedPlotCanvas_IsVisibleChanged;
            clippedPlotCanvas.SizeChanged += clippedPlotCanvas_SizeChanged;
        }

        /// <summary>
        /// Attaches mouse handling events to the canvas passed in. The canvas passed in should be the top level canvas.
        /// </summary>
        /// <param name="eventCanvas"></param>
        protected void AttachEventsToCanvas(UIElement eventCanvas)
        {
            eventCanvas.LostMouseCapture += clippedPlotCanvas_LostMouseCapture;
            eventCanvas.MouseMove += clippedPlotCanvas_MouseMove;
            eventCanvas.MouseLeftButtonDown += clippedPlotCanvas_MouseLeftButtonDown;
            eventCanvas.MouseLeftButtonUp += clippedPlotCanvas_MouseLeftButtonUp;
            eventCanvas.MouseRightButtonDown += clippedPlotCanvas_MouseRightButtonDown;
            eventCanvas.MouseRightButtonUp += clippedPlotCanvas_MouseRightButtonUp;
            eventCanvas.MouseEnter += clippedPlotCanvas_MouseEnter;
            eventCanvas.MouseLeave += clippedPlotCanvas_MouseLeave;
        }

        /// <summary>
        /// Reset everything from the current collection of Chart Primitives
        /// </summary>
        public void RedrawPlotLines()
        {
            closestPointPicker.Points.Clear();
            legendBox.Children.Clear();
            foreach (ChartPrimitive primitive in primitiveList)
            {
                if (primitive.ShowInLegend)
                {
                    legendBox.Children.Insert(0, new ColorLabel(primitive.Label, primitive.Color));
                }
                if (primitive.HitTest)
                {
                    closestPointPicker.Points.AddRange(primitive.Points);
                }
            }
            InvalidateMeasure();
            SetChartTransform(clippedPlotCanvas.ActualWidth, clippedPlotCanvas.ActualHeight);
            RenderPlotLines(clippedPlotCanvas);
        }

        private void DrawSelectionSquare(Point point1, Point point2)
        {
            selectionSquarePen.DashStyle = DashStyles.Dash;
            selectionSquareBrush.Opacity = 0.3;
            using (DrawingContext dc = selectionSquare.RenderOpen())
            {
                var r = new Rect(point1, point2);

                dc.DrawRectangle(selectionSquareBrush, selectionSquarePen, r);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Resize the plot by changing the transform and drawing the grid lines
        /// </summary>
        protected void ResizePlot()
        {
            SetChartTransform(clippedPlotCanvas.ActualWidth, clippedPlotCanvas.ActualHeight);
            // Don't need to re-render the plot lines, just change the transform
            //RenderPlotLines(clippedPlotCanvas);

            // Still need to redraw the grid lines
            InvalidateMeasure();
            // Grid lines are now added in the measure override to stop flickering
            //DrawGridlinesAndLabels(textCanvas, new Size(textCanvas.ActualWidth, textCanvas.ActualHeight), minPoint, maxPoint);
        }

        /// <summary>
        /// Set the chart transform
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected void SetChartTransform(double width, double height)
        {
            Rect plotArea = ChartUtilities.GetPlotRectangle(primitiveList, 0.01f, 0.05f);

            minPoint = plotArea.Location;
            minPoint.Offset(-plotArea.Width*panZoomCalculator.Pan.X, plotArea.Height*panZoomCalculator.Pan.Y);
            minPoint.Offset(0.5*plotArea.Width*(1 - 1/panZoomCalculator.Zoom.X),
                            0.5*plotArea.Height*(1 - 1/panZoomCalculator.Zoom.Y));

            maxPoint = minPoint;
            maxPoint.Offset(plotArea.Width/panZoomCalculator.Zoom.X, plotArea.Height/panZoomCalculator.Zoom.Y);

            var plotScale = new Point();
            plotScale.X = (width/plotArea.Width)*panZoomCalculator.Zoom.X;
            plotScale.Y = (height/plotArea.Height)*panZoomCalculator.Zoom.Y;

            Matrix shapeMatrix = Matrix.Identity;
            shapeMatrix.Translate(-minPoint.X, -minPoint.Y);
            shapeMatrix.Scale(plotScale.X, plotScale.Y);
            shapeTransform.Matrix = shapeMatrix;
        }


        /// <summary>
        /// Render all the plot lines from the collection of Chart Primitives
        /// </summary>
        /// <param name="canvas"></param>
        protected void RenderPlotLines(Canvas canvas)
        {
            // Draw the Chart Plot Points

            // Fill in the background
            canvas.Children.Clear();

            foreach (ChartPrimitive primitive in primitiveList)
            {
                if (primitive.Points.Count > 0)
                {
                    var path = new Path();
                    var pathGeometry = new PathGeometry {Transform = shapeTransform};
                    
                    pathGeometry.AddGeometry(primitive.PathGeometry);
                    if (primitive.Filled)
                    {
                        path.Stroke = null;
                        if (primitive.Dashed)
                            path.Fill = ChartUtilities.CreateHatch50(primitive.Color, new Size(2, 2));
                        else
                        {
                            var brush = new SolidColorBrush(primitive.Color) {Opacity = primitive.FillOpacity};
                            path.Fill = brush;
                            path.Stroke = new SolidColorBrush(primitive.Color);
                            path.StrokeThickness = primitive.LineThickness;
                        }
                    }
                    else
                    {
                        path.Stroke = new SolidColorBrush(primitive.Color);
                        path.StrokeThickness = primitive.LineThickness;
                        path.Fill = null;
                        if (primitive.Dashed)
                            path.StrokeDashArray = new DoubleCollection(new double[] {2, 2});
                    }
                    path.Data = pathGeometry;
                    path.Clip = chartClip;
                    canvas.Children.Add(path);

                    if (primitive.ShowIndividualPoints)
                    {
                        path = new Path {Fill = new SolidColorBrush(primitive.Color) {Opacity = primitive.FillOpacity}};

                        var gm = new GeometryGroup {Transform = shapeTransform};
                        foreach (var point in primitive.Points)
                            gm.Children.Add(NewPointGeometry(point, shapeTransform.Matrix));

                        path.Data = gm;
                        path.Clip = chartClip;
                        canvas.Children.Add(path);
                    }
                }
            }
        }

        private static EllipseGeometry NewPointGeometry(Point point, Matrix matrix)
        {
            return new EllipseGeometry(point, 2/matrix.M11, 2/matrix.M22);
        }

        /// <summary>
        /// Add grid lines here
        /// </summary>
        /// <param name="constraint"></param>
        /// <returns></returns>
        protected override Size MeasureOverride(Size constraint)
        {
            DrawGridlinesAndLabels(new Size(textCanvas.ActualWidth, textCanvas.ActualHeight), minPoint, maxPoint);
            return base.MeasureOverride(constraint);
        }

        /// <summary>
        /// Draw all the gridlines and labels for the gridlines
        /// </summary>
        /// <param name="size"></param>
        /// <param name="minXY"></param>
        /// <param name="maxXY"></param>
        protected void DrawGridlinesAndLabels(Size size, Point minXY, Point maxXY)
        {
            // Clear the text canvas
            textCanvas.Children.Clear();

            // Create brush for writing text
            Brush axisBrush = new SolidColorBrush(gridLineColor);
            Brush axisScaleBrush = new SolidColorBrush(gridLineLabelColor);

            // Need to pick appropriate scale increment.
            // Go for a 2Exx, 5Exx, or 1Exx type scale
            double scaleX = 0.0;
            double scaleY = 0.0;

            // Work out all the limits

            if (maxXY.X != minXY.X)
                scaleX = size.Width/(maxXY.X - minXY.X);
            if (maxXY.Y != minXY.Y)
                scaleY = size.Height/(maxXY.Y - minXY.Y);

            double spacingX;
            int startXmult, endXmult;
            if (ScaleXHours.HasValue && scaleX != 0)
            {
                int diff = Math.Max(1, Convert.ToInt32(Math.Truncate((MaxDate - MinDate).TotalHours / ScaleXHours.Value)));
                spacingX = (maxXY.X - minXY.X) / diff;
                startXmult = 0;
                endXmult = diff;
            }
            else
            {
                var optimalSpacingX = optimalGridLineSpacing.X/scaleX;
                spacingX = ChartUtilities.Closest_1_2_5_Pow10(optimalSpacingX);
                startXmult = (int) Math.Ceiling(minXY.X/spacingX);
                endXmult = (int) Math.Floor(maxXY.X/spacingX);
            }

            var optimalSpacingY = optimalGridLineSpacing.Y / scaleY;
            var spacingY = ChartUtilities.Closest_1_2_5_Pow10(optimalSpacingY);
            var startYmult = (int) Math.Ceiling(minXY.Y/spacingY);
            var endYmult = (int) Math.Floor(maxXY.Y/spacingY);

            double maxXLabelHeight = 0;

            var pathFigure = new PathFigure();
            
            // Draw all the vertical gridlines
            double lastTextBorder = 0;
            for (int lineNo = startXmult; lineNo <= endXmult; ++lineNo)
            {
                double xValue = lineNo*spacingX;
                double xPos = (xValue - minXY.X)*scaleX;

                var startPoint = new Point(xPos, size.Height);
                var endPoint = new Point(xPos, 0);

                pathFigure.Segments.Add(new LineSegment(startPoint, false));
                pathFigure.Segments.Add(new LineSegment(endPoint, true));

                var text = new TextBlock
                               {
                                   TextAlignment = TextAlignment.Center,
                                   Foreground = axisScaleBrush,
                                   //FontSize = 8
                               };
                if (MinDate != DateTime.MaxValue)
                {
                    DateTime date = MinDate.AddSeconds(xValue);
                    text.Text = (axisDateFormat != null) ? date.ToString(axisDateFormat) : date.ToString();
                }
                else
                    text.Text = xValue.ToString();

                text.Measure(size);
                var textX = xPos - text.DesiredSize.Width * .5;
                if (lastTextBorder == 0 || lastTextBorder <= textX)
                {
                    if (textX + text.DesiredSize.Width > size.Width)
                        textX = size.Width - text.DesiredSize.Width;
                    text.SetValue(Canvas.LeftProperty, textX);
                    text.SetValue(Canvas.TopProperty, size.Height + 1);
                    textCanvas.Children.Add(text);
                    maxXLabelHeight = Math.Max(maxXLabelHeight, text.DesiredSize.Height);

                    lastTextBorder = textX + text.DesiredSize.Width;
                }
            }

            xGridlineLabels.Height = maxXLabelHeight + 2;

            // Set string format for vertical text
            double maxYLabelHeight = 0;

            // Draw all the horizontal gridlines

            for (int lineNo = startYmult; lineNo <= endYmult; ++lineNo)
            {
                double yValue = lineNo*spacingY;
                double yPos = (-yValue + minXY.Y)*scaleY + size.Height;

                var startPoint = new Point(0, yPos);
                var endPoint = new Point(size.Width, yPos);

                pathFigure.Segments.Add(new LineSegment(startPoint, false));
                pathFigure.Segments.Add(new LineSegment(endPoint, true));

                var text = new TextBlock();
                if (!yTextForDigitalView ||
                    Math.Abs(yValue - 1) < 0.00000001 ||
                    Math.Abs(yValue) < 0.00000001)
                {
                    text.Text = yValue.ToString();
                }
                else
                {
                    text.Text = " ";
                }
                text.LayoutTransform = new RotateTransform(-90);
                text.Measure(size);

                text.SetValue(Canvas.LeftProperty, -text.DesiredSize.Width - 1);
                text.SetValue(Canvas.TopProperty, yPos - text.DesiredSize.Height*.5);
                textCanvas.Children.Add(text);
                maxYLabelHeight = Math.Max(maxYLabelHeight, text.DesiredSize.Width);
            }
            yGridLineLabels.Height = maxYLabelHeight + 2;

            var path = new Path();
            path.Stroke = axisBrush;
            var pathGeometry = new PathGeometry(new PathFigure[] {pathFigure});

            pathGeometry.Transform = (Transform) textCanvas.RenderTransform.Inverse;
            path.Data = pathGeometry;

            textCanvas.Children.Add(path);
        }

        #endregion Protected Methods

        #region Properties

        /// <summary>
        /// Gets/Sets the title of the chart
        /// </summary>
        public string Title
        {
            get { return titleBox.Text; }
            set { titleBox.Text = value; }
        }

        /// <summary>
        /// Gets/Sets the X axis label
        /// </summary>
        public string XAxisLabel
        {
            get { return xAxisLabel.Text; }
            set { xAxisLabel.Text = value; }
        }

        /// <summary>
        /// Gets/Sets the Y axis label
        /// </summary>
        public string YAxisLabel
        {
            get { return yAxisLabel.Text; }
            set { yAxisLabel.Text = value; }
        }

        /// <summary>
        /// Gets/Sets the subnotes to be printed at the bottom of the chart
        /// </summary>
        public IEnumerable SubNotes
        {
            get { return subNotes.ItemsSource; }
            set { subNotes.ItemsSource = value; }
        }

        /// <summary>
        /// Gets the collection of chart primitives
        /// </summary>
        public List<ChartPrimitive> Primitives
        {
            get { return primitiveList; }
        }

        public DateTime MinDate
        {
            get { return minDate; }
            set { minDate = value; }
        }

        public DateTime MaxDate
        {
            get { return maxDate; }
            set { maxDate = value; }
        }

        public int? ScaleXHours { get; set; }

        public string AxisDateFormat
        {
            get { return axisDateFormat; }
            set { axisDateFormat = value; }
        }

        public string CoordinatDateFormat
        {
            get { return coordinatDateFormat; }
            set { coordinatDateFormat = value; }
        }

        public string CoordinatFormatString
        {
            get { return coordinatFormatString; }
            set { coordinatFormatString = value; }
        }

        public bool YTextForDigitalView
        {
            get { return yTextForDigitalView; }
            set { yTextForDigitalView = value; }
        }

        private XYLineChartLegendPosition legendPosition;
        public XYLineChartLegendPosition LegendPosition
        {
            get { return legendPosition; } 
            set
            {
                legendPosition = value;
                switch (value)
                {
                    case XYLineChartLegendPosition.Top:
                        legendBox.SetValue(Grid.ColumnProperty, 2);
                        legendBox.SetValue(Grid.RowProperty, 1);
                        legendBox.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);
                        legendBox.WrapMode = Orientation.Horizontal;
                        break;
                    case XYLineChartLegendPosition.Right:
                        legendBox.WrapMode = Orientation.Vertical;
                        legendBox.SetValue(Grid.ColumnProperty, 3);
                        legendBox.SetValue(Grid.RowProperty, 2);
                        legendBox.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
                        break;
                }
            }
        }

        #endregion Properties

        #region Misc Event Handlers

        private object selectedPointData;

        /// <summary>
        /// Handles when the closest point to the mouse cursor changes. Hides
        /// or shows the closest point, and changes the mouse cursor accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closestPointPicker_ClosestPointChanged(object sender, ClosestPointArgs e)
        {
            adorner.Locked = closestPointPicker.Locked;
            adorner.LockPoint = closestPointPicker.ClosestPoint;
            if (!closestPointPicker.Locked)
            {
                selectedPointData = null;
            }
            else
            {
                foreach (ChartPrimitive primitive in primitiveList)
                {
                    selectedPointData = primitive.GetPointData(closestPointPicker.ClosestPoint);
                    if (selectedPointData != null) break;
                }
            }
        }

        /// <summary>
        /// Handles when the pan or zoom changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void panZoomCalculator_PanZoomChanged(object sender, PanZoomArgs e)
        {
            ResizePlot();
        }

        #endregion Misc Event Handlers

        #region clippedPlotCanvas Event Handlers

        private Point? startPoint = null;

        /// <summary>
        /// Adds an adorner when the plot canvas is visible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible && adornerLayer == null)
            {
                adornerLayer = AdornerLayer.GetAdornerLayer(clippedPlotCanvas);
                adornerLayer.Add(adorner);
                adorner.Visibility = IsMouseOver ? Visibility.Visible : Visibility.Hidden;
            }
            else if (adornerLayer != null)
            {
                adornerLayer.Remove(adorner);
                adornerLayer = null;
            }
        }

        /// <summary>
        /// Handles when the plot size changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            chartClip.Clear();
            chartClip.AddGeometry(
                new RectangleGeometry(new Rect(0, 0, clippedPlotCanvas.ActualWidth, clippedPlotCanvas.ActualHeight)));
            panZoomCalculator.Window = new Rect(0, 0, clippedPlotCanvas.ActualWidth, clippedPlotCanvas.ActualHeight);
            ResizePlot();
        }

        /// <summary>
        /// Handles when the plot loses focus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_LostMouseCapture(object sender, MouseEventArgs e)
        {
            panZoomCalculator.StopPanning();
            panZoomCalculator.StopZooming();
            Cursor = Cursors.None;
        }

        /// <summary>
        /// Handles when the mouse moves over the plot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(clippedPlotCanvas);

            if (!panZoomCalculator.IsPanning && !panZoomCalculator.IsZooming)
            {
                adorner.MousePoint = mousePos;
                closestPointPicker.MouseMoved(mousePos, shapeTransform.Inverse);
            }
            else
            {
                panZoomCalculator.MouseMoved(clippedPlotCanvas.RenderTransform.Inverse.Transform(mousePos));
            }
            if (isMultiSelecting)
            {
                Debug.WriteLine("Drawing");
                Point pointDragged = e.GetPosition(clippedPlotCanvas);
                DrawSelectionSquare(selectionSquareTopLeft, pointDragged);
            }
        }

        /// <summary>
        /// Handles when the user clicks on the plot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2)
            {
                if (((UIElement) sender).CaptureMouse())
                {
                    Cursor = Cursors.ScrollAll;
                    adorner.Visibility = Visibility.Hidden;
                    panZoomCalculator.StartPan(
                        clippedPlotCanvas.RenderTransform.Inverse.Transform(e.GetPosition(clippedPlotCanvas)));
                }
            }
        }

        /// <summary>
        /// Handles when the user releases the mouse button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            panZoomCalculator.StopPanning();
            if (!panZoomCalculator.IsZooming)
            {
                Mouse.Capture(null);
                Cursor = Cursors.None;
                if (IsMouseOver)
                {
                    adorner.Visibility = Visibility.Visible;
                }
            }
        }

        public event EventHandler<PoinSelectedEventArgs> PointSelected;


        /// <summary>
        ///  Handles when the user clicks on the plot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2)
            {
                if (((UIElement) sender).CaptureMouse())
                {
                    Cursor = Cursors.ScrollAll;
                    adorner.Visibility = Visibility.Visible;
                    startPoint = e.GetPosition(clippedPlotCanvas);
                        //clippedPlotCanvas.RenderTransform.Inverse.Transform(e.GetPosition(clippedPlotCanvas));
                    //panZoomCalculator.StartZoom(clippedPlotCanvas.RenderTransform.Inverse.Transform(e.GetPosition(clippedPlotCanvas)));
                    selectionSquare = new DrawingVisual();
                    clippedPlotCanvas.AddVisual(selectionSquare);
                    selectionSquareTopLeft = e.GetPosition(clippedPlotCanvas);
                    isMultiSelecting = true;
                    if (selectedPointData != null && PointSelected != null)
                    {
                        PointSelected(sender, new PoinSelectedEventArgs(selectedPointData));
                    }
                }
            }
            else
            {
                ResetZoom();
            }
        }

        public void ResetZoom()
        {
            Mouse.Capture(null);
            panZoomCalculator.Reset();
        }

        /// <summary>
        /// Handles when the user releases the mouse
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (startPoint != null)
            {
                Point endPoint = e.GetPosition(clippedPlotCanvas);
                    //clippedPlotCanvas.RenderTransform.Inverse.Transform(e.GetPosition(clippedPlotCanvas));
                var centerPoint = new Point((startPoint.Value.X + endPoint.X)/2f, (startPoint.Value.Y + endPoint.Y)/2f);
                centerPoint = clippedPlotCanvas.RenderTransform.Inverse.Transform(centerPoint);
                var rect = new Rect(startPoint.Value, endPoint);
                double zoomX = clippedPlotCanvas.ActualWidth/rect.Width;
                double zoomY = clippedPlotCanvas.ActualHeight/rect.Height;
                if (!Double.IsInfinity(zoomY) && !Double.IsInfinity(zoomX))
                {
                    if (zoomX < 1) zoomX = 1;
                    if (zoomY < 1) zoomY = 1;
                    panZoomCalculator.StartZoom(centerPoint);
                    double panX = (clippedPlotCanvas.ActualWidth*0.5 - centerPoint.X)/clippedPlotCanvas.ActualWidth/
                                  panZoomCalculator.Zoom.X;
                    double panY = (-centerPoint.Y - clippedPlotCanvas.ActualHeight*0.5)/clippedPlotCanvas.ActualHeight/
                                  panZoomCalculator.Zoom.Y;
                    var panPoint = new Point(panZoomCalculator.Pan.X + panX, panZoomCalculator.Pan.Y + panY);
                    var zoomPoint = new Point(panZoomCalculator.Zoom.X*zoomX, panZoomCalculator.Zoom.Y*zoomY);
                    panZoomCalculator.Zoom = zoomPoint;
                    panZoomCalculator.Pan = panPoint;
                    panZoomCalculator.StopZooming();
                }
                startPoint = null;
            }
            if (!panZoomCalculator.IsPanning)
            {
                Mouse.Capture(null);
                Cursor = Cursors.None;
            }
            else
            {
                adorner.Visibility = Visibility.Hidden;
                Cursor = Cursors.ScrollAll;
            }
            if (isMultiSelecting)
            {
                isMultiSelecting = false;
                clippedPlotCanvas.RemoveVisual(selectionSquare);
                clippedPlotCanvas.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Handles when the mouse leaves the plot. Removes the nearest point indicator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            adorner.Locked = false;
            adorner.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Handles when the mouse enters the plot. Puts back the nearest point indicator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clippedPlotCanvas_MouseEnter(object sender, MouseEventArgs e)
        {
            adorner.Visibility = Visibility.Visible;
            adorner.Locked = closestPointPicker.Locked;
        }

        #endregion clippedPlotCanvas Event Handlers

        // ********************************************************************
        // Private Fields
        // ********************************************************************

        // ********************************************************************
        // Public Methods
        // ********************************************************************

        // ********************************************************************
        // Protected Methods
        // ********************************************************************

        // ********************************************************************
        // Properties
        // ********************************************************************

        // ********************************************************************
        // Misc Event Handlers
        // ********************************************************************

        // ********************************************************************
        // clippedPlotCanvas Event Handlers
        // ********************************************************************
    }
}