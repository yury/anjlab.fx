// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish.WPF.Charts\LineAndPolygon.cs                          **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
	/// <summary>
	/// Holds a line and a polygon pair
	/// </summary>
	public class LineAndPolygon
	{
		public ChartPrimitive Line;
		public ChartPrimitive Polygon;

		public LineAndPolygon()
		{
		}

		public LineAndPolygon(ChartPrimitive line, ChartPrimitive polygon)
		{
			Line = line;
			Polygon = polygon;
		}
	}
}
