// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A control for entering a <see cref="TimeSpan"/>, with buttons to step it up and down.
    /// </summary>
    /// <remarks>
    /// This is the control for a length of time rather than a time of day: how long something runs,
    /// how long to wait. A <see cref="TimePicker"/> is the other one, and it stops at a day because
    /// its value is a point in time.
    /// <para>
    /// The step is whatever <see cref="NumericUpDownBase{T}.Interval"/> says and nothing else, so it
    /// does not change under the caret: <c>00:00:30</c> counts in half minutes wherever the cursor
    /// stands, <c>01:00:00</c> in hours.
    /// </para>
    /// <para>
    /// What is typed is read by <see cref="TimeSpan.TryParse(string, IFormatProvider, out TimeSpan)"/>,
    /// which reads a bare number as a count of days, <c>5</c> as five of them. Write <c>0:05</c> for
    /// five minutes.
    /// </para>
    /// </remarks>
    public class TimeSpanUpDown : NumericUpDownBase<TimeSpan>
    {
        /// <summary>
        /// What a value looks like when it is typed rather than written by the control: a number,
        /// then any number of groups behind a colon or a separator, the way a TimeSpan is written.
        /// It is what a paste is searched for, so that text around the value does not have to be
        /// cleaned up by hand.
        /// </summary>
        private static readonly Regex RegexTimeSpan = new(@"[-+]?[0-9]+(?:[:.,][0-9]+)*", RegexOptions.Compiled);

        static TimeSpanUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(typeof(TimeSpanUpDown)));

            MinimumProperty.OverrideMetadata(typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(TimeSpan.MinValue));
            MaximumProperty.OverrideMetadata(typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(TimeSpan.MaxValue));
            IntervalProperty.OverrideMetadata(typeof(TimeSpanUpDown), new FrameworkPropertyMetadata(TimeSpan.FromMinutes(1)));
        }

        /// <inheritdoc />
        /// <remarks>
        /// The number styles have nothing to say about a TimeSpan, so they are passed over. What the
        /// culture decides is the separator in front of the fraction of a second.
        /// </remarks>
        protected override bool TryParse(string text, NumberStyles style, IFormatProvider provider, out TimeSpan value)
        {
            return TimeSpan.TryParse(text, provider, out value);
        }

        /// <inheritdoc />
        /// <remarks>
        /// A TimeSpan throws where a number would wrap, and the step of a held button grows until it
        /// does. Both ends therefore stop at what a TimeSpan can hold.
        /// </remarks>
        protected override TimeSpan Add(TimeSpan left, TimeSpan right)
        {
            if (right.Ticks > 0 && left.Ticks > long.MaxValue - right.Ticks)
            {
                return TimeSpan.MaxValue;
            }

            if (right.Ticks < 0 && left.Ticks < long.MinValue - right.Ticks)
            {
                return TimeSpan.MinValue;
            }

            return TimeSpan.FromTicks(left.Ticks + right.Ticks);
        }

        /// <inheritdoc />
        protected override TimeSpan Multiply(TimeSpan value, double factor)
        {
            return FromTicks(value.Ticks * factor);
        }

        /// <inheritdoc />
        protected override TimeSpan Divide(TimeSpan value, double divisor)
        {
            return FromTicks(value.Ticks / divisor);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Behind the separator of a TimeSpan stands the fraction of a second, so that is what goes.
        /// </remarks>
        protected override TimeSpan Truncate(TimeSpan value)
        {
            return TimeSpan.FromTicks(value.Ticks - (value.Ticks % TimeSpan.TicksPerSecond));
        }

        /// <inheritdoc />
        protected override TimeSpan RoundToMultiple(TimeSpan value, TimeSpan multiple)
        {
            return FromTicks(Math.Round((double)value.Ticks / multiple.Ticks) * multiple.Ticks);
        }

        /// <inheritdoc />
        protected override int Compare(TimeSpan left, TimeSpan right)
        {
            return left.CompareTo(right);
        }

        /// <inheritdoc />
        protected override bool IsZero(TimeSpan value)
        {
            return value == TimeSpan.Zero;
        }

        /// <inheritdoc />
        /// <remarks>
        /// The short of the two built-in formats, which leaves out the days where there are none and
        /// the fraction of a second where it is zero, and which is read back by the same culture.
        /// </remarks>
        protected override string ToPlainString(TimeSpan value, CultureInfo culture)
        {
            return value.ToString("g", culture);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Seconds rather than ticks, because the step that grows while a button is held multiplies
        /// by this and ticks would run past what a TimeSpan can hold after a press or two.
        /// </remarks>
        protected override double ToDouble(TimeSpan value)
        {
            return value.TotalSeconds;
        }

        /// <inheritdoc />
        protected override TimeSpan FromDouble(double value)
        {
            return FromTicks(value * TimeSpan.TicksPerSecond);
        }

        /// <inheritdoc />
        /// <remarks>
        /// The value is not a number, so the number the base class would look for is not in there.
        /// </remarks>
        protected override string TakeNumberFrom(string text)
        {
            var match = RegexTimeSpan.Match(text);
            return match.Success ? match.Value : text;
        }

        /// <summary>
        /// Carries a count of ticks over into a TimeSpan and stops at either end rather than
        /// throwing, since the callers above arrive here with whatever an unbounded calculation
        /// came to.
        /// </summary>
        private static TimeSpan FromTicks(double ticks)
        {
            if (double.IsNaN(ticks))
            {
                return TimeSpan.Zero;
            }

            if (ticks >= long.MaxValue)
            {
                return TimeSpan.MaxValue;
            }

            if (ticks <= long.MinValue)
            {
                return TimeSpan.MinValue;
            }

            return TimeSpan.FromTicks((long)ticks);
        }
    }
}
