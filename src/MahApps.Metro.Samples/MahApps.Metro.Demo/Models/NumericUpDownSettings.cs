// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Windows;
using MahApps.Metro.Controls;
using MetroDemo.Core;

namespace MetroDemo.Models
{
    /// <summary>
    /// One set of settings for every up-down control on the page. The example view binds them
    /// through a style, so a switch flipped here reaches all four types at once.
    /// </summary>
    public class NumericUpDownSettings : ViewModelBase
    {
        private bool limitRange;

        /// <summary>
        /// Whether the bounds below are handed to the controls at all. Left off, each of them keeps
        /// the range of the type it holds, which is the only way a long can show a count no other
        /// type here could hold.
        /// </summary>
        public bool LimitRange
        {
            get => this.limitRange;
            set
            {
                if (this.Set(ref this.limitRange, value))
                {
                    this.OnPropertyChanged(nameof(this.EffectiveMinimum));
                    this.OnPropertyChanged(nameof(this.EffectiveMaximum));
                }
            }
        }

        private double minimum = -1000;

        public double Minimum
        {
            get => this.minimum;
            set
            {
                if (this.Set(ref this.minimum, value))
                {
                    this.OnPropertyChanged(nameof(this.EffectiveMinimum));
                }
            }
        }

        private double maximum = 1000;

        public double Maximum
        {
            get => this.maximum;
            set
            {
                if (this.Set(ref this.maximum, value))
                {
                    this.OnPropertyChanged(nameof(this.EffectiveMaximum));
                }
            }
        }

        /// <summary>
        /// The bound, or the unset value, which sends the control back to the default of its own type.
        /// </summary>
        public object EffectiveMinimum => this.LimitRange ? this.Minimum : DependencyProperty.UnsetValue;

        public object EffectiveMaximum => this.LimitRange ? this.Maximum : DependencyProperty.UnsetValue;

        private double interval = 1;

        public double Interval
        {
            get => this.interval;
            set => this.Set(ref this.interval, value);
        }

        private double? defaultValue;

        public double? DefaultValue
        {
            get => this.defaultValue;
            set => this.Set(ref this.defaultValue, value);
        }

        private bool isReadOnly;

        public bool IsReadOnly
        {
            get => this.isReadOnly;
            set => this.Set(ref this.isReadOnly, value);
        }

        private bool interceptArrowKeys = true;

        public bool InterceptArrowKeys
        {
            get => this.interceptArrowKeys;
            set => this.Set(ref this.interceptArrowKeys, value);
        }

        private bool interceptMouseWheel = true;

        public bool InterceptMouseWheel
        {
            get => this.interceptMouseWheel;
            set => this.Set(ref this.interceptMouseWheel, value);
        }

        private bool trackMouseWheelWhenMouseOver;

        public bool TrackMouseWheelWhenMouseOver
        {
            get => this.trackMouseWheelWhenMouseOver;
            set => this.Set(ref this.trackMouseWheelWhenMouseOver, value);
        }

        private bool interceptManualEnter = true;

        public bool InterceptManualEnter
        {
            get => this.interceptManualEnter;
            set => this.Set(ref this.interceptManualEnter, value);
        }

        private bool syncTextWithValueWhileEditing;

        public bool SyncTextWithValueWhileEditing
        {
            get => this.syncTextWithValueWhileEditing;
            set => this.Set(ref this.syncTextWithValueWhileEditing, value);
        }

        private bool hideUpDownButtons;

        public bool HideUpDownButtons
        {
            get => this.hideUpDownButtons;
            set => this.Set(ref this.hideUpDownButtons, value);
        }

        private bool switchUpDownButtons;

        public bool SwitchUpDownButtons
        {
            get => this.switchUpDownButtons;
            set => this.Set(ref this.switchUpDownButtons, value);
        }

        private bool upDownButtonsFocusable = true;

        public bool UpDownButtonsFocusable
        {
            get => this.upDownButtonsFocusable;
            set => this.Set(ref this.upDownButtonsFocusable, value);
        }

        private double upDownButtonsWidth = 20;

        public double UpDownButtonsWidth
        {
            get => this.upDownButtonsWidth;
            set => this.Set(ref this.upDownButtonsWidth, value);
        }

        private ButtonsAlignment buttonsAlignment = ButtonsAlignment.Right;

        public ButtonsAlignment ButtonsAlignment
        {
            get => this.buttonsAlignment;
            set => this.Set(ref this.buttonsAlignment, value);
        }

        private int delay = 500;

        public int Delay
        {
            get => this.delay;
            set => this.Set(ref this.delay, value);
        }

        private bool speedup = true;

        public bool Speedup
        {
            get => this.speedup;
            set => this.Set(ref this.speedup, value);
        }

        private bool snapToMultipleOfInterval;

        public bool SnapToMultipleOfInterval
        {
            get => this.snapToMultipleOfInterval;
            set => this.Set(ref this.snapToMultipleOfInterval, value);
        }

        private NumericInput numericInputMode = NumericInput.All;

        public NumericInput NumericInputMode
        {
            get => this.numericInputMode;
            set => this.Set(ref this.numericInputMode, value);
        }

        private NumberStyles parsingNumberStyle = NumberStyles.Any;

        /// <summary>
        /// What typed text is read with. A StringFormat of X sets this to HexNumber by itself, so
        /// changing it here is only for being stricter than the default.
        /// </summary>
        public NumberStyles ParsingNumberStyle
        {
            get => this.parsingNumberStyle;
            set => this.Set(ref this.parsingNumberStyle, value);
        }

        private DecimalPointCorrectionMode decimalPointCorrection = DecimalPointCorrectionMode.Inherits;

        public DecimalPointCorrectionMode DecimalPointCorrection
        {
            get => this.decimalPointCorrection;
            set => this.Set(ref this.decimalPointCorrection, value);
        }

        private string? stringFormat = string.Empty;

        public string? StringFormat
        {
            get => this.stringFormat;
            set => this.Set(ref this.stringFormat, value);
        }

        private CultureInfo? culture;

        public CultureInfo? Culture
        {
            get => this.culture;
            set => this.Set(ref this.culture, value);
        }

        private TextAlignment textAlignment = TextAlignment.Right;

        public TextAlignment TextAlignment
        {
            get => this.textAlignment;
            set => this.Set(ref this.textAlignment, value);
        }

        private TextAlignment watermarkAlignment = TextAlignment.Right;

        public TextAlignment WatermarkAlignment
        {
            get => this.watermarkAlignment;
            set => this.Set(ref this.watermarkAlignment, value);
        }

        private string? watermark = "Type a number";

        public string? Watermark
        {
            get => this.watermark;
            set => this.Set(ref this.watermark, value);
        }

        private bool clearTextButton = true;

        public bool ClearTextButton
        {
            get => this.clearTextButton;
            set => this.Set(ref this.clearTextButton, value);
        }
    }
}
