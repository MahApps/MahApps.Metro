// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using MahApps.Metro.Automation.Peers;

namespace MahApps.Metro.Controls
{
    public class MetroHeader : GroupBox
    {
        private const string PartHeaderPresenter = "PART_Header";

        /// <summary>Identifies the <see cref="Orientation"/> dependency property.</summary>
        public static readonly DependencyProperty OrientationProperty
            = DependencyProperty.Register(nameof(Orientation),
                                          typeof(Orientation),
                                          typeof(MetroHeader),
                                          new PropertyMetadata(Orientation.Vertical));

        /// <summary>
        /// Gets or sets the <see cref="Orientation"/> used for the header.
        /// </summary>
        /// <remarks>
        /// If set to <see cref="System.Windows.Controls.Orientation.Vertical"/> the header will be above the content.
        /// If set to <see cref="System.Windows.Controls.Orientation.Horizontal"/> the header will be to the left of the content.
        /// </remarks>
        public Orientation Orientation
        {
            get => (Orientation)this.GetValue(OrientationProperty);
            set => this.SetValue(OrientationProperty, value);
        }

        static MetroHeader()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MetroHeader), new FrameworkPropertyMetadata(typeof(MetroHeader)));
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.SetHeaderVisibility();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new MetroHeaderAutomationPeer(this);
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);

            this.SetHeaderVisibility();
        }

        private void SetHeaderVisibility()
        {
            if (this.GetTemplateChild(PartHeaderPresenter) is FrameworkElement headerPresenter)
            {
                if (this.Header is string headerText)
                {
                    headerPresenter.Visibility = string.IsNullOrEmpty(headerText)
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                }
                else
                {
                    headerPresenter.Visibility = this.Header != null
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }
    }
}