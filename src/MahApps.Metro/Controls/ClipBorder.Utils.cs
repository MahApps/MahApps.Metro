// Copyright (c) 2017 Ratish Philip 
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions: 
// 
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software. 
// 
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE. 
//
// This file is part of the WPFSpark project: https://github.com/ratishphilip/wpfspark
//
// WPFSpark v1.3.1
// 

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A few very useful extension methods
    /// </summary>
    public static class Utils
    {
        #region Double

        // Const values 
        // Smallest double value such that 1.0+DBL_EPSILON != 1.0
        internal const double DBL_EPSILON = 2.2204460492503131e-016; 
        // Number close to zero, where float.MinValue is -float.MaxValue
        internal const float FLT_MIN = 1.175494351e-38F;

        /// <summary>
        /// Returns whether or not two doubles are "close". 
        /// </summary>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        public static bool IsCloseTo(this double value1, double value2)
        {
            //in case they are Infinities (then epsilon check does not work)
            if (value1 == value2) return true;
            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            var delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// Returns whether or not the first double is less than the second double.
        /// </summary>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns>
        /// bool - the result of the LessThan comparision.
        /// </returns>
        public static bool IsLessThan(double value1, double value2)
        {
            return (value1 < value2) && !value1.IsCloseTo(value2);
        }

        /// <summary>
        /// Returns whether or not the first double is greater than the second double.
        /// </summary>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns>
        /// bool - the result of the GreaterThan comparision.
        /// </returns>
        public static bool IsGreaterThan(this double value1, double value2)
        {
            return (value1 > value2) && !value1.IsCloseTo(value2);
        }

        /// <summary>
        /// Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
        /// but this is faster.
        /// </summary>
        /// <param name="value"> The double to compare to 1. </param>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        public static bool IsOne(this double value)
        {
            return Math.Abs(value - 1.0) < 10.0 * DBL_EPSILON;
        }

        /// <summary>
        /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        /// <param name="value"> The double to compare to 0. </param>
        /// <returns>
        /// bool - the result of the AreClose comparision.
        /// </returns>
        public static bool IsZero(this double value)
        {
            return Math.Abs(value) < 10.0 * DBL_EPSILON;
        }

        /// <summary>
        /// Compares two points for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='point1'>The first point to compare</param>
        /// <param name='point2'>The second point to compare</param>
        /// <returns>Whether or not the two points are equal</returns>
        public static bool IsCloseTo(this Point point1, Point point2)
        {
            return point1.X.IsCloseTo(point2.X) && point1.Y.IsCloseTo(point2.Y);
        }

        /// <summary>
        /// Compares two Size instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='size1'>The first size to compare</param>
        /// <param name='size2'>The second size to compare</param>
        /// <returns>Whether or not the two Size instances are equal</returns>
        public static bool IsCloseTo(this Size size1, Size size2)
        {
            return size1.Width.IsCloseTo(size2.Width) && size1.Height.IsCloseTo(size2.Height);
        }

        /// <summary>
        /// Compares two Vector instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='vector1'>The first Vector to compare</param>
        /// <param name='vector2'>The second Vector to compare</param>
        /// <returns>Whether or not the two Vector instances are equal</returns>
        public static bool IsCloseTo(this Vector vector1, Vector vector2)
        {
            return vector1.X.IsCloseTo(vector2.X) && vector1.Y.IsCloseTo(vector2.Y);
        }

        /// <summary>
        /// Compares two rectangles for fuzzy equality.  This function
        /// helps compensate for the fact that double values can 
        /// acquire error when operated upon
        /// </summary>
        /// <param name='rect1'>The first rectangle to compare</param>
        /// <param name='rect2'>The second rectangle to compare</param>
        /// <returns>Whether or not the two rectangles are equal</returns>
        public static bool IsCloseTo(this Rect rect1, Rect rect2)
        {
            // If they're both empty, don't bother with the double logic.
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            // At this point, rect1 isn't empty, so the first thing we can test is
            // rect2.IsEmpty, followed by property-wise compares.

            return (!rect2.IsEmpty)
                   && rect1.X.IsCloseTo(rect2.X) && rect1.Y.IsCloseTo(rect2.Y)
                   && rect1.Height.IsCloseTo(rect2.Height) && rect1.Width.IsCloseTo(rect2.Width);
        }

        [StructLayout(LayoutKind.Explicit)]
        private struct NanUnion
        {
            [FieldOffset(0)]
            internal double DoubleValue;
            [FieldOffset(0)]
            internal UInt64 UintValue;
        }
        
        /// <summary>
        /// Faster check for NaN ( faster than double.IsNaN() )
        /// IEEE 754 : If the argument is any value in the range 0x7ff0000000000001L through 0x7fffffffffffffffL 
        /// or in the range 0xfff0000000000001L through 0xffffffffffffffffL, the result will be NaN.
        /// </summary>
        /// <param name="value">Value to check</param>
        /// <returns></returns>
        public static bool IsNaN(double value)
        {
            var t = new NanUnion
            {
                DoubleValue = value
            };

            var exp = t.UintValue & 0xfff0000000000000;
            var man = t.UintValue & 0x000fffffffffffff;

            return (exp == 0x7ff0000000000000 || exp == 0xfff0000000000000) && (man != 0);
        }

        /// <summary>
        /// Rounds the given value based on the DPI scale
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="dpiScale">DPI Scale</param>
        /// <returns></returns>
        public static double RoundLayoutValue(double value, double dpiScale)
        {
            double newValue;

            // If DPI == 1, don't use DPI-aware rounding. 
            if (!dpiScale.IsCloseTo(1.0))
            {
                newValue = Math.Round(value * dpiScale) / dpiScale;
                // If rounding produces a value unacceptable to layout (NaN, Infinity or MaxValue), use the original value.
                if (IsNaN(newValue) ||
                    Double.IsInfinity(newValue) ||
                    newValue.IsCloseTo(Double.MaxValue))
                {
                    newValue = value;
                }
            }
            else
            {
                newValue = Math.Round(value);
            }

            return newValue;
        }
        #endregion

        #region Thickness

        /// <summary>
        /// Verifies if this Thickness contains only valid values
        /// The set of validity checks is passed as parameters.
        /// </summary>
        /// <param name='thick'>Thickness value</param>
        /// <param name='allowNegative'>allows negative values</param>
        /// <param name='allowNaN'>allows Double.NaN</param>
        /// <param name='allowPositiveInfinity'>allows Double.PositiveInfinity</param>
        /// <param name='allowNegativeInfinity'>allows Double.NegativeInfinity</param>
        /// <returns>Whether or not the thickness complies to the range specified</returns>
        public static bool IsValid(this Thickness thick, bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
        {
            if (!allowNegative)
            {
                if (thick.Left < 0d || thick.Right < 0d || thick.Top < 0d || thick.Bottom < 0d)
                    return false;
            }

            if (!allowNaN)
            {
                if (IsNaN(thick.Left) || IsNaN(thick.Right)
                    || IsNaN(thick.Top) || IsNaN(thick.Bottom))
                    return false;
            }

            if (!allowPositiveInfinity)
            {
                if (Double.IsPositiveInfinity(thick.Left) || Double.IsPositiveInfinity(thick.Right)
                    || Double.IsPositiveInfinity(thick.Top) || Double.IsPositiveInfinity(thick.Bottom))
                {
                    return false;
                }
            }

            if (!allowNegativeInfinity)
            {
                if (Double.IsNegativeInfinity(thick.Left) || Double.IsNegativeInfinity(thick.Right)
                    || Double.IsNegativeInfinity(thick.Top) || Double.IsNegativeInfinity(thick.Bottom))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Method to add up the left and right size as width, as well as the top and bottom size as height
        /// </summary>
        /// <param name="thick">Thickness</param>
        /// <returns>Size</returns>
        public static Size CollapseThickness(this Thickness thick)
        {
            return new Size(thick.Left + thick.Right, thick.Top + thick.Bottom);
        }

        /// <summary>
        /// Verifies if the Thickness contains only zero values
        /// </summary>
        /// <param name="thick">Thickness</param>
        /// <returns>Size</returns>
        public static bool IsZero(this Thickness thick)
        {
            return thick.Left.IsZero()
                    && thick.Top.IsZero()
                    && thick.Right.IsZero()
                    && thick.Bottom.IsZero();
        }

        /// <summary>
        /// Verifies if all the values in Thickness are same
        /// </summary>
        /// <param name="thick">Thickness</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsUniform(this Thickness thick)
        {
            return thick.Left.IsCloseTo(thick.Top)
                    && thick.Left.IsCloseTo(thick.Right)
                    && thick.Left.IsCloseTo(thick.Bottom);
        }

        #endregion

        #region CornerRadius

        /// <summary>
        /// Verifies if this CornerRadius contains only valid values
        /// The set of validity checks is passed as parameters.
        /// </summary>
        /// <param name='corner'>CornerRadius value</param>
        /// <param name='allowNegative'>allows negative values</param>
        /// <param name='allowNaN'>allows Double.NaN</param>
        /// <param name='allowPositiveInfinity'>allows Double.PositiveInfinity</param>
        /// <param name='allowNegativeInfinity'>allows Double.NegativeInfinity</param>
        /// <returns>Whether or not the CornerRadius complies to the range specified</returns>
        public static bool IsValid(this CornerRadius corner, bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
        {
            if (!allowNegative)
            {
                if (corner.TopLeft < 0d || corner.TopRight < 0d || corner.BottomLeft < 0d || corner.BottomRight < 0d)
                {
                    return (false);
                }
            }

            if (!allowNaN)
            {
                if (IsNaN(corner.TopLeft) || IsNaN(corner.TopRight) ||
                    IsNaN(corner.BottomLeft) || IsNaN(corner.BottomRight))
                {
                    return (false);
                }
            }

            if (!allowPositiveInfinity)
            {
                if (Double.IsPositiveInfinity(corner.TopLeft) || Double.IsPositiveInfinity(corner.TopRight) ||
                    Double.IsPositiveInfinity(corner.BottomLeft) || Double.IsPositiveInfinity(corner.BottomRight))
                {
                    return (false);
                }
            }

            if (!allowNegativeInfinity)
            {
                if (Double.IsNegativeInfinity(corner.TopLeft) || Double.IsNegativeInfinity(corner.TopRight) ||
                    Double.IsNegativeInfinity(corner.BottomLeft) || Double.IsNegativeInfinity(corner.BottomRight))
                {
                    return (false);
                }
            }

            return true;
        }

        /// <summary>
        /// Verifies if the CornerRadius contains only zero values
        /// </summary>
        /// <param name="corner">CornerRadius</param>
        /// <returns>Size</returns>
        public static bool IsZero(this CornerRadius corner)
        {
            return corner.TopLeft.IsZero()
                   && corner.TopRight.IsZero()
                   && corner.BottomRight.IsZero()
                   && corner.BottomLeft.IsZero();
        }

        /// <summary>
        /// Verifies if the CornerRadius contains same values
        /// </summary>
        /// <param name="corner">CornerRadius</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsUniform(this CornerRadius corner)
        {
            var topLeft = corner.TopLeft;
            return topLeft.IsCloseTo(corner.TopRight) &&
                   topLeft.IsCloseTo(corner.BottomLeft) &&
                   topLeft.IsCloseTo(corner.BottomRight);
        }

        #endregion

        #region Rect

        /// <summary>
        /// Deflates rectangle by given thickness
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="thick">Thickness</param>
        /// <returns>Deflated Rectangle</returns>
        public static Rect Deflate(this Rect rect, Thickness thick)
        {
            return new Rect(rect.Left + thick.Left,
                rect.Top + thick.Top,
                Math.Max(0.0, rect.Width - thick.Left - thick.Right),
                Math.Max(0.0, rect.Height - thick.Top - thick.Bottom));
        }

        /// <summary>
        /// Inflates rectangle by given thickness
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="thick">Thickness</param>
        /// <returns>Inflated Rectangle</returns>
        public static Rect Inflate(this Rect rect, Thickness thick)
        {
            return new Rect(rect.Left - thick.Left,
                rect.Top - thick.Top,
                Math.Max(0.0, rect.Width + thick.Left + thick.Right),
                Math.Max(0.0, rect.Height + thick.Top + thick.Bottom));
        }

        #endregion

        #region Brush

        /// <summary>
        /// Verifies if the given brush is a SolidColorBrush and
        /// its color does not include transparency.
        /// </summary>
        /// <param name="brush">Brush</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsOpaqueSolidColorBrush(this Brush brush)
        {
            return (brush as SolidColorBrush)?.Color.A == 0xff;
        }

        /// <summary>
        /// Verifies if the given brush is the same as the otherBrush.
        /// </summary>
        /// <param name="brush">Brush</param>
        /// <param name="otherBrush">Brush</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsEqualTo(this Brush brush, Brush otherBrush)
        {
            if (brush.GetType() != otherBrush.GetType())
                return false;

            if (ReferenceEquals(brush, otherBrush))
                return true;

            // Are both instances of SolidColorBrush
            var solidBrushA = brush as SolidColorBrush;
            var solidBrushB = otherBrush as SolidColorBrush;
            if ((solidBrushA != null) && (solidBrushB != null))
            {
                return (solidBrushA.Color == solidBrushB.Color)
                       && solidBrushA.Opacity.IsCloseTo(solidBrushB.Opacity);
            }

            // Are both instances of LinearGradientBrush
            var linGradBrushA = brush as LinearGradientBrush;
            var linGradBrushB = otherBrush as LinearGradientBrush;

            if ((linGradBrushA != null) && (linGradBrushB != null))
            {
                var result = (linGradBrushA.ColorInterpolationMode == linGradBrushB.ColorInterpolationMode)
                               && (linGradBrushA.EndPoint == linGradBrushB.EndPoint)
                               && (linGradBrushA.MappingMode == linGradBrushB.MappingMode)
                               && linGradBrushA.Opacity.IsCloseTo(linGradBrushB.Opacity)
                               && (linGradBrushA.StartPoint == linGradBrushB.StartPoint)
                               && (linGradBrushA.SpreadMethod == linGradBrushB.SpreadMethod)
                               && (linGradBrushA.GradientStops.Count == linGradBrushB.GradientStops.Count);
                if (!result)
                    return false;

                for (var i = 0; i < linGradBrushA.GradientStops.Count; i++)
                {
                    result = (linGradBrushA.GradientStops[i].Color == linGradBrushB.GradientStops[i].Color)
                             && linGradBrushA.GradientStops[i].Offset.IsCloseTo(linGradBrushB.GradientStops[i].Offset);

                    if (!result)
                        break;
                }

                return result;
            }

            // Are both instances of RadialGradientBrush
            var radGradBrushA = brush as RadialGradientBrush;
            var radGradBrushB = otherBrush as RadialGradientBrush;

            if ((radGradBrushA != null) && (radGradBrushB != null))
            {
                var result = (radGradBrushA.ColorInterpolationMode == radGradBrushB.ColorInterpolationMode)
                             && (radGradBrushA.GradientOrigin == radGradBrushB.GradientOrigin)
                             && (radGradBrushA.MappingMode == radGradBrushB.MappingMode)
                             && radGradBrushA.Opacity.IsCloseTo(radGradBrushB.Opacity)
                             && radGradBrushA.RadiusX.IsCloseTo(radGradBrushB.RadiusX)
                             && radGradBrushA.RadiusY.IsCloseTo(radGradBrushB.RadiusY)
                             && (radGradBrushA.SpreadMethod == radGradBrushB.SpreadMethod)
                             && (radGradBrushA.GradientStops.Count == radGradBrushB.GradientStops.Count);

                if (!result)
                    return false;

                for (var i = 0; i < radGradBrushA.GradientStops.Count; i++)
                {
                    result = (radGradBrushA.GradientStops[i].Color == radGradBrushB.GradientStops[i].Color)
                             && radGradBrushA.GradientStops[i].Offset.IsCloseTo(radGradBrushB.GradientStops[i].Offset);

                    if (!result)
                        break;
                }

                return result;
            }

            // Are both instances of ImageBrush
            var imgBrushA = brush as ImageBrush;
            var imgBrushB = otherBrush as ImageBrush;
            if ((imgBrushA != null) && (imgBrushB != null))
            {
                var result = (imgBrushA.AlignmentX == imgBrushB.AlignmentX)
                              && (imgBrushA.AlignmentY == imgBrushB.AlignmentY)
                              && imgBrushA.Opacity.IsCloseTo(imgBrushB.Opacity)
                              && (imgBrushA.Stretch == imgBrushB.Stretch)
                              && (imgBrushA.TileMode == imgBrushB.TileMode)
                              && (imgBrushA.Viewbox == imgBrushB.Viewbox)
                              && (imgBrushA.ViewboxUnits == imgBrushB.ViewboxUnits)
                              && (imgBrushA.Viewport == imgBrushB.Viewport)
                              && (imgBrushA.ViewportUnits == imgBrushB.ViewportUnits)
                              && (imgBrushA.ImageSource == imgBrushB.ImageSource);

                return result;
            }

            return false;
        }

        #endregion
    }
}
