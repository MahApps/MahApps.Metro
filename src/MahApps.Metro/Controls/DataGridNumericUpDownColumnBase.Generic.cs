// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// The half of an up-down column that knows what kind of number the cell holds: the bounds, the
    /// step, and which control to put in the cell.
    /// </summary>
    /// <typeparam name="TControl">The control this column puts in a cell.</typeparam>
    /// <typeparam name="T">The type of the value, for instance <see cref="double"/> or <see cref="decimal"/>.</typeparam>
    /// <remarks>
    /// A type deriving from this has to say what its bounds and its step default to, through
    /// OverrideMetadata in its static constructor, the same way the control itself does.
    /// </remarks>
    public abstract class DataGridNumericUpDownColumnBase<TControl, T> : DataGridNumericUpDownColumnBase
        where TControl : NumericUpDownBase<T>, new()
        where T : struct, System.IComparable<T>, System.IFormattable
    {
        // The three below are registered rather than added as an owner of the property the control
        // has. AddOwner would tie them to one that is itself registered per closed type, and the
        // column hands its values over itself anyway, in SyncTypedProperties.

        /// <summary>Identifies the <see cref="Minimum"/> dependency property.</summary>
        public static readonly DependencyProperty MinimumProperty
            = DependencyProperty.Register(nameof(Minimum),
                                          typeof(T),
                                          typeof(DataGridNumericUpDownColumnBase<TControl, T>),
                                          new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// Gets or sets the lowest value a cell of this column accepts.
        /// </summary>
        public T Minimum
        {
            get => (T)this.GetValue(MinimumProperty);
            set => this.SetValue(MinimumProperty, value);
        }

        /// <summary>Identifies the <see cref="Maximum"/> dependency property.</summary>
        public static readonly DependencyProperty MaximumProperty
            = DependencyProperty.Register(nameof(Maximum),
                                          typeof(T),
                                          typeof(DataGridNumericUpDownColumnBase<TControl, T>),
                                          new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// Gets or sets the highest value a cell of this column accepts.
        /// </summary>
        public T Maximum
        {
            get => (T)this.GetValue(MaximumProperty);
            set => this.SetValue(MaximumProperty, value);
        }

        /// <summary>Identifies the <see cref="Interval"/> dependency property.</summary>
        public static readonly DependencyProperty IntervalProperty
            = DependencyProperty.Register(nameof(Interval),
                                          typeof(T),
                                          typeof(DataGridNumericUpDownColumnBase<TControl, T>),
                                          new FrameworkPropertyMetadata(default(T), FrameworkPropertyMetadataOptions.Inherits, NotifyPropertyChangeForRefreshContent));

        /// <summary>
        /// Gets or sets how much a step up or down changes the value in a cell of this column.
        /// </summary>
        public T Interval
        {
            get => (T)this.GetValue(IntervalProperty);
            set => this.SetValue(IntervalProperty, value);
        }

        /// <summary>
        /// The one control this column keeps for itself, never shown and never in a tree, purely to be
        /// asked how a value would look. Making one is cheap; what costs is the template, and that is
        /// only applied to a control that is put on screen.
        /// </summary>
        private readonly TControl formatter = new TControl();

        private IMultiValueConverter? textConverter;

        /// <inheritdoc />
        protected override IMultiValueConverter TextConverter => this.textConverter ??= new ValueToText(this.formatter);

        /// <inheritdoc />
        protected override NumericUpDownBase TakeOrMakeControl(DataGridCell? cell)
        {
            return cell?.Content as TControl ?? new TControl();
        }

        /// <inheritdoc />
        protected override void SyncTypedProperties(NumericUpDownBase control)
        {
            SyncColumnProperty(this, control, MinimumProperty, NumericUpDownBase<T>.MinimumProperty);
            SyncColumnProperty(this, control, MaximumProperty, NumericUpDownBase<T>.MaximumProperty);
            SyncColumnProperty(this, control, IntervalProperty, NumericUpDownBase<T>.IntervalProperty);
        }

        /// <inheritdoc />
        protected override void SyncTypedProperty(NumericUpDownBase control, string propertyName)
        {
            switch (propertyName)
            {
                case nameof(this.Minimum):
                    SyncColumnProperty(this, control, MinimumProperty, NumericUpDownBase<T>.MinimumProperty);
                    break;
                case nameof(this.Maximum):
                    SyncColumnProperty(this, control, MaximumProperty, NumericUpDownBase<T>.MaximumProperty);
                    break;
                case nameof(this.Interval):
                    SyncColumnProperty(this, control, IntervalProperty, NumericUpDownBase<T>.IntervalProperty);
                    break;
                default:
                    // Everything else on the column does not depend on the type, and the half that
                    // holds those has already dealt with it by the time this is asked.
                    break;
            }
        }

        /// <inheritdoc />
        protected override void BindValue(NumericUpDownBase control)
        {
            ApplyBinding(this.Binding, control, NumericUpDownBase<T>.ValueProperty);
        }

        /// <inheritdoc />
        protected override object? ValueOf(NumericUpDownBase control)
        {
            return ((TControl)control).Value;
        }

        /// <summary>
        /// Turns the value of a cell into the text the control would have shown, given the format and
        /// the culture the column carries.
        /// </summary>
        private sealed class ValueToText : IMultiValueConverter
        {
            private readonly TControl formatter;

            public ValueToText(TControl formatter)
            {
                this.formatter = formatter;
            }

            public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
            {
                if (values.Length < 3 || values[0] == DependencyProperty.UnsetValue)
                {
                    return string.Empty;
                }

                var value = values[0] switch
                             {
                                 T typed => typed,
                                 null => (T?)null,
                                 IConvertible convertible => (T)System.Convert.ChangeType(convertible, typeof(T), culture),
                                 _ => (T?)null
                             };

                var format = values[1] as string ?? string.Empty;
                var itsCulture = values[2] as CultureInfo ?? culture;

                return this.formatter.TextFor(value, format, itsCulture) ?? string.Empty;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
