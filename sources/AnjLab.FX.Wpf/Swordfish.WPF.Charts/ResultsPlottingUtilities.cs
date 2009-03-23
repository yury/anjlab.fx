// ****************************************************************************
// Copyright Swordfish Computing Australia 2006                              **
// http://www.swordfish.com.au/                                              **
//                                                                           **
// Filename: Swordfish.WPF.Charts\ResultsPlottingUtilities.cs                **
// Authored by: John Stewien of Swordfish Computing                          **
// Date: April 2006                                                          **
//                                                                           **
// - Change Log -                                                            **
//*****************************************************************************

using System;
using System.Windows.Media;
using System.Windows;

namespace AnjLab.FX.Wpf.Swordfish.WPF.Charts
{
	public static class ResultsPlottingUtilities
	{
		/// <summary>
		/// Constant that the alpha channel is multiplied by when creating
		/// polygons
		/// </summary>
		public static float alpha = 0.125f;

		/// <summary>
		/// Converts movements to a graphical line and polygon
		/// </summary>
		/// <param name="results"></param>
		/// <param name="color"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		public static LineAndPolygon ConvertResultsToMovementLineAndPolygon(float[] results, Color color, string label)
		{
			// Create the Line
			ChartPrimitive chartLine = new ChartPrimitive();
			chartLine.Color = color;
			chartLine.Label = label;
			chartLine.ShowInLegend = true;
			chartLine.HitTest = true;

			for (int monthNo = 0; monthNo < results.Length; ++monthNo)
			{
				chartLine.AddSmoothHorizontalBar(new Point(monthNo + .5f, results[monthNo]));
			}

			// Create the polygon
			ChartPrimitive polygon = ChartUtilities.ChartLineToBaseLinedPolygon(chartLine);
			color.A = (byte)(alpha * color.A);
			polygon.Color = color;
			polygon.ShowInLegend = false;
			polygon.HitTest = false;

			return new LineAndPolygon(chartLine, polygon);
		}

		/// <summary>
		/// Gets ChartLines and ChartPolygons for the population line, and
		/// the target line.
		/// </summary>
		public static LineAndPolygon ConvertResultsToTargetLineAndPolygon(ChartPrimitive populationLine, float[] results, Color color, string label)
		{

			// Calculate Target Primitives
			ChartPrimitive targetLine = new ChartPrimitive();
			targetLine.Color = color;
			targetLine.Dashed = true;
			targetLine.Label = label + " Target";
			targetLine.ShowInLegend = false;
			targetLine.HitTest = true;

			if (populationLine.Points.Count == results.Length)
			{
				for (int monthNo = 0; monthNo < results.Length; monthNo += 2)
				{
					targetLine.AddPoint(new Point(monthNo * .5f, results[monthNo]));
					targetLine.AddPoint(new Point(monthNo * .5f + 1f, results[monthNo + 1]));
				}
			}
			else
			{

				for (int monthNo = 0; monthNo < results.Length; ++monthNo)
				{
					targetLine.AddPoint(new Point(monthNo, results[monthNo]));
					targetLine.AddPoint(new Point(monthNo + 1f, results[monthNo]));
				}
			}

			ChartPrimitive targetPolygon = ChartUtilities.LineDiffToPolygon(populationLine, targetLine);
			color.A = (byte)(alpha * color.A);
			targetPolygon.Color = color;
			targetPolygon.Dashed = true;
			targetPolygon.ShowInLegend = false;
			targetLine.HitTest = false;

			return new LineAndPolygon(targetLine, targetPolygon);
		}

		/// <summary>
		/// Converts population level to a line and polygon
		/// </summary>
		/// <param name="results"></param>
		/// <param name="color"></param>
		/// <param name="label"></param>
		/// <returns></returns>
		public static LineAndPolygon ConvertResultsToPopulationLineAndPolygon(float[] results, Color color, string label)
		{
			ChartPrimitive populationLine = new ChartPrimitive();
			populationLine.Color = color;
			populationLine.Label = label;
			populationLine.ShowInLegend = true;
			populationLine.HitTest = true;

			for (int monthNo = 0; monthNo < results.Length; monthNo += 2)
			{
				populationLine.AddPoint(new Point(monthNo * .5f, results[monthNo]));
				populationLine.AddPoint(new Point(monthNo * .5f + 1f, results[monthNo + 1]));
			}

			ChartPrimitive populationPolygon = ChartUtilities.ChartLineToBaseLinedPolygon(populationLine);
			color.A = (byte)(alpha * color.A);
			populationPolygon.Color = color;
			populationPolygon.ShowInLegend = false;
			populationPolygon.HitTest = false;

			return new LineAndPolygon(populationLine, populationPolygon);
		}

		/// <summary>
		/// Adds the second array of floats to the first.
		/// </summary>
		/// <param name="values1"></param>
		/// <param name="values2"></param>
		public static void AddSecondArrayToFirst(float[] values1, float[] values2)
		{
			if (values1.Length != values2.Length)
				throw (new ApplicationException("Array Lengths need to be equal"));

			for (int i = 0; i < values1.Length; ++i)
			{
				values1[i] += values2[i];
			}
		}

		/// <summary>
		/// Accumulates the values along the array
		/// </summary>
		/// <param name="values"></param>
		public static void ConvertToRunningTotal(float[] values)
		{
			float total = 0f;
			for (int i = 0; i < values.Length; ++i)
			{
				total += values[i];
				values[i] = total;
			}
		}

		/// <summary>
		/// Creates and an array hold each alternate value from the input array.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public static float[] StripAlternateValues(float[] values)
		{
			float[] retVal = new float[values.Length / 2];
			for (int i = 0, j = 0; i < retVal.Length; ++i, j += 2)
			{
				retVal[i] = values[j];
			}
			return retVal;
		}

		/// <summary>
		/// Gets the difference between the arrays passed in. First Array
		/// needs to be twice the length or equal length to the seconds Array.
		/// </summary>
		/// <param name="personnelLevels"></param>
		/// <param name="targets"></param>
		/// <returns></returns>
		public static float[] GetSecondArrayMinusFirst(float[] personnelLevels, float[] targets)
		{

			float[] retVal = new float[personnelLevels.Length];

			if (personnelLevels.Length == targets.Length * 2)
			{
				for (int i = 0, j = 0; i < personnelLevels.Length; i += 2, ++j)
				{
					retVal[i] = targets[j] - personnelLevels[i];
					retVal[i + 1] = targets[j] - personnelLevels[i + 1];
				}
			}
			else if (personnelLevels.Length == targets.Length)
			{
				for (int i = 0; i < personnelLevels.Length; ++i)
				{
					retVal[i] = targets[i] - personnelLevels[i];
				}
			}
			else
				throw (new ApplicationException("First Array needs to be twice the Length or equal to the length of the second"));

			return retVal;
		}

		/// <summary>
		/// Converts the two arrays to an Array holding percentage values.
		/// </summary>
		/// <param name="numerator"></param>
		/// <param name="target"></param>
		/// <param name="divideZeroGivesZero"></param>
		/// <returns></returns>
		public static float[] GetPercent(float[] numerator, float[] target, bool divideZeroGivesZero)
		{
			// Two functions in one. Select the code path depending on the lengths of the arrays.
			// If the two arrays are equal the just divide the first one by the values in the
			// second. If the first array has twice as many values as the second, the use each
			// value in the second one twice in a row.

			// Return value length is equal to the first array
			float[] retVal = new float[numerator.Length];

			if (numerator.Length == target.Length)
			{
				for (int i = 0; i < retVal.Length; ++i)
				{
					if (target[i] != 0f)
					{
						retVal[i] = 100f * numerator[i] / target[i];
					}
					else if (!divideZeroGivesZero)
					{
						retVal[i] = 100f;
					}
				}
			}
			else if (numerator.Length == target.Length * 2)
			{
				for (int i = 0, j = 0; i < retVal.Length; i += 2, ++j)
				{
					if (target[j] != 0f)
					{
						retVal[i] = 100f * numerator[i] / target[j];
						retVal[i + 1] = 100f * numerator[i + 1] / target[j];
					}
					else if (!divideZeroGivesZero)
					{
						retVal[i] = 100f;
						retVal[i + 1] = 100f;
					}
				}
			}
			else
			{
				throw (new ApplicationException("First Array needs to be equal to or twice the Length of the second Array"));
			}
			return retVal;
		}

	}
}
