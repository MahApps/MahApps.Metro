// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using MahApps.Metro.Automation.Peers;

namespace MahApps.Metro.Controls
{
    /// <summary>
    /// A clock face with an hour, a minute and a second hand, the one the drop-down of a
    /// <see cref="TimePickerBase"/> carries.
    /// <para/>
    /// It is a control of its own rather than a piece of markup inside the picker, so its face is a
    /// template and its look is a style, and either can be replaced without copying the picker along
    /// with it. The face is drawn at a natural size of 120 by 120 and scaled to whatever size the
    /// clock is given, which is what <see cref="TimePickerBase.ClockSize"/> sets from the picker.
    /// </summary>
    [TemplatePart(Name = ElementHourHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementMinuteHand, Type = typeof(UIElement))]
    [TemplatePart(Name = ElementSecondHand, Type = typeof(UIElement))]
    public class AnalogClock : Control
    {
        private const string ElementHourHand = "PART_HourHand";
        private const string ElementMinuteHand = "PART_MinuteHand";
        private const string ElementSecondHand = "PART_SecondHand";

        private UIElement? hourHand;
        private UIElement? minuteHand;
        private UIElement? secondHand;

        static AnalogClock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AnalogClock), new FrameworkPropertyMetadata(typeof(AnalogClock)));
        }

        /// <summary>Identifies the <see cref="Time"/> dependency property.</summary>
        public static readonly DependencyProperty TimeProperty
            = DependencyProperty.Register(nameof(Time),
                                          typeof(DateTime?),
                                          typeof(AnalogClock),
                                          new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the time the hands point at. Only the time of day is read, the date that
        /// comes with it is left alone.
        /// </summary>
        [Category("Common")]
        public DateTime? Time
        {
            get => (DateTime?)this.GetValue(TimeProperty);
            set => this.SetValue(TimeProperty, value);
        }

        /// <summary>Identifies the <see cref="HandVisibility"/> dependency property.</summary>
        public static readonly DependencyProperty HandVisibilityProperty
            = DependencyProperty.Register(nameof(HandVisibility),
                                          typeof(TimePartVisibility),
                                          typeof(AnalogClock),
                                          new PropertyMetadata(TimePartVisibility.All, OnHandVisibilityChanged));

        private static void OnHandVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AnalogClock)d).SetHandVisibility((TimePartVisibility)e.NewValue);
        }

        /// <summary>
        /// Gets or sets which of the three hands are on the face.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(TimePartVisibility.All)]
        public TimePartVisibility HandVisibility
        {
            get => (TimePartVisibility)this.GetValue(HandVisibilityProperty);
            set => this.SetValue(HandVisibilityProperty, value);
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.hourHand = this.GetTemplateChild(ElementHourHand) as UIElement;
            this.minuteHand = this.GetTemplateChild(ElementMinuteHand) as UIElement;
            this.secondHand = this.GetTemplateChild(ElementSecondHand) as UIElement;

            this.SetHandVisibility(this.HandVisibility);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new AnalogClockAutomationPeer(this);
        }

        private void SetHandVisibility(TimePartVisibility visibility)
        {
            if (this.hourHand is not null)
            {
                this.hourHand.Visibility = visibility.HasFlag(TimePartVisibility.Hour) ? Visibility.Visible : Visibility.Collapsed;
            }

            if (this.minuteHand is not null)
            {
                this.minuteHand.Visibility = visibility.HasFlag(TimePartVisibility.Minute) ? Visibility.Visible : Visibility.Collapsed;
            }

            if (this.secondHand is not null)
            {
                this.secondHand.Visibility = visibility.HasFlag(TimePartVisibility.Second) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
