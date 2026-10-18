// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// Ported from RatingControlAutomationPeer.cpp of microsoft-ui-xaml, Copyright (c) Microsoft Corporation, MIT License.

using System;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using MahApps.Metro.Controls;
using AutomationStrings = MahApps.Metro.Lang.Automation;

namespace MahApps.Metro.Automation.Peers
{
    public class RatingControlAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider, IValueProvider
    {
        public RatingControlAutomationPeer(RatingControl owner)
            : base(owner)
        {
        }

        private RatingControl RatingControl => (RatingControl)this.Owner;

        protected override string GetClassNameCore()
        {
            return nameof(RatingControl);
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Slider;
        }

        protected override string GetLocalizedControlTypeCore()
        {
            return AutomationStrings.RatingLocalizedControlType;
        }

        public override object? GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Value || patternInterface == PatternInterface.RangeValue)
            {
                return this;
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// UIA does not tolerate a missing value, so -1 goes out as 0.
        /// </summary>
        internal void RaiseValuePropertyChangedEvent(double oldValue, double newValue)
        {
            var oldRange = oldValue == -1 ? 0.0 : oldValue;
            var newRange = newValue == -1 ? 0.0 : newValue;

            this.RaisePropertyChangedEvent(ValuePatternIdentifiers.ValueProperty, this.GetValueString(oldValue), this.GetValueString(newValue));
            this.RaisePropertyChangedEvent(RangeValuePatternIdentifiers.ValueProperty, oldRange, newRange);
        }

        // IValueProvider

        bool IValueProvider.IsReadOnly => this.RatingControl.IsReadOnly;

        string IValueProvider.Value => this.GetValueString(this.RatingControl.Value);

        void IValueProvider.SetValue(string value)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.CurrentCulture, out var rating))
            {
                this.SetRating(rating);
            }
        }

        // IRangeValueProvider

        bool IRangeValueProvider.IsReadOnly => this.RatingControl.IsReadOnly;

        double IRangeValueProvider.SmallChange => 1.0;

        double IRangeValueProvider.LargeChange => 1.0;

        double IRangeValueProvider.Maximum => this.RatingControl.MaxRating;

        double IRangeValueProvider.Minimum => 0.0;

        double IRangeValueProvider.Value
        {
            get
            {
                var value = this.RatingControl.Value;
                return value == -1 ? 0.0 : value;
            }
        }

        void IRangeValueProvider.SetValue(double value)
        {
            this.SetRating(value);
        }

        private void SetRating(double value)
        {
            if (!this.IsEnabled())
            {
                throw new ElementNotEnabledException();
            }

            this.RatingControl.SetCurrentValue(RatingControl.ValueProperty, value);
        }

        private string GetValueString(double ratingValue)
        {
            if (ratingValue == -1)
            {
                var placeholderValue = this.RatingControl.PlaceholderValue;
                return placeholderValue == -1
                    ? AutomationStrings.RatingUnset
                    : this.GenerateValueString(AutomationStrings.CommunityRatingString, placeholderValue);
            }

            return this.GenerateValueString(AutomationStrings.BasicRatingString, ratingValue);
        }

        private string GenerateValueString(string format, double ratingValue)
        {
            var culture = CultureInfo.CurrentCulture;
            var rating = ratingValue.ToString("F" + DetermineFractionDigits(ratingValue), culture);
            return string.Format(culture, format, rating, this.RatingControl.MaxRating.ToString(culture));
        }

        // read out the least number of digits possible, a 3 is not a 3.00
        private static int DetermineFractionDigits(double value)
        {
            // rounded, since 2.3 * 100 comes out just below 230
            var intValue = (int)Math.Round(value * 100);
            if (intValue % 100 == 0)
            {
                return 0;
            }

            return intValue % 10 == 0 ? 1 : 2;
        }
    }
}
