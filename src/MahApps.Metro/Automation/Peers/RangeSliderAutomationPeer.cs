// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Globalization;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls.Primitives;
using MahApps.Metro.Controls;

namespace MahApps.Metro.Automation.Peers
{
    /// <summary>
    /// A range slider holds two values, and no automation pattern holds two. The control says what it
    /// is and reads out both ends at once, and each of the two outer thumbs carries the value it
    /// stands for, so a client has something to read and something to set. The thumb in the middle
    /// moves the whole range and stands for no value of its own.
    /// </summary>
    public class RangeSliderAutomationPeer : FrameworkElementAutomationPeer
    {
        public RangeSliderAutomationPeer(RangeSlider owner)
            : base(owner)
        {
        }

        private RangeSlider Slider => (RangeSlider)this.Owner;

        protected override string GetClassNameCore()
        {
            return "RangeSlider";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Slider;
        }

        protected override string GetItemStatusCore()
        {
            var slider = this.Slider;

            return string.Format(CultureInfo.CurrentCulture, "{0} - {1}", slider.LowerValue, slider.UpperValue);
        }

        protected override List<AutomationPeer>? GetChildrenCore()
        {
            var children = base.GetChildrenCore();

            if (children is null)
            {
                return null;
            }

            var slider = this.Slider;

            this.HandOverTheValue(children, slider.FindChild<Thumb>("PART_LeftThumb"), RangeSliderThumb.Lower);
            this.HandOverTheValue(children, slider.FindChild<Thumb>("PART_RightThumb"), RangeSliderThumb.Upper);

            return children;
        }

        /// <summary>
        /// Swaps the peer of one thumb for one that carries the value that thumb stands for.
        /// </summary>
        private void HandOverTheValue(List<AutomationPeer> children, Thumb? thumb, RangeSliderThumb which)
        {
            if (thumb is null)
            {
                return;
            }

            for (var i = 0; i < children.Count; i++)
            {
                if (children[i] is UIElementAutomationPeer peer && ReferenceEquals(peer.Owner, thumb))
                {
                    children[i] = new RangeSliderThumbAutomationPeer(thumb, this.Slider, which);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Which end of the range a thumb stands for.
    /// </summary>
    public enum RangeSliderThumb
    {
        Lower,
        Upper
    }

    /// <summary>
    /// One end of a range slider, handed over as a value a client can read and set.
    /// </summary>
    public class RangeSliderThumbAutomationPeer : ThumbAutomationPeer, IRangeValueProvider
    {
        private readonly RangeSlider slider;
        private readonly RangeSliderThumb which;

        public RangeSliderThumbAutomationPeer(Thumb owner, RangeSlider slider, RangeSliderThumb which)
            : base(owner)
        {
            this.slider = slider;
            this.which = which;
        }

        public bool IsReadOnly => this.slider.IsEnabled == false;

        public double LargeChange => this.slider.LargeChange;

        public double SmallChange => this.slider.SmallChange;

        public double Maximum => this.slider.Maximum;

        public double Minimum => this.slider.Minimum;

        public double Value => this.which == RangeSliderThumb.Lower ? this.slider.LowerValue : this.slider.UpperValue;

        public override object? GetPattern(PatternInterface patternInterface)
        {
            return patternInterface == PatternInterface.RangeValue ? this : base.GetPattern(patternInterface);
        }

        public void SetValue(double value)
        {
            if (this.IsReadOnly)
            {
                throw new ElementNotEnabledException();
            }

            // the slider itself keeps one end from passing the other
            this.slider.SetCurrentValue(this.which == RangeSliderThumb.Lower ? RangeSlider.LowerValueProperty : RangeSlider.UpperValueProperty, value);
        }

        protected override string GetClassNameCore()
        {
            return "RangeSliderThumb";
        }

        protected override AutomationControlType GetAutomationControlTypeCore()
        {
            return AutomationControlType.Thumb;
        }
    }
}
